namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Serialization;
    using System;
    using System.Collections.Generic;

    using DebugUI;


    public class CharacterLocomotion : RigidbodyCharacterController
    {
        

        public event Action<bool> OnAim = delegate {};


        //  --  Character Actions
        [SerializeField, HideInInspector, FormerlySerializedAs("m_actions")]
        protected CharacterAction[] m_actions;
        [SerializeField, HideInInspector, FormerlySerializedAs("m_activeAction")]
        protected CharacterAction m_activeAction;

        protected Dictionary<CharacterAction, int> m_actionInfo;






        private bool m_frameUpdated;
        //  Used to update the animator every X second.
        private float m_animatorUpdateTimer;


        private Vector3 leftFootPosition, rightFootPosition;



        #region Properties
        public float timeScale { get {
                m_timeScale = Time.timeScale;
                return m_timeScale; }
        }

        public bool Aiming { get; set; }

        public bool CanAim { get => Grounded; }


        public float viewAngle
        {
            get { return m_viewAngle; }
            set { m_viewAngle = Mathf.Clamp(value, -180, 180); }
        }

        public CharacterAction[] CharActions { get { return m_actions; } set { m_actions = value; } }

        public int ActiveActionIndex{
            get {
                if (m_activeAction != null)
                    return Array.IndexOf(m_actions, m_activeAction);
                return -1;
            }
        }
        

        #endregion







        private void Awake()
        {
            m_gameObject = gameObject;
            m_transform = transform;

            m_animatorMonitor = GetComponent<AnimatorMonitor>();
            m_animator = GetComponent<Animator>();
            m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_animator.applyRootMotion = m_useRootMotionPosition || m_useRootMotionRotation;
            //  Rigidbody settings
            m_rigidbody = GetComponent<Rigidbody>();
            m_gravity = Physics.gravity;

            //  Collider settings
            m_collider = GetComponent<CapsuleCollider>();
            if (m_collider == null) {
                m_collider = gameObject.AddComponent<CapsuleCollider>();
                m_collider.radius = 0.36f;
                m_collider.height = MathUtil.Round(gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.y * 2);
                m_collider.center = new Vector3(0, m_collider.height / 2, 0);
            }


            //  Collision settings
            m_layerManager = GetComponent<LayerManager>();
            m_collisionsLayerMask = m_layerManager.SolidLayers;
            m_probedColliders = new Collider[m_maxCollisionCount];


            m_motionPath = new Trajectory(1, m_sampleRate, new AffineTransform(m_transform));
            m_physicsMaterial = new PhysicMaterial() { name = "Character Physics Material" };

            //  --------------------------
            //  Init
            //  --------------------------
            InitializeVariables();



            //  --------------------------
            //  Initialize actions;
            //  --------------------------
            m_actionInfo = new Dictionary<CharacterAction, int>();
            for (int i = 0; i < m_actions.Length; i++)
            {
                if (!m_actionInfo.ContainsKey(m_actions[i])) m_actionInfo.Add(m_actions[i], i);
                else m_actionInfo[m_actions[i]] = i;

                m_actions[i].Initialize(this, Array.IndexOf(m_actions, m_actions[i]), Time.deltaTime);
            }

            //  --------------------------
            //  Initialize events;
            //  --------------------------
            //EventHandler.RegisterEvent<CharacterAction, bool>(m_gameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            //EventHandler.RegisterEvent<ItemAction, bool>(m_gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
            //EventHandler.RegisterEvent<bool>(m_gameObject, EventIDs.OnAimActionStart, OnAimActionStart);

            //  --------------------------
            //  Time manager
            //  --------------------------
            m_deltaTime = Time.deltaTime;
            m_lastFrameTime = Time.realtimeSinceStartup;
            m_lastFixedUpdateTime = new float[2];
            //  --------------------------


            //  --------------------------
            //  Initialize debugger;
            //  --------------------------
            Debugger.Initialize(this);
        }


        protected void OnDestroy()
		{
            //EventHandler.UnregisterEvent<CharacterAction, bool>(m_gameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            //EventHandler.UnregisterEvent<ItemAction, bool>(m_gameObject, EventIDs.OnItemActionActive, OnItemActionActive);
            //EventHandler.UnregisterEvent<bool>(m_gameObject, EventIDs.OnAimActionStart, OnAimActionStart);
        }


        private void OnValidate()
        {
            if(m_actions != null){
                for (int i = 0; i < m_actions.Length; i++)
                    if(m_actions[i] != null) m_actions[i].SetPriority(i);
            }
        }

        private static float m_interpolationFactor;
        public static float interpolateFactor { get { return m_interpolationFactor; } }

        private float m_lastFrameTime;
        private float[] m_lastFixedUpdateTime = new float[2];
		private float m_newDeltaTime;
        private int m_newTimeIndex;
        private int m_oldTimeIndex { get { return m_newTimeIndex == 0 ? 1 : 0; } }



        private void FixedUpdate()
		{
            //  --------------------------
            //  Time manager
            //  --------------------------
            m_newDeltaTime = Time.realtimeSinceStartup - m_lastFrameTime;
            m_lastFrameTime = Time.realtimeSinceStartup;
            DebugUI.Log(this, "[F] newDeltaTime", m_newDeltaTime, RichTextColor.Lime);
            //  --------------------------


            if (!m_rigidbody.isKinematic) {
                OnUpdate(m_deltaTime);
                //Set frameUpdated.
                m_frameUpdated = true;
            }

        }

        private void Update()
        {
            float newerTime = m_lastFixedUpdateTime[m_newTimeIndex];
            float olderTime = m_lastFixedUpdateTime[m_oldTimeIndex];

            if (newerTime != olderTime)
                m_interpolationFactor = (Time.realtimeSinceStartup - newerTime) / (newerTime - olderTime);
            else m_interpolationFactor = 1;


            if (m_frameUpdated) return;

            //  --------------------------
            //  Time manager
            //  --------------------------
            m_newDeltaTime = Time.realtimeSinceStartup - m_lastFrameTime;
            m_lastFrameTime = Time.realtimeSinceStartup;
            DebugUI.Log(this, "[U] newDeltaTime", m_newDeltaTime, RichTextColor.Magenta);
            //  --------------------------


            //KinematicMove();

            OnUpdate(m_deltaTime);
            m_frameUpdated = true;
        }



        private void LateUpdate()
        {
            DebugAttributes();
            DebugUI.Log(this, "DeltaTime", m_deltaTime);
            DebugUI.Log(this, "DeltaTime", m_deltaTime);
            //  -------------------
            //  Log debug messages.
            DebugUI.Log(this, "interpolateFactor", interpolateFactor, RichTextColor.Lime);
            //  -------------------
            //  Continue with updates.
            if (m_frameUpdated) {
                m_frameUpdated = false;
                return;
            }
        }


        private void OnAnimatorMove()
        {
            AnimatorMove();
        }


        private void OnAnimatorIK(int layerIndex)
        {
            leftFootPosition = GetFootPosition(AvatarIKGoal.LeftFoot);
            rightFootPosition = GetFootPosition(AvatarIKGoal.RightFoot);
        }






        #region Character Locomotion


        private void OnUpdate(float deltaTime)
        {
            m_deltaTime = deltaTime;


            for (int i = 0; i < m_actions.Length; i++)
            {
                //  Move to next action if action componenet is disabled.
                if (!m_actions[i].enabled) continue;


                CharacterAction action = m_actions[i];
                if(action.IsActive) {
                    if (action.StopType != ActionStopType.Manual) {
                        TryStopAction(action);

                    }
                        
                }
                else {
                    if (action.StartType != ActionStartType.Manual)
                    {
                        bool actionStarted = TryStartAction(action);
                        if (actionStarted) {
                            if (action.IsConcurrentAction()) {
                                continue;
                            }
                            m_activeAction = action;
                            break;
                        }
                        
                    }
                }
            }



            InternalMove();

            if (m_activeAction != null) {
                if (m_activeAction.Move()) Move();
            }
            else Move();

            if (m_activeAction != null) {
                if (m_activeAction.CheckGround()) CheckGround();
            }
            else CheckGround();

            if (m_activeAction != null) {
                if (m_activeAction.CheckMovement()) CheckMovement();
            }
            else CheckMovement();

            if (m_activeAction != null) {
                if (m_activeAction.SetPhysicsMaterial()) SetPhysicsMaterial();
            }
            else SetPhysicsMaterial();

            if (m_activeAction != null) {
                if (m_activeAction.UpdateMovement()) UpdateMovement();
            }
            else UpdateMovement();

            if (m_activeAction != null) {
                if (m_activeAction.UpdateRotation()) UpdateRotation();
            }
            else UpdateRotation();

            if (m_activeAction != null) {
                if (m_activeAction.UpdateAnimator()) UpdateAnimator();
            }
            else UpdateAnimator();


            ApplyRotation();
            //  Execute al the updates.
            ApplyMovement();






        }

        //  Float value to indicate which leg is moving forward.  Right leg is 0, Left leg is 1.  (opposite of pivotWeight)
        float m_legIndex = 0.5f;

        /// <summary>
        /// Updates the animator.
        /// </summary>
        protected override void UpdateAnimator()
        {

            //if(!m_moving && m_animatorUpdateTimer > 0.5f)
            //{

            //    //var angle = Mathf.Atan2(m_lookDirection.x, m_lookDirection.z) * Mathf.Rad2Deg;
            //    //if (Mathf.Abs(angle) < 0.1f)
            //    //    angle = 0;
            //    //m_animator.SetFloat(HashID.LookAngle, angle);

            //    m_animatorUpdateTimer = 0;
            //}
            //m_animatorUpdateTimer += m_deltaTime;

            float forwardLeg = 0.5f;
            if (m_moving)
                forwardLeg = leftFootPosition.normalized.z > rightFootPosition.normalized.z ? 0 : 1;
            m_legIndex = Mathf.Lerp(m_legIndex, forwardLeg, m_deltaTime * 8);
            m_animator.SetFloat(HashID.LegFwdIndex, m_legIndex);
            base.UpdateAnimator();
        }







        #endregion








        #region Character Actions


        public T GetAction<T>() where T : CharacterAction
        {
            for (int i = 0; i < m_actions.Length; i++){
                if (m_actions[i] is T){
                    return (T)m_actions[i];
                }
            }
            return null;
        }


        public bool TryStartAction(CharacterAction action)
        {
            if (action == null) return false;

            int index = Array.IndexOf(m_actions, action);
            //  If there is an active action and current action is non concurrent.
            if (m_activeAction != null && !action.IsConcurrentAction())
            {
                int activeActionIndex = Array.IndexOf(m_actions, m_activeAction);
                //Debug.LogFormat("Action index {0} | Active Action index {1}", index, activeActionIndex);
                if (index < activeActionIndex)
                {
                    if (action.CanStartAction())
                    {
                        //  Stop the current active action.
                        TryStopAction(m_activeAction);
                        //  Set the active action.
                        m_activeAction = m_actions[index];
                        //m_activeAction[index] = m_actions[index];
                        action.StartAction();
                        //action.UpdateAnimator();
                        return true;
                    }
                }
            }
            //  If there is an active action and current action is concurrent.
            else if (m_activeAction != null && action.IsConcurrentAction())
            {
                if (action.CanStartAction())
                {
                    //m_activeAction[index] = m_actions[index];
                    action.StartAction();
                    //action.UpdateAnimator();
                    return true;
                }
            }
            //  If there is no active action.
            else if (m_activeAction == null)
            {
                if (action.CanStartAction())
                {
                    m_activeAction = m_actions[index];
                    //m_activeAction[index] = m_actions[index];
                    action.StartAction();
                    //action.UpdateAnimator();
                    return true;
                }
            }
            else {

                
            }

            return false;
        }


        public void TryStopAllActions()
        {
            for (int i = 0; i < m_actions.Length; i++)
            {
                if (m_actions[i].IsActive)
                {
                    TryStopAction(m_actions[i]);
                }
            }
        }


        public void TryStopAction(CharacterAction action)
        {
            if (action == null) return;

            if (action.CanStopAction())
            {
                int index = Array.IndexOf(m_actions, action);
                if (m_activeAction == action)
                    m_activeAction = null;


                action.StopAction();
                ActionStopped();
            }
        }


        public void TryStopAction(CharacterAction action, bool force)
        {
            if (action == null) return;
            if (force)
            {
                //int index = Array.IndexOf(m_actions, action);
                if (m_activeAction == action)
                    m_activeAction = null;
                action.StopAction();
                ActionStopped();
                return;
            }

            TryStopAction(action);
        }

        #endregion



        #region Event Actions


        protected void OnActionActive(CharacterAction action, bool activated)
        {
            int index = Array.IndexOf(m_actions, action);
            if (action == m_actions[index])
            {
                if (m_actions[index].enabled)
                {
                    if(activated)
                    {
                        //Debug.LogFormat(" {0} is starting.", action.GetType().Name);

                    }
                    else
                    {

                    }
                }
            }

        }


        private void OnItemActionActive( ItemAction action, bool activated )
        {

        }


        protected void OnAimActionStart( bool aim )
        {
            Aiming = aim;
            m_MovementType = Aiming ? MovementTypes.Combat : MovementTypes.Adventure;
        }


        protected void OnImpactHit()
        {

        }

        protected void ActionStopped()
        {

        }

        #endregion



        public int GetActionPriority(CharacterAction action)
        {
            if (!m_actionInfo.TryGetValue(action, out int index)) index = -1;
            return index;
        }


        public void SetMovementType( MovementTypes movementType )
        {
            m_MovementType = movementType;
        }






        private Vector3 GetFootPosition(AvatarIKGoal foot, bool worldPos = false)
        {
            if (foot == AvatarIKGoal.LeftFoot || foot == AvatarIKGoal.RightFoot)
            {
                Vector3 footPos = m_animator.GetIKPosition(foot);
                Quaternion footRot = m_animator.GetIKRotation(foot);
                float botFootHeight = foot == AvatarIKGoal.LeftFoot ? m_animator.leftFeetBottomHeight : m_animator.rightFeetBottomHeight;
                Vector3 footHeight = new Vector3(0, -botFootHeight, 0);


                footPos += footRot * footHeight;

                return !worldPos ? footPos : m_transform.InverseTransformPoint(footPos);
            }

            return Vector3.zero;
        }

        private Vector3 GetFootPosition(HumanBodyBones foot, bool worldPos = false)
        {
            int side = 0;
            if (foot == HumanBodyBones.LeftFoot || foot == HumanBodyBones.LeftToes) side = -1;
            else if (foot == HumanBodyBones.RightFoot || foot == HumanBodyBones.RightToes) side = 1;

            if (side == -1 || side == 1)
            {
                foot = side == -1 ? HumanBodyBones.LeftToes : HumanBodyBones.RightToes;
                Vector3 footPos = m_animator.GetBoneTransform(foot).localPosition;
                Quaternion footRot = m_animator.GetBoneTransform(foot).localRotation;
                //var rot = Quaternion.
                float botFootHeight = side == -1 ? m_animator.leftFeetBottomHeight : m_animator.rightFeetBottomHeight;
                Vector3 footHeight = new Vector3(0,side * -botFootHeight, 0);

                if (side == -1) footPos = Quaternion.Euler(0, 0, 180) * footPos;
                footPos = footRot * footPos + footHeight;
                //footPos += footRot * footHeight;

                

                return worldPos ? m_animator.GetBoneTransform(foot).TransformPoint(footPos) : footPos;
            }

            return Vector3.zero;
        }




        //------


        protected override void DebugAttributes()
        {
            base.DebugAttributes();

            var lf = m_transform.position - GetFootPosition(HumanBodyBones.LeftFoot, true);
            lf = m_animator.GetBoneTransform(HumanBodyBones.LeftFoot).localPosition;
            DebugUI.Log(this, m_animator.GetBoneTransform(HumanBodyBones.LeftFoot).name, lf, RichTextColor.Orange);
            var rf = m_transform.position - GetFootPosition(HumanBodyBones.RightFoot, true);
            rf = m_animator.GetBoneTransform(HumanBodyBones.RightFoot).localPosition;
            DebugUI.Log(this, m_animator.GetBoneTransform(HumanBodyBones.RightFoot).name, rf, RichTextColor.Orange);

        }



        protected override void DrawGizmos()
        {


        }




    }
}


