namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Text;

    //[DisallowMultipleComponent]
    //[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class CharacterLocomotion : RigidbodyController
    {
        public event Action<bool> OnAim = delegate {};
        public enum MovementType { Adventure, Combat };

        //  Locomotion variables
        [SerializeField, HideInInspector]
        protected bool m_UseRootMotion = true;
        [SerializeField, HideInInspector]
        protected float m_RootMotionSpeedMultiplier = 1;
        [SerializeField, HideInInspector]
        protected float m_Acceleration = 0.12f;
        [SerializeField, HideInInspector]
        protected float m_MotorDamping = 0.2f;             //  TODO:  Need to add to editor
        [SerializeField, HideInInspector]
        protected float m_MovementSpeed = 1f;
        [SerializeField, HideInInspector]
        protected float m_RotationSpeed = 10f;
        [SerializeField, HideInInspector]
        protected float m_SlopeForceDown = 1.25f;

        protected float m_StopMovementThreshold = 0.2f;     //  TODO:  Need to add to editor


        [SerializeField, HideInInspector]
        protected CharacterAction[] m_Actions;
        [SerializeField]
        private CharacterAction m_ActiveAction;





        private MovementType m_MovementType = MovementType.Adventure;
        [SerializeField, DisplayOnly]
        private bool m_Moving;
        private bool m_Aiming;
        private float m_InputMagnitude, m_InputMagSmooth;
        private float m_InputAngle, m_InputAngleSmooth, m_InputAngleDamp = 0.18f;
        private Vector3 m_Velocity, m_VerticalVelocity;
        private Vector3 m_InputVector, m_RelativeInputVector;
        private Vector3 m_LookDirection;
        private Quaternion m_LookRotation;
        private Vector3 m_MoveDirection;
        private Vector3 m_ExternalForce;


        private float m_VelocityY;
        private float m_Stickiness;

        private float m_StartAngle, m_StartAngleSmooth;
        //private RaycastHit m_GroundHit, m_StepHit;
        //private float m_GroundDistance;
        private bool m_OnStep;
        private float m_SlopeAngle;
        private Vector3 m_StepRayStart, m_StepRayDirection;

        private Vector3 m_VelocitySmooth;



        //[SerializeField]
        private bool m_CheckGround = true, m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true, m_Move = true, m_CheckMovement = true;


        private Animator m_Animator;
        private AnimatorMonitor m_AnimationMonitor;



        #region Properties

        public bool Moving{
            get { return m_Moving;}
            set { m_Moving = value; }
        }

        public bool Aiming{
            get{
                if (m_Aiming && Grounded)
                    return true;
                return false;
            }
            set { m_Aiming = value; }
        }

        public bool Grounded{
            get { return m_Grounded; }
            set { m_Grounded = value; }
        }

        public float RotationSpeed{
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }

        public Vector3 InputVector{
            get { return m_InputVector; }
            set { m_InputVector = value; }
        }

        public Vector3 LookDirection{
            get { return m_LookDirection; }
            set { m_LookDirection = value; }
        }

        public Vector3 Velocity{
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        public Quaternion LookRotation{
            get { return m_LookRotation; }
            set { m_LookRotation = value; }
        }

        public CharacterAction[] CharActions{
            get { return m_Actions; }
            set { m_Actions = value; }
        }






        #endregion



        protected override void Awake()
        {
            base.Awake();
            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
            m_Animator = GetComponent<Animator>();
        }



		protected void OnEnable()
		{
            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_Rigidbody.mass = m_Mass;
            m_Animator.applyRootMotion = m_UseRootMotion;

            EventHandler.RegisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}


		protected void OnDisable()
		{
            EventHandler.UnregisterEvent<CharacterAction, bool>(m_GameObject, "OnCharacterActionActive", OnActionActive);
            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}





        private void Update()
		{
            m_Animator.applyRootMotion = m_UseRootMotion;
            ////  Start Stop Actions.
            //StartStopActions();


            m_UpdateAnimator = true;
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].enabled == false)
                    continue;
                CharacterAction charAction = m_Actions[i];
                StopStartAction(charAction);


                if(charAction.IsActive){
                    if (m_UpdateAnimator) m_UpdateAnimator = charAction.UpdateAnimator();
                }

                //  Call Action Update.
                charAction.UpdateAction();
            }


            UpdateAnimator();
		}


		private void FixedUpdate()
		{
            //m_InputMagnitude = Mathf.SmoothDamp(m_InputMagnitude, m_InputVector.magnitude, ref m_InputMagSmooth, 0.1f);
            m_InputMagnitude = m_InputVector.magnitude;
            if (m_InputMagnitude < 0.01f || m_InputMagnitude > 0.99f)
                m_InputMagnitude = Mathf.Round(m_InputMagnitude);
            if (m_InputMagnitude > 1)
                m_InputVector.Normalize();

            m_Moving = m_InputMagnitude > m_StopMovementThreshold;
            //m_Moving = m_InputMagnitude > 0.2f;



            m_CheckMovement = true;
            m_CheckGround = true;
            m_UpdateRotation = true;
            m_UpdateMovement = true;
            for (int i = 0; i < m_Actions.Length; i++)
            {
                if (m_Actions[i].enabled == false)
                    continue;
                CharacterAction charAction = m_Actions[i];

                if (charAction.IsActive)
                {
                    if (m_CheckMovement) m_CheckMovement = charAction.CheckMovement();
                    if (m_CheckGround) m_CheckGround = charAction.CheckGround();
                    if (m_UpdateRotation) m_UpdateRotation = charAction.UpdateRotation();
                    if (m_UpdateMovement) m_UpdateMovement = charAction.UpdateMovement();
                }
            }  //  end of for loop


            CheckMovement();
            CheckGround();
            UpdateRotation();
            UpdateMovement();
		}


		//  Should the character look independetly of the camera?  AI Agents do not need to use camera rotation.
		public bool IndependentLook()
        {
            if(m_Moving || m_Aiming){
                return false;
            }
            //if(m_Aiming) return true;
            return true;
        }






        protected void CheckGround()
        {
            //  First, do anything that is independent of a character action.


            //  Does the current active character action override this method.
            if (m_CheckGround == true)
            {

                m_GroundDistance = 10;

                m_GroundCheckHeight = m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2 + m_SkinWidth;
                if (Physics.Raycast(m_Rigidbody.position + Vector3.up * m_GroundCheckHeight, Vector3.down,
                                    out m_GroundHit, m_CapsuleCollider.radius * 2, m_Layers.GroundLayer))
                {
                    //m_GroundDistance = m_Transform.position.y - m_GroundHit.point.y;
                    m_GroundDistance = Vector3.Project(m_Rigidbody.position - m_GroundHit.point, m_Transform.up).magnitude;
                }

                if (Physics.SphereCast(m_Rigidbody.position + Vector3.up * m_CapsuleCollider.radius, m_CapsuleCollider.radius + m_SkinWidth,
                                       Vector3.down, out m_GroundHit, m_CapsuleCollider.radius + 2, m_Layers.GroundLayer))
                {
                    // check if sphereCast distance is small than the ray cast distance
                    if (m_GroundDistance > (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f))
                        m_GroundDistance = (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f);
                }

  
                m_GroundDistance = (float)Math.Round(m_GroundDistance, 2);


                if(m_GroundDistance < m_AirbornThreshold * 0.5f ){
                    Vector3 horizontalVelocity = Vector3.Project(m_Rigidbody.velocity, m_Gravity);
                    m_Stickiness = m_GroundStickiness * horizontalVelocity.magnitude * m_AirbornThreshold;
                    m_Grounded = true;
                }
                else{
                    if(m_GroundDistance > m_AirbornThreshold){
                        m_Grounded = false;
                    }
                       
                }
            }

        }


        //  Ensure the current movement direction is valid.
        protected void CheckMovement()
        {
            //  First, do anything that is independent of a character action.
            //  -----------


            //  -----------
            //  Does the current active character action override this method.
            //  -----------
            if (m_CheckMovement == true)
            {
                ////  Start walk angle
                Vector3 axisSign = Vector3.Cross(m_LookDirection, m_Transform.forward);
                m_InputAngle = Vector3.Angle(m_Transform.forward, m_LookDirection) * (axisSign.y >= 0 ? -1f : 1f);
                m_InputAngle = (float)Math.Round(m_InputAngle, 2);
                //m_StartAngle = m_InputAngle;
                if (m_InputMagnitude <= 0.2f)
                    m_StartAngle = m_InputAngle;
                else if (m_InputMagnitude > 0.2f && Mathf.Abs(m_InputAngle) < 10)
                    m_StartAngle = Mathf.SmoothDamp(m_StartAngle, 0, ref m_StartAngleSmooth, 0.25f);
                m_StartAngle = Mathf.Approximately(m_StartAngle, 0) ? 0 : (float)Math.Round(m_StartAngle, 2);




                m_VerticalVelocity = Vector3.Project(m_Rigidbody.velocity, m_Gravity);
                m_VelocityY = m_VerticalVelocity.magnitude;
                if (Vector3.Dot(m_VerticalVelocity, m_Gravity) > 0)
                    m_VelocityY = -m_VelocityY;
                
                if (m_Grounded)
                {
                    m_MoveDirection = Vector3.Cross(m_Transform.right, m_GroundHit.normal) * m_InputMagnitude;
                    m_Velocity = m_MoveDirection * m_InputMagnitude + (m_DeltaTime * (Physics.gravity * m_GravityModifier));

                    m_Rigidbody.velocity = m_Rigidbody.velocity - m_Transform.up * m_Stickiness * m_DeltaTime;

                    if (m_SlopeAngle > m_SlopeLimit)
                    {
                        if (m_InputVector == Vector3.zero && m_Rigidbody.velocity.sqrMagnitude < (0.5f * 0.5f))
                        {
                            m_Rigidbody.velocity = Vector3.zero;
                        }
                    }

                }
                ////  If airborne.
                //else{
                //    m_MoveDirection = m_Transform.forward + (m_DeltaTime * (Physics.gravity * m_GravityModifier));
                //    m_Velocity = m_MoveDirection + Vector3.up * m_Rigidbody.velocity.y;
                //}

            }
        }


        //  Update the rotation forces.
        protected void UpdateRotation()
        {
            //  First, do anything that is independent of a character action.
            //  -----------

            //  -----------
            //  Does the current active character action override this method.
            //  -----------
            if (m_UpdateRotation == true)
            {
                if (m_LookDirection == Vector3.zero)
                    m_LookDirection = m_Transform.forward;

                if (m_InputMagnitude > 0.2f && m_LookDirection.sqrMagnitude > 0.2f)
                {
                    Vector3 local = m_Transform.InverseTransformDirection(m_LookDirection);
                    float angle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
                    if (m_InputVector == Vector3.zero)
                        angle *= (1.01f - (Mathf.Abs(angle) / 180)) * 1;

                    //m_Rigidbody.MoveRotation(Quaternion.AngleAxis(angle * m_DeltaTime * m_RotationSpeed, m_Transform.up) * m_Rigidbody.rotation);
                    
                    Quaternion rotation = Quaternion.AngleAxis(angle * m_DeltaTime * m_RotationSpeed, m_Transform.up);
                    if(m_Grounded){
                        Vector3 d = transform.position - m_Animator.pivotPosition;
                        m_Rigidbody.MovePosition(m_Animator.pivotPosition + rotation * d);
                    }
                    m_Rigidbody.MoveRotation(rotation * m_Rigidbody.rotation);
                }
            }
        }




        //  Apply any movement.
        protected void UpdateMovement()
        {
            //  First, do anything that is independent of a character action.
            //  -----------


            //  -----------
            //  Does the current active character action override this method.
            //  -----------
            if (m_UpdateMovement == true)
            {
                if (Vector3.Dot(m_VerticalVelocity, m_Gravity) < 0f){
                    float maxVerticalVelocityOnGround = 3;
                    m_VerticalVelocity = Vector3.ClampMagnitude(m_VerticalVelocity, maxVerticalVelocityOnGround);
                }
                //else{
                //    m_VerticalVelocity = m_Gravity * m_GravityModifier;
                //}
                //m_Velocity + m_VerticalVelocity;
                if(m_Grounded)
                {
                    //m_Velocity = Vector3.SmoothDamp(m_Velocity, m_MoveDirection, ref m_VelocitySmooth, m_Acceleration);
                    m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
                    if (m_SlopeAngle < 0)
                        m_Velocity += Vector3.down * m_SlopeForceDown;
                    m_Rigidbody.AddForce(m_Velocity * m_DeltaTime, ForceMode.VelocityChange);
                    //m_Rigidbody.velocity = m_Velocity * m_DeltaTime + m_VerticalVelocity;
                }
                else{
                    //m_Rigidbody.AddForce(m_VerticalVelocity * m_DeltaTime);
                    //m_Rigidbody.velocity = Vector3.up * m_VelocityY;
                    m_Rigidbody.velocity = m_VerticalVelocity;
                }

            }
        }



        protected void Move()
		{
            //  First, do anything that is independent of a character action.
            //  -----------


            //  -----------
            //  Does the current active character action override this method.
            //  -----------
            if(m_Move == true)
            {
                if (m_UseRootMotion)
                {
                    float angleInDegrees;
                    Vector3 rotationAxis;
                    m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
                    Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad * m_RotationSpeed;
                    m_Rigidbody.angularVelocity = angularDisplacement;



                    Vector3 velocity = (m_Animator.deltaPosition / m_DeltaTime);
                    velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
                    //m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_VelocitySmooth, m_Moving ? m_Acceleration : m_MotorDamping);
                    //m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, m_MovementSpeed);
                    m_Rigidbody.velocity = velocity;
                }
            }

		}

        private float deltaVelY;
        private void OnAnimatorMove()
        {
            deltaVelY = m_Animator.deltaPosition.y;
            m_Move = true;
            for (int i = 0; i < m_Actions.Length; i++){
                if (m_Actions[i].IsActive){
                    if (m_Move) m_Move = m_Actions[i].Move();
                }
            }


            Move();
        }



        protected void UpdateAnimator()
        {
            //  First, do anything that is independent of a character action.
            //  -----------

            // The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            m_Animator.speed = m_RootMotionSpeedMultiplier;

            m_Animator.SetFloat(HashID.StartAngle, m_StartAngle);
            m_Animator.SetBool(HashID.Moving, m_Moving);
            m_Animator.SetFloat(HashID.InputMagnitude, m_InputMagnitude);
            m_Animator.SetFloat(HashID.InputAngle, (m_InputAngle * Mathf.Deg2Rad));
            //var localVelocity = Quaternion.Inverse(m_Transform.rotation) * (m_Rigidbody.velocity / m_Acceleration);
            if (m_Animator.pivotWeight > 0.9f){
                m_Animator.SetBool(HashID.StopLeftUp, true);
            }
            else if (m_Animator.pivotWeight < 0.1f){
                m_Animator.SetBool(HashID.StopRightUp, true);
            }
            else{
                m_Animator.SetBool(HashID.StopLeftUp, false);
                m_Animator.SetBool(HashID.StopRightUp, false);
            }
            m_Animator.SetFloat(HashID.LegUpIndex, m_Animator.pivotWeight);


            //  -----------
            //  Does a character action override the controllers update animator.
            //  -----------
            if (m_UpdateAnimator == true)
            {
                //  Movement Input
                m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
                m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);
            }
        }





        #region Public Functions

        public void SetPosition(Vector3 position){
            m_Rigidbody.MovePosition(position);
        }


        public void SetRotation(Quaternion rotation){
            m_Rigidbody.MoveRotation(rotation.normalized);
        }


		public void MoveCharacter(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            throw new NotImplementedException(string.Format("<color=yellow>{0}</color> MoveCharacter not Implemented.", GetType()));
        }


        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Moving = false;
        }


        #endregion





        #region Actions

        private void StartStopActions()
        {
            for (int i = 0; i < m_Actions.Length; i++)
            {
                //  First, check if current Action can Start or Stop.
                var currentAction = m_Actions[i];
                //  Current Action is Active.
                if (m_Actions[i].enabled && m_Actions[i].IsActive)
                {
                    //  Check if can stop Action is StopType is NOT Manual.
                    if (m_Actions[i].StopType != ActionStopType.Manual)
                    {
                        if (m_Actions[i].CanStopAction())
                        {
                            //  Start the Action and update the animator.
                            m_Actions[i].StopAction();
                            //m_Actions[i].UpdateAnimator();
                            //  Reset Active Action.
                            if (m_ActiveAction = m_Actions[i])
                                m_ActiveAction = null;
                            //  Move on to the next Action.
                            continue;
                        }
                    }
                }
                //  Current Action is NOT Active.
                else
                {
                    //  Check if can start Action is StartType is NOT Manual.
                    if (m_Actions[i].enabled && m_Actions[i].StartType != ActionStartType.Manual )
                    {
                        if (m_ActiveAction == null)
                        {
                            if (m_Actions[i].CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_Actions[i].StartAction();
                                //m_Actions[i].UpdateAnimator();
                                //  Set active Action if not concurrent.
                                if (m_Actions[i].IsConcurrentAction() == false)
                                    m_ActiveAction = m_Actions[i];
                                //  Move onto the next Action.
                                continue;
                            }
                        }
                        else if (m_Actions[i].IsConcurrentAction())
                        {
                            if (m_Actions[i].CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_Actions[i].StartAction();
                                //m_Actions[i].UpdateAnimator();
                                //  Move onto the next Action.
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }


                //  Call Action Update.
                m_Actions[i].UpdateAction();

            }  //  end of for loop
            //  end of
        }




        private void StopStartAction(CharacterAction charAction)
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
                        action.UpdateAnimator();
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
                    action.UpdateAnimator();
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
                    action.UpdateAnimator();
                    return true;
                }
            }


            return false;
        }


        public void TryStopAllActions()
        {

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



        #region OnAction execute

        private void OnAimActionStart(bool aim)
        {
            m_Aiming = aim;
            m_MovementType = m_Aiming ? MovementType.Combat : MovementType.Adventure;
            //  Call On Aim Delegate.
            OnAim(aim);

            //CameraController.Instance.FreeRotation = aim;
            //Debug.LogFormat("Camera Free Rotation is {0}", CameraController.Instance.FreeRotation);
        }


        private void OnActionActive(CharacterAction action, bool activated)
        {
            //int index = Array.IndexOf(m_Actions, action);
            //if(activated){
            //    m_ActiveActions[index] = m_Actions[index];
            //    Debug.LogFormat(" {0} is active.", action.GetType().Name);
            //}
            //else{
            //    m_ActiveActions[index] = null;
            //    Debug.LogFormat(" {0} is now not active.", action.GetType().Name);
            //}
        }


        public void ActionStopped()
        {

        }

        #endregion








        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            //Gizmos.color = Color.blue;
            //Gizmos.DrawRay(m_Transform.position + m_DebugHeightOffset, m_Velocity);
            //GizmosUtils.DrawString("m_Velocity", m_Transform.position + m_Velocity, Color.white);

            Gizmos.color = m_Grounded ? Color.green : Color.red;
            Gizmos.DrawRay(m_Transform.position + Vector3.up * m_GroundCheckHeight, Vector3.down * m_CapsuleCollider.radius * 2);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(m_Transform.position + m_DebugHeightOffset, m_LookDirection);
            //GizmosUtils.DrawString("m_LookDirection", m_Transform.position + m_MoveDirection, Color.white);

            //Gizmos.color = m_OnStep ? Color.yellow : Color.white;
            //Gizmos.DrawRay(m_StepRayStart, Vector3.down * (m_MaxStepHeight - m_SkinWidth));
            //GizmosUtils.DrawString("Step", m_StepRayStart + Vector3.up * m_MaxStepHeight, Color.white);
        }









        private void DebugMessages()
        {
            //debugMessages.AppendFormat("Ground Distance: {0} \n", m_GroundDistance);
            //debugMessages.AppendFormat("VelocityY: {0} \n", (float)Math.Round(m_VelocityY, 2));
            //debugMessages.AppendFormat("Grounded: {0} \n", m_Grounded);
            //debugMessages.AppendFormat("deltaAngle: {0} \n", deltaAngle);
            debugMsgs = new string[]
            {
                string.Format("Ground Distance: {0}", m_GroundDistance),
                string.Format("VelocityY: {0}", (float)Math.Round(m_VelocityY, 2)),
                string.Format("Grounded: {0}", m_Grounded),
                string.Format("Ground Distance: {0}", m_GroundDistance),
                string.Format("Airborne: {0}", m_AirbornThreshold),
                string.Format("deltaVelY: {0}", deltaVelY),
                string.Format("RigidbodyVelY: {0}", (float)Math.Round(m_Rigidbody.velocity.y, 2)),

                string.Format("Next State (short): {0}\n", m_Animator.GetNextAnimatorStateInfo(0).shortNameHash),
                string.Format("Next State (full): {0}\n", m_Animator.GetNextAnimatorStateInfo(0).fullPathHash),


                //string.Format("Current State (short): {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash),

                //string.Format("Current State (full): {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash),
                //string.Format("In Transition?: {0}\n", m_Animator.IsInTransition(0)),
                //string.Format("Transition Norm Time: {0}\n", m_Animator.GetAnimatorTransitionInfo(0).normalizedTime),
                //string.Format("Transition Duration: {0}\n", m_Animator.GetAnimatorTransitionInfo(0).duration),
            };
        }

        StringBuilder debugMessages = new StringBuilder();
        //Color debugTextColor = new Color(0, 1f, 1f, 1);
        Color debugTextColor = Color.black;
        string[] debugMsgs;
        Rect groupRect = new Rect(10, 50, 200, 20);
        Rect rect = new Rect(10, 50, 200, 20);
        float rectStartHeight = 50;
        float labelHeight = 16;
        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                GUI.color = debugTextColor;
                DebugMessages();

                //rect.y = rectStartHeight;
                //groupRect.y = rect.y + rectStartHeight * 2;
                groupRect = rect;
                //groupRect.height = labelHeight * debugMsgs.Length + rectStartHeight;
                groupRect.height = Screen.height * 0.75f;


                GUI.BeginGroup(groupRect, GUI.skin.box);

                //GUI.Label(rect, debugMessages.ToString(0, debugMessages.Length));

                for (int i = 0; i < debugMsgs.Length; i++)
                {
                    rect.y = labelHeight * i;
                    GUI.Label(rect, debugMsgs[i]);
                }
                GUI.EndGroup();
            }

        }






    }

}
