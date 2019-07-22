namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Text;


    public class CharacterLocomotion : RigidbodyCharacterController
    {
        public event Action<bool> OnAim = delegate {};


        //  --  Character Actions
        [SerializeField, HideInInspector]
        protected CharacterAction[] m_Actions;
        [SerializeField, HideInInspector]
        protected CharacterAction m_ActiveAction;

        protected bool update_CheckGround;
        protected bool update_CheckMovement;
        protected bool update_SetPhysicsMaterial;
        protected bool update_UpdateRotation;
        protected bool update_UpdateMovement;
        protected bool update_UpdateAnimator;
        protected bool update_Move;



        public bool Aiming { get; set; }

        public bool CanAim {
            get {
                if (Grounded)
                    return true;
                return false;
            }
        }

        public CharacterAction[] CharActions{
            get { return m_Actions; }
            set { m_Actions = value; }
        }





		protected void OnEnable()
		{

            EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            EventHandler.RegisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);

        }


        protected void OnDisable()
		{

            EventHandler.UnregisterEvent<CharacterAction, bool>(m_GameObject, EventIDs.OnCharacterActionActive, OnActionActive);
            EventHandler.UnregisterEvent<bool>(m_GameObject, EventIDs.OnAimActionStart, OnAimActionStart);

        }


        protected override void Update()
		{
            m_TimeScale = Time.timeScale;
            if (Math.Abs(m_TimeScale) < float.Epsilon) return;
            m_DeltaTime = deltaTime;

            //m_CheckGround = true;
            //m_CheckMovement = true;
            //m_SetPhysicsMaterial = true;
            //m_Move = true;
            //m_UpdateAnimator = true;


            ////  Start Stop Actions.
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].enabled == false)
                    continue;
                CharacterAction charAction = m_Actions[i];
                StopStartAction(charAction);

                //  Call Action Update.
                charAction.UpdateAction();
            }
        }


        protected override void FixedUpdate()
		{
            if (m_TimeScale == 0) return;
            m_DeltaTime = fixedDeltaTime;

            m_Move = true;
            m_CheckGround = true;
            m_CheckMovement = true;
            m_SetPhysicsMaterial = true;
            m_UpdateRotation = true;
            m_UpdateMovement = true;
            m_UpdateAnimator = true;


            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].enabled == false)
                    continue;
                CharacterAction charAction = m_Actions[i];

                if (charAction.IsActive)
                {
                    //  Move charatcer based on input values.
                    if (m_Move) m_Move = charAction.Move();
                    //  Perform checks to determine if the character is on the ground.
                    if (m_CheckGround) m_CheckGround = charAction.CheckGround();
                    //  Ensure the current movement direction is valid.
                    if (m_CheckMovement) m_CheckMovement = charAction.CheckMovement();


                    //  Update the rotation forces.
                    if (m_UpdateRotation)m_UpdateRotation = charAction.UpdateRotation();
                    //  Apply any movement.
                    if (m_UpdateMovement)m_UpdateMovement = charAction.UpdateMovement();
                    // Update the Animator.
                    if (m_UpdateAnimator)m_UpdateAnimator = charAction.UpdateAnimator();

                }
            }  //  end of for loop

            //  Moves the character according to the input.
            if (m_Move) Move();
            //  Perform checks to determine if the character is on the ground.
            if (m_CheckGround) CheckGround();
            //  Ensure the current movement direction is valid.
            if (m_CheckMovement) CheckMovement();
            //  Set the physic material based on the grounded and stepping state
            if (m_SetPhysicsMaterial) SetPhysicsMaterial();


            //  Update the rotation forces.
            if (m_UpdateRotation) UpdateRotation();
            //  Apply any movement.
            if (m_UpdateMovement) UpdateMovement();
            // Update the Animator.
            if (m_UpdateAnimator) UpdateAnimator();

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

            base.UpdateAnimator();
        }


        /// <summary>
        /// Anything that should be done in the OnAnimatorMove function.
        /// </summary>
        protected override void AnimatorMove()
        {
            base.AnimatorMove();
        }


        /// <summary>
        /// Set the collider's physics material.
        /// </summary>
        protected override void SetPhysicsMaterial()
        {
            base.SetPhysicsMaterial();
        }



        #endregion



        public void Move( float horizontalMovement, float forwardMovement, Quaternion lookRotation )
        {

            LookRotation = lookRotation;
            LookDirection = m_LookRotation * mTransform.forward;

            switch (m_MovementType) {
                case (MovementType.Adventure):
                    //  Set input vector
                    InputVector.Set(0, 0, forwardMovement);
                    //  
                    //DeltaYRotation = Mathf.Atan2(horizontalMovement, forwardMovement) * Mathf.Rad2Deg;
                    //DeltaYRotation -= mTransform.eulerAngles.y;
                    //var direction = Mathf.Abs(InputVector.z) > 0 ? InputVector.x : -InputVector.x;
                    //DeltaYRotation = Mathf.Atan2(direction, Mathf.Abs(InputVector.z)) * Mathf.Deg2Rad;
                    break;

                case (MovementType.Combat):

                    InputVector.Set(horizontalMovement, 0, forwardMovement);

                    Vector3 lookDir = mTransform.InverseTransformDirection(LookDirection);
                    //DeltaYRotation = Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg;
                    //DeltaYRotation -= mTransform.eulerAngles.y;
                    break;
            }
        }




        #region Character Actions

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
                        return;
                    }
                }
            }


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


        protected void OnAimActionStart( bool aim )
        {
            Aiming = aim;
            m_MovementType = Aiming ? MovementType.Combat : MovementType.Adventure;
        }


        protected void ActionStopped()
        {

        }

        #endregion



        public void SetMovementType( MovementType movementType )
        {
            m_MovementType = movementType;
        }


        public void SetPosition( Vector3 position )
        {
            m_Rigidbody.MovePosition(position);
        }


        public void SetRotation( Quaternion rotation )
        {
            m_Rigidbody.MoveRotation(rotation.normalized);
        }


        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Moving = false;
        }





        //------
    }
}