//protected bool canCheckGround;
//protected bool canCheckMovement;
//protected bool canSetPhysicsMaterial;
//protected bool canUpdateRotation;
//protected bool canUpdateMovement;
//protected bool canUpdateAnimator;
//protected bool canMove;


//private void ActionUpdate()
//{
//    timeScale = Time.timeScale;
//    if (Math.Abs(timeScale) < float.Epsilon) return;
//    m_deltaTime = deltaTime;


//    canMove = true;

//    ////  Start Stop Actions.
//    for (int i = 0; i < m_actions.Length; i++)
//    {
//        if (m_actions[i].enabled == false)
//            continue;
//        CharacterAction charAction = m_actions[i];
//        //  If action was started, move onto next action.
//        StopStartAction(charAction);
//        //if (StopStartAction(charAction)) continue;

//        if (charAction.IsActive)
//        {
//            //  Move charatcer based on input values.
//            if (canMove) canMove = charAction.Move();

//            //// Update the Animator.
//            //if (m_UpdateAnimator) m_UpdateAnimator = charAction.UpdateAnimator();
//        }
//        //  Call Action Update.
//        charAction.UpdateAction();
//    }

//    //  Moves the character according to the input.
//    InternalMove();
//    if (canMove) Move();
//}

//private void ActionFixedUpdate()
//{
//    timeScale = Time.timeScale;
//    if (Math.Abs(timeScale) < float.Epsilon) return;
//    m_deltaTime = deltaTime;


