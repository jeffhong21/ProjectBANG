namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;

    using DebugUI;


    public class CharacterLocomotion : RigidbodyCharacterController
    {
        

        public event Action<bool> OnAim = delegate {};


        //  --  Character Actions
        [SerializeField, HideInInspector]
        protected CharacterAction[] m_Actions;
        [SerializeField, HideInInspector]
        protected CharacterAction m_ActiveAction;

        protected bool canCheckGround;
        protected bool canCheckMovement;
        protected bool canSetPhysicsMaterial;
        protected bool canUpdateRotation;
        protected bool canUpdateMovement;
        protected bool canUpdateAnimator;
        protected bool canMove;



        private Vector3 leftFootPosition, rightFootPosition;



        #region Properties

        public bool Aiming { get; set; }

        public bool CanAim { get => Grounded; }


        public CharacterAction[] CharActions
        {
            get { return m_Actions; }
            set { m_Actions = value; }
        }

        #endregion







        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < m_Actions.Length; i++) {
                m_Actions[i].Initialize(this, fixedDeltaTime);
            }

            EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            EventHandler.RegisterEvent<ItemAction, bool>(m_GameObject, EventIDs.OnItemActionActive, OnItemActionActive);
            EventHandler.RegisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);

        }


        protected void OnDestroy()
		{
            EventHandler.UnregisterEvent<CharacterAction, bool>(m_GameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            EventHandler.UnregisterEvent<ItemAction, bool>(m_GameObject, EventIDs.OnItemActionActive, OnItemActionActive);
            EventHandler.UnregisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);


        }




        protected override void Update()
        {
            base.Update();



            canMove = true;

            ////  Start Stop Actions.
            for (int i = 0; i < m_Actions.Length; i++) {
                if (m_Actions[i].enabled == false)
                    continue;
                CharacterAction charAction = m_Actions[i];
                //  If action was started, move onto next action.
                StopStartAction(charAction);
                //if (StopStartAction(charAction)) continue;

                if (charAction.IsActive) {
                    //  Move charatcer based on input values.
                    if (canMove) canMove = charAction.Move();

                    //// Update the Animator.
                    //if (m_UpdateAnimator) m_UpdateAnimator = charAction.UpdateAnimator();
                }
                //  Call Action Update.
                charAction.UpdateAction();
            }

            //  Moves the character according to the input.
            if (canMove) Move();
        }


        protected override void FixedUpdate()
		{
            base.FixedUpdate();

            //canMove = true;

            canCheckGround = true;
            canCheckMovement = true;
            canSetPhysicsMaterial = true;
            canUpdateRotation = true;
            canUpdateMovement = true;
            canUpdateAnimator = true;


            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].enabled == false) {
                    //  Call Action Update.
                    m_Actions[i].UpdateAction();
                    continue;
                }
                CharacterAction charAction = m_Actions[i];
                if (charAction.IsActive)
                {
                    ////  Move charatcer based on input values.
                    //if (canMove) canMove = charAction.Move();

                    //  Perform checks to determine if the character is on the ground.
                    if (canCheckGround) canCheckGround = charAction.CheckGround();
                    //  Ensure the current movement direction is valid.
                    if (canCheckMovement) canCheckMovement = charAction.CheckMovement();
                    //  Apply any movement.
                    if (canUpdateMovement) canUpdateMovement = charAction.UpdateMovement();
                    //  Update the rotation forces.
                    if (canUpdateRotation) canUpdateRotation = charAction.UpdateRotation();

                    // Update the Animator.
                    if (canUpdateAnimator) canUpdateAnimator = charAction.UpdateAnimator();

                }
                //  Call Action Update.
                charAction.UpdateAction();
            }  //  end of for loop


            ////  Moves the character according to the input.
            //if (canMove) Move();


            //  Perform checks to determine if the character is on the ground.
            if (canCheckGround) CheckGround();
            //  Ensure the current movement direction is valid.
            if (canCheckMovement) CheckMovement();
            //  Set the physic material based on the grounded and stepping state
            if (canSetPhysicsMaterial) SetPhysicsMaterial();

            //  Apply any movement.
            if (canUpdateMovement) UpdateMovement();
            //  Update the rotation forces.
            if (canUpdateRotation) UpdateRotation();

            // Update the Animator.
            if (canUpdateAnimator) UpdateAnimator();


            ApplyMovement();
            ApplyRotation();
        }


        protected override void LateUpdate()
        {
            base.LateUpdate();

            //DebugDraw.Circle(m_animator.pivotPosition, transform.up, 0.1f, Color.yellow);
        }


        private void OnAnimatorIK(int layerIndex)
        {
            leftFootPosition = GetFootPosition(AvatarIKGoal.LeftFoot);
            rightFootPosition = GetFootPosition(AvatarIKGoal.RightFoot);
        }


        #region Character Locomotion


        /// <summary>
        /// Move charatcer based on input values.
        /// </summary>
        protected override void Move()
        {
            base.Move();
        }


        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected override void CheckGround()
        {
            base.CheckGround();
        }


        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected override void CheckMovement()
        {
            base.CheckMovement();
        }


        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected override void UpdateRotation()
        {
            base.UpdateRotation();
        }


        /// <summary>
        /// Apply any movement.
        /// </summary>
        protected override void UpdateMovement()
        {
            base.UpdateMovement();
        }


        /// <summary>
        /// Updates the animator.
        /// </summary>
        protected override void UpdateAnimator()
        {

            if (Moving)
            {
                float legFwdIndex = leftFootPosition.normalized.z > rightFootPosition.normalized.z ? 0 : 1;
                m_animator.SetFloat(HashID.LegFwdIndex, legFwdIndex);
            }
            else
            {
                m_animator.SetFloat(HashID.LegFwdIndex, 0.5f);
            }

            base.UpdateAnimator();
        }




        /// <summary>
        /// Set the collider's physics material.
        /// </summary>
        protected override void SetPhysicsMaterial()
        {
            base.SetPhysicsMaterial();
        }


        protected override void ApplyMovement()
        {
            base.ApplyMovement();
        }


        protected override void ApplyRotation()
        {
            base.ApplyRotation();



        }


        #endregion




        public void Move(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            throw new NotImplementedException();
        }




        #region Character Actions


        /// <summary>
        /// Try to stop the characterAction.
        /// </summary>
        /// <param name="charAction"></param>
        /// <returns></returns>
        protected bool StopAction(CharacterAction charAction)
        {
            //  Current Action is Active.
            if (charAction.enabled && charAction.IsActive) {
                //  Check if can stop Action is StopType is NOT Manual.
                if (charAction.StopType != ActionStopType.Manual) {
                    if (charAction.CanStopAction()) {
                        //  Start the Action and update the animator.
                        charAction.StopAction();
                        //  Reset Active Action.
                        if (m_ActiveAction = charAction)
                            m_ActiveAction = null;
                        //  Move on to the next Action.
                        return true;
                    }
                }
            }

            return false;
        }



        /// <summary>
        /// Starts and Stops the Action internally.
        /// </summary>
        /// <param name="charAction"></param>
        /// <returns>Returns true if action was started.</returns>
        protected void StopStartAction(CharacterAction charAction)
        {
            //  First, check if current Action can Start or Stop.

            //  Current Action is Active.
            if (charAction.enabled && charAction.IsActive)
            {
                //  Check if can stop Action is StopType is NOT Manual.
                if (charAction.StopType != ActionStopType.Manual)
                {
                    if (charAction.CanStopAction())
                    {
                        //  Start the Action and update the animator.
                        charAction.StopAction();
                        //  Reset Active Action.
                        if (m_ActiveAction = charAction)
                            m_ActiveAction = null;
                        //  Move on to the next Action.
                        return;
                    }
                }
            }
            //  Current Action is NOT Active.
            else
            {
                //  Check if can start Action is StartType is NOT Manual.
                if (charAction.enabled && charAction.StartType != ActionStartType.Manual)
                {
                    if (m_ActiveAction == null)
                    {
                        if (charAction.CanStartAction())
                        {
                            //  Start the Action and update the animator.
                            charAction.StartAction();
                            //charAction.UpdateAnimator();
                            //  Set active Action if not concurrent.
                            if (charAction.IsConcurrentAction() == false)
                                m_ActiveAction = charAction;
                            //  Move onto the next Action.
                            return;
                        }
                    }
                    else if (charAction.IsConcurrentAction())
                    {
                        if (charAction.CanStartAction())
                        {
                            //  Start the Action and update the animator.
                            charAction.StartAction();
                            //charAction.UpdateAnimator();
                            //  Move onto the next Action.
                            return;
                        }
                    }
                    else
                    {

                    }
                }
            }

            return;
        }


        public T GetAction<T>() where T : CharacterAction
        {
            for (int i = 0; i < m_Actions.Length; i++){
                if (m_Actions[i] is T){
                    return (T)m_Actions[i];
                }
            }
            return null;
        }


        public bool TryStartAction(CharacterAction action)
        {
            if (action == null) return false;

            int index = Array.IndexOf(m_Actions, action);
            //  If there is an active action and current action is non concurrent.
            if(m_ActiveAction != null && action.IsConcurrentAction() == false){
                int activeActionIndex = Array.IndexOf(m_Actions, m_ActiveAction);
                //Debug.LogFormat("Action index {0} | Active Action index {1}", index, activeActionIndex);
                if(index < activeActionIndex){
                    if (action.CanStartAction()){
                        //  Stop the current active action.
                        TryStopAction(m_ActiveAction);
                        //  Set the active action.
                        m_ActiveAction = m_Actions[index];
                        //m_ActiveActions[index] = m_Actions[index];
                        action.StartAction();
                        //action.UpdateAnimator();
                        return true;
                    }
                } 
            }
            //  If there is an active action and current action is concurrent.
            else if (m_ActiveAction != null && action.IsConcurrentAction())
            {
                if (action.CanStartAction()){
                    //m_ActiveActions[index] = m_Actions[index];
                    action.StartAction();
                    //action.UpdateAnimator();
                    return true;
                }
            }
            //  If there is no active action.
            else if (m_ActiveAction == null){
                if (action.CanStartAction())
                {
                    m_ActiveAction = m_Actions[index];
                    //m_ActiveActions[index] = m_Actions[index];
                    action.StartAction();
                    //action.UpdateAnimator();
                    return true;
                }
            }


            return false;
        }


        public void TryStopAllActions()
        {
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].IsActive)
                {
                    TryStopAction(m_Actions[i]);
                }
            }
        }


        public void TryStopAction(CharacterAction action)
        {
            if (action == null) return; 

            if (action.CanStopAction()){
                int index = Array.IndexOf(m_Actions, action);
                if (m_ActiveAction == action)
                    m_ActiveAction = null;


                action.StopAction();
                ActionStopped();
            }
        }


        public void TryStopAction(CharacterAction action, bool force)
        {
            if (action == null) return; 
            if(force){
                int index = Array.IndexOf(m_Actions, action);
                if (m_ActiveAction == action)
                    m_ActiveAction = null;


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
            int index = Array.IndexOf(m_Actions, action);
            if (action == m_Actions[index])
            {
                if (m_Actions[index].enabled)
                {
                    if(activated)
                    {
                        //Debug.LogFormat(" {0} is starting.", action.GetType().Name);
                        CharacterDebug.Log(action.GetType().Name, action.GetType());

                    }
                    else
                    {
                        CharacterDebug.Remove(action.GetType().Name);
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


        protected void ActionStopped()
        {

        }

        #endregion





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

            var lf = m_transform.position - GetFootPosition(HumanBodyBones.LeftToes, true);
            DebugUI.Log(this, m_animator.GetBoneTransform(HumanBodyBones.LeftToes).name, lf, RichTextColor.Orange);
            var rf = m_transform.position - GetFootPosition(HumanBodyBones.RightToes, true);
            DebugUI.Log(this, m_animator.GetBoneTransform(HumanBodyBones.RightToes).name, rf, RichTextColor.Orange);

        }



        protected override void DrawGizmos()
        {


        }




    }
}
