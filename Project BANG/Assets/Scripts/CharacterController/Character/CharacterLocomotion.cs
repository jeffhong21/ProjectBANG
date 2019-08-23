namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Text;
    using MathUtil = MathUtilities;

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



        #region Properties

        public bool Aiming { get; set; }

        public bool CanAim { get => Grounded; }

        public Vector3 CenterOfMass { get { return m_Animator.bodyPosition; } }

        public Vector3 BalanceVector {  get { return m_Animator.bodyPosition - m_Transform.position; } }

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
            if (canUpdateAnimator)
                UpdateAnimator();
            //if (m_Animator.pivotWeight < 0.5f || m_Animator.pivotWeight > 0.5f)
            //    m_Animator.SetFloat(HashID.LegUpIndex, m_Animator.pivotWeight >= 0.5f ? 1 : 0);
            //else m_Animator.SetFloat(HashID.LegUpIndex, 0.5f + 1);


        }


        protected override void LateUpdate()
        {
            base.LateUpdate();

            //for (int i = 0; i < colliderBuffer.Length; i++) {
            //    colliderBuffer[i] = null;
            //}
            GetPivotPosition();

            //DebugDraw.Circle(m_Animator.pivotPosition, transform.up, 0.1f, Color.yellow);
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
            //m_Animator.SetFloat(HashID.LegUpIndex, )

            Vector3 leftFootPos = GetFootPosition(AvatarIKGoal.LeftFoot);
            Vector3 rightFootPos = GetFootPosition(AvatarIKGoal.RightFoot);

            float legFwdIndex = leftFootPos.normalized.z > rightFootPos.normalized.z ? 0 : 1;
            m_Animator.SetFloat(HashID.LegFwdIndex, legFwdIndex);
            base.UpdateAnimator();
        }


        /// <summary>
        /// Anything that should be done in the OnAnimatorMove function.
        /// </summary>
        protected override void AnimatorMove()
        {
            base.AnimatorMove();

            if (m_Animator.isHuman)
            {

            }
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




        public void Move( float horizontalMovement, float forwardMovement, Quaternion lookRotation )
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


        /// <summary>
        /// Returns which foot is planted.  0 is left, 1 is right.
        /// </summary>
        /// <param name="pivotWeightThreshold"></param>
        /// <returns>Returns -1 if neither foot is planted.</returns>
        public int GetPlantedPivotFoot( float pivotWeightThreshold = 0.5f)
        {
            pivotWeightThreshold = Mathf.Clamp(pivotWeightThreshold, 0, 0.5f);
            int pivotFoot = 2;
            if (m_Animator.pivotWeight >= 1 - pivotWeightThreshold) {
                pivotFoot = 1;
            } else if (m_Animator.pivotWeight < 0 + pivotWeightThreshold) {
                pivotFoot = 0;
            } else
                pivotFoot = -1;

            return pivotFoot;
        }


        public void SetMovementType( MovementTypes movementType )
        {
            m_MovementType = movementType;
        }



        private Vector3 GetPivotPosition()
        {
            m_Animator.stabilizeFeet = true;

            Vector3 pivotPosition = m_Animator.pivotPosition;

            Vector3 leftFootPos = GetFootPosition(AvatarIKGoal.LeftFoot);
            Vector3 rightFootPos = GetFootPosition(AvatarIKGoal.RightFoot);
            float leftHeight = MathUtil.Round(Mathf.Abs(leftFootPos.y));
            float rightHeight = MathUtil.Round(Mathf.Abs(rightFootPos.y));

            float threshold = 0.1f;
            float pivotDifference = Mathf.Abs(0.5f - m_Animator.pivotWeight);
            float t = Mathf.Clamp(pivotDifference, 0, 0.5f) / 0.5f;
            //  1 means feet are not pivot.
            float feetPivotActive = 1;  

            //  Both feet are grouned.
            if( (leftHeight < threshold && rightHeight < threshold) || (leftHeight > threshold && rightHeight > threshold ) && pivotDifference < 5f){
                t = Time.deltaTime;
                feetPivotActive = MathF.Clamp01(feetPivotActive + t);
                pivotPosition = m_Transform.position;  
            }
            //  If one leg is raised and one is planted.
            else if ((leftHeight < tinyOffset && rightHeight > 0) || rightHeight < tinyOffset && leftHeight > 0)
            {
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                feetPivotActive = Mathf.Lerp(0f, 1f, t);

                m_Animator.feetPivotActive = feetPivotActive;
                pivotPosition = m_Animator.pivotPosition;
            }

            
            pivotPosition.y = m_Transform.position.y;


            CharacterDebug.Log("<color=blue>* FeetPivotWeight *</color>", MathUtil.Round(feetPivotActive));
            return pivotPosition;
        }


        private Vector3 GetFootPosition(AvatarIKGoal foot, bool worldPos = false)
        {
            if (foot == AvatarIKGoal.LeftFoot || foot == AvatarIKGoal.RightFoot)
            {
                Vector3 footPos = m_Animator.GetIKPosition(foot);
                Quaternion footRot = m_Animator.GetIKRotation(foot);
                float botFootHeight = foot == AvatarIKGoal.LeftFoot ? m_Animator.leftFeetBottomHeight : m_Animator.rightFeetBottomHeight;
                Vector3 footHeight = new Vector3(0, -botFootHeight, 0);


                footPos += footRot * footHeight;

                return !worldPos ? footPos : m_Transform.InverseTransformPoint(footPos);
            }

            return Vector3.zero;
        }





        //------


        protected override void DebugAttributes()
        {
            base.DebugAttributes();


            CharacterDebug.Log("seperator", "----------");
            //CharacterDebug.Log("<color=blue>* FeetPivotWeight *</color>", MathUtil.Round(feetPivotActive));
            CharacterDebug.Log("<color=cyan>* PivotWeight *</color>", m_Animator.pivotWeight);
            CharacterDebug.Log("<color=cyan>* PivotPos *</color>", m_Transform.InverseTransformPoint(m_Animator.pivotPosition));

            //CharacterDebug.Log("<color=yellow> leftFootPos </color>", GetFootPosition(0, true));
            //CharacterDebug.Log("<color=yellow> rightFootPos </color>", GetFootPosition(1, true));
            //CharacterDebug.Log("<color=green> leftFootPos Y </color>", MathUtil.Round(GetFootPosition(AvatarIKGoal.LeftFoot, true).normalized.y));
            //CharacterDebug.Log("<color=green> rightFootPos Y</color>", MathUtil.Round(GetFootPosition(AvatarIKGoal.RightFoot, true).normalized.y));
            CharacterDebug.Log("<color=yellow> leftFootPos Z </color>", MathUtil.Round(GetFootPosition(AvatarIKGoal.LeftFoot).normalized.z));
            CharacterDebug.Log("<color=yellow> rightFootPos Z</color>", MathUtil.Round(GetFootPosition(AvatarIKGoal.RightFoot).normalized.z));

            CharacterDebug.Log("<color=green> leftFootPos Y </color>", MathUtil.Round(GetFootPosition(AvatarIKGoal.LeftFoot).y));
            CharacterDebug.Log("<color=green> rightFootPos Y</color>", MathUtil.Round(GetFootPosition(AvatarIKGoal.RightFoot).y));
            //CharacterDebug.Log("<color=yellow> leftFootPos Z </color>", MathUtil.Round(GetFootPosition(AvatarIKGoal.LeftFoot, true).z));
            //CharacterDebug.Log("<color=yellow> rightFootPos Z</color>", MathUtil.Round(GetFootPosition(AvatarIKGoal.RightFoot, true).z));

            CharacterDebug.Log("seperator", "----------");


        }



        protected override void DrawGizmos()
        {

            Gizmos.color = Debugger.colors.animatorColor;
            Gizmos.DrawRay(m_Transform.position, Vector3.up);
            Gizmos.DrawRay(m_Transform.position, m_Animator.bodyPosition - m_Transform.position);
            Gizmos.DrawSphere(m_Animator.bodyPosition, 0.05f);
            Gizmos.DrawSphere(GetPivotPosition(), 0.05f);


            Gizmos.color = Debugger.colors.yellow1;
            Gizmos.DrawWireSphere(GetFootPosition(AvatarIKGoal.RightFoot), 0.05f);
            //Gizmos.matrix = Matrix4x4.TRS(m_Animator.GetIKPosition(AvatarIKGoal.LeftFoot), m_Animator.GetIKRotation(AvatarIKGoal.LeftFoot), Vector3.one);
            Gizmos.DrawWireSphere(GetFootPosition(AvatarIKGoal.LeftFoot), 0.05f);
            //Gizmos.DrawWireSphere(Vector3.zero, 0.05f);


            //Gizmos.DrawWireSphere(GetFootPosition(0), 0.05f);
            //Gizmos.DrawWireSphere(GetFootPosition(1), 0.05f);
        }




    }
}