//    //canMove = true;

//    canCheckGround = true;
//    canCheckMovement = true;
//    canSetPhysicsMaterial = true;
//    canUpdateRotation = true;
//    canUpdateMovement = true;
//    canUpdateAnimator = true;


//    for (int i = 0; i < m_actions.Length; i++)
//    {
//        if (m_actions[i].enabled == false)
//        {
//            //  Call Action Update.
//            m_actions[i].UpdateAction();
//            continue;
//        }
//        CharacterAction charAction = m_actions[i];
//        if (charAction.IsActive)
//        {
//            ////  Move charatcer based on input values.
//            //if (canMove) canMove = charAction.Move();

//            //  Perform checks to determine if the character is on the ground.
//            if (canCheckGround) canCheckGround = charAction.CheckGround();
//            //  Ensure the current movement direction is valid.
//            if (canCheckMovement) canCheckMovement = charAction.CheckMovement();
//            //  Apply any movement.
//            if (canUpdateMovement) canUpdateMovement = charAction.UpdateMovement();
//            //  Update the rotation forces.
//            if (canUpdateRotation) canUpdateRotation = charAction.UpdateRotation();

//            // Update the Animator.
//            if (canUpdateAnimator) canUpdateAnimator = charAction.UpdateAnimator();

//        }
//        //  Call Action Update.
//        charAction.UpdateAction();
//    }  //  end of for loop


//    ////  Moves the character according to the input.
//    //if (canMove) Move();


//    //  Perform checks to determine if the character is on the ground.
//    if (canCheckGround) CheckGround();
//    //  Ensure the current movement direction is valid.
//    if (canCheckMovement) CheckMovement();
//    //  Set the physic material based on the grounded and stepping state
//    if (canSetPhysicsMaterial) SetPhysicsMaterial();

//    //  Apply any movement.
//    if (canUpdateMovement) UpdateMovement();
//    //  Update the rotation forces.
//    if (canUpdateRotation) UpdateRotation();

//    // Update the Animator.
//    if (canUpdateAnimator) UpdateAnimator();


//    ApplyMovement();
//    ApplyRotation();
//}





//public bool TryStartAction(CharacterAction action)
//{
//    if (action == null) return false;

//    int actionPriority = Array.IndexOf(m_actions, action);
//    //  If there is an active action and current action is non concurrent.
//    if (m_activeAction != null && action.IsConcurrentAction() == false)
//    {
//        int activeActionPriority = Array.IndexOf(m_actions, m_activeAction);
//        //Debug.LogFormat("Action index {0} | Active Action index {1}", index, activeActionIndex);
//        if (actionPriority < activeActionPriority)
//        {
//            if (action.CanStartAction())
//            {
//                //  Stop the current active action.
//                TryStopAction(m_activeAction, true);
//                //  Set the active action.
//                m_activeAction = action;
//                action.StartAction();
//                return true;
//            }
//        }
//    }
//    //  If there is an active action and current action is concurrent.
//    else if (m_activeAction != null && action.IsConcurrentAction())
//    {
//        if (action.CanStartAction())
//        {
//            //m_activeAction[index] = m_actions[index];
//            action.StartAction();
//            //action.UpdateAnimator();
//            return true;
//        }
//    }
//    //  If there is no active action.
//    else
//    {
//        if (action.CanStartAction())
//        {
//            m_activeAction = m_actions[actionPriority];
//            //m_activeAction[index] = m_actions[index];
//            action.StartAction();
//            //action.UpdateAnimator();
//            return true;
//        }
//    }


//    return false;
//}


//public void TryStopAction(CharacterAction action, bool force)
//{
//    if (force)
//    {
//        if (action == null || !action.IsActive || !action.enabled) return;

//        if (action.enabled && action.IsActive)
//        {
//            action.StopAction();
//            ActionStopped();
//        }

//    }
//    else
//    {
//        TryStopAction(action);
//    }






//}


//public void TryStopAction(CharacterAction action)
//{
//    if (action == null || !action.IsActive || !action.enabled) return;

//    if (action.enabled && action.IsActive)
//    {
//        if (action.CanStopAction())
//        {
//            action.StopAction();
//            if (m_activeAction == action)
//                m_activeAction = null;
//            ActionStopped();
//            return;
//        }
//    }
//}






///// <summary>
///// Starts and Stops the Action internally.
///// </summary>
///// <param name="charAction"></param>
///// <returns>Returns true if action was started.</returns>
//protected void StopStartAction(CharacterAction charAction)
//{
//    //  First, check if current Action can Start or Stop.

//    //  Current Action is Active.
//    if (charAction.enabled && charAction.IsActive)
//    {
//        //  Check if can stop Action is StopType is NOT Manual.
//        if (charAction.StopType != ActionStopType.Manual)
//        {
//            if (charAction.CanStopAction())
//            {
//                //  Start the Action and update the animator.
//                charAction.StopAction();
//                //  Reset Active Action.
//                if (m_activeAction = charAction)
//                    m_activeAction = null;
//                //  Move on to the next Action.
//                return;
//            }
//        }
//    }
//    //  Current Action is NOT Active.
//    else
//    {
//        //  Check if can start Action is StartType is NOT Manual.
//        if (charAction.enabled && charAction.StartType != ActionStartType.Manual)
//        {
//            if (m_activeAction == null)
//            {
//                if (charAction.CanStartAction())
//                {
//                    //  Start the Action and update the animator.
//                    charAction.StartAction();
//                    //charAction.UpdateAnimator();
//                    //  Set active Action if not concurrent.
//                    if (charAction.IsConcurrentAction() == false)
//                        m_activeAction = charAction;
//                    //  Move onto the next Action.
//                    return;
//                }
//            }
//            else if (charAction.IsConcurrentAction())
//            {
//                if (charAction.CanStartAction())
//                {
//                    //  Start the Action and update the animator.
//                    charAction.StartAction();
//                    //charAction.UpdateAnimator();
//                    //  Move onto the next Action.
//                    return;
//                }
//            }
//            else
//            {

//            }
//        }
//    }

//    return;
//}