namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;


    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class CharacterLocomotion : MonoBehaviour
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

        protected float m_StopMovementThreshold = 0.5f;     //  TODO:  Need to add to editor


        //  Physics variables
        [SerializeField, HideInInspector]
        protected float m_Mass = 100;
        [SerializeField, HideInInspector]
        protected float m_SkinWidth = 0.08f;
        [SerializeField, HideInInspector, Range(0, 90)]
        protected float m_SlopeLimit = 45f;
        [SerializeField, HideInInspector]
        protected float m_MaxStepHeight = 0.25f;

        protected float m_ExternalForceDamping = 0.1f;      //  TODO:  Need to add to editor
        [SerializeField, HideInInspector, Range(0, 0.3f), Tooltip("Minimum height to consider a step.")]
        protected float m_StepOffset = 0.15f;
        [SerializeField, HideInInspector]
        protected float m_StepSpeed = 4f;



        //[SerializeField, HideInInspector]
        protected float m_GravityModifier = 0.4f;           //  TODO:  Need to add to editor

        [SerializeField, HideInInspector]
        protected bool m_AlignToGround = true;
        [SerializeField, HideInInspector]
        protected float m_AlignToGroundDepthOffset = 0.5f;




        [SerializeField, HideInInspector]
        protected CharacterAction[] m_Actions;
        [SerializeField]
        private CharacterAction m_ActiveAction;




        [Header("-- Debug --")]
        [SerializeField] bool m_DrawDebugLine;
        private MovementType m_MovementType = MovementType.Adventure;
        private bool m_Grounded = true;
        private bool m_Aiming, m_Moving;
        private float m_InputMagnitude, m_InputMagSmooth;
        private float m_InputAngle, m_InputAngleSmooth, m_InputAngleDamp = 0.18f;
        private Vector3 m_Velocity;
        private Vector3 m_InputVector, m_RelativeInputVector;
        private Vector3 m_LookDirection;
        private Quaternion m_LookRotation;
        private Vector3 m_MoveDirection;
        private Vector3 m_ExternalForce;
        private Vector3 m_GroundNormal;




        private float m_StartAngle, m_StartAngleSmooth;
        private RaycastHit m_GroundHit, m_StepHit;
        private float m_GroundDistance;
        private bool m_OnStep;
        private float m_SlopeAngle;
        private Vector3 m_StepRayStart, m_StepRayDirection;

        private Vector3 m_VelocitySmooth;




        private bool m_CheckGround = true, m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true, m_Move = true, m_CheckMovement = true;

        private CapsuleCollider m_CapsuleCollider;
        private Rigidbody m_Rigidbody;
        private Animator m_Animator;
        private AnimatorMonitor m_AnimationMonitor;
        private LayerManager m_Layers;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;


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



        public float GroundDistance{
            get { return m_GroundDistance; }
        }


        #endregion



        protected void Awake()
        {
            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
            m_Layers = GetComponent<LayerManager>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_GameObject = gameObject;
            m_Transform = transform;

            m_DeltaTime = Time.deltaTime;


            if (m_Rigidbody == null)
                m_Rigidbody = m_GameObject.AddComponent<Rigidbody>();
            if (m_Layers == null)
                m_Layers = m_GameObject.AddComponent<LayerManager>();

            m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_Rigidbody.mass = m_Mass;
            m_Animator.applyRootMotion = m_UseRootMotion;
        }



		protected void OnEnable()
		{
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
            m_InputMagnitude = Mathf.SmoothDamp(m_InputMagnitude, m_InputVector.magnitude, ref m_InputMagSmooth, 0.1f);
            if(m_InputMagnitude < 0.01f || m_InputMagnitude > 0.99f)
                m_InputMagnitude = Mathf.Round(m_InputMagnitude);
            if (m_InputMagnitude > 1)
                m_InputVector.Normalize();
            m_RelativeInputVector = m_InputVector;

            //m_Moving = m_InputMagnitude > m_StopMovementThreshold;
            m_Moving = m_InputMagnitude > 0.2f;




            //  Start Stop Actions.
            StartStopActions();

            CheckGround();

            CheckMovement();

            UpdateRotation();

            UpdateMovement();

            UpdateAnimator();

		}


		private void FixedUpdate()
		{
			
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


        private void CheckGround()
        {
            //  First, do anything that is independent of a character action.


            //  Does the current active character action override this method.
            if (m_CheckGround == true)
            {
                if (m_CapsuleCollider != null)
                {
                    float distance = 10f;
                    //  Position of the SphereCast origin starting at the base of the capsule.
                    Vector3 pos = m_Transform.position + Vector3.up * (m_CapsuleCollider.radius);

                    if (Physics.Raycast(m_Transform.position + Vector3.up * (m_CapsuleCollider.height / 2), Vector3.down,
                                        out m_GroundHit, m_CapsuleCollider.height / 2 + 2 + m_SkinWidth, m_Layers.GroundLayer))
                    {
                        distance = m_Transform.position.y - m_GroundHit.point.y;
                    }
                    if (Physics.SphereCast(m_Transform.position + Vector3.up * (m_CapsuleCollider.radius), m_CapsuleCollider.radius * 0.9f,
                                           -Vector3.up, out m_GroundHit, m_CapsuleCollider.radius * 1 + 2, m_Layers.GroundLayer))
                    {
                        // check if sphereCast distance is small than the ray cast distance
                        if (distance > (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f))
                            distance = (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f);
                    }

                    m_GroundDistance = (float)Math.Round(distance, 2);

                    //  Check if on step.
                    m_OnStep = false;
                    m_StepRayStart = (m_Transform.position + Vector3.up * m_MaxStepHeight) + m_Transform.forward * (m_CapsuleCollider.radius * 2);  //+ m_SkinWidth);
                    if (Physics.Raycast(m_StepRayStart, Vector3.down, out m_StepHit, m_MaxStepHeight - m_SkinWidth, m_Layers.GroundLayer) && !m_StepHit.collider.isTrigger){
                        if (m_StepHit.normal == Vector3.up){
                            if (m_StepHit.point.y >= (m_Transform.position.y) && m_StepHit.point.y <= (m_Transform.position.y + m_MaxStepHeight + m_SkinWidth)){
                                m_Transform.position += (Vector3.up * m_StepOffset * m_InputMagnitude) + ((m_StepHit.point - m_Transform.position) * m_StepSpeed * m_DeltaTime);
                                m_OnStep = true;
                            }
                        }
                    }


                    if (m_GroundDistance <= 0.05f)
                    {
                        m_SlopeAngle = (float)Math.Round(Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90, 2);
                        m_Grounded = true;
                    }
                    else
                    {
                        if (m_GroundDistance >= 0.5f)
                        {
                            m_SlopeAngle = 0;
                            m_Grounded = false;
                            m_Rigidbody.AddForce(Physics.gravity * m_DeltaTime, ForceMode.VelocityChange);
                        }
                        else if (m_OnStep == false)
                        {
                            m_Rigidbody.AddForce(Physics.gravity * 2 * m_DeltaTime, ForceMode.VelocityChange);
                        }

                    }
                }
                m_GroundNormal = m_GroundHit.normal;
            }



        }


        //  Ensure the current movement direction is valid.
        private void CheckMovement()
        {
            //  First, do anything that is independent of a character action.
            m_RelativeInputVector = m_Transform.InverseTransformDirection(m_InputVector);
            m_RelativeInputVector = m_Transform.rotation * m_RelativeInputVector;
            m_RelativeInputVector = Vector3.ProjectOnPlane(m_RelativeInputVector, m_GroundNormal);

            //  Does the current active character action override this method.
            if (m_CheckMovement == true)
            {
                Vector3 axisSign = Vector3.Cross(m_LookDirection, m_Transform.forward);


                m_InputAngle = Vector3.Angle(m_Transform.forward, m_LookDirection) * (axisSign.y >= 0 ? -1f : 1f);
                m_InputAngle = (float)Math.Round(m_InputAngle, 2);

                //m_StartAngle = m_InputAngle;
                if (m_InputMagnitude <= 0.2f)
                    m_StartAngle = m_InputAngle;
                else if (m_InputMagnitude > 0.2f && Mathf.Abs(m_InputAngle) < 10)
                    m_StartAngle = Mathf.SmoothDamp(m_StartAngle, 0, ref m_StartAngleSmooth, 0.25f);
                m_StartAngle = Mathf.Approximately(m_StartAngle, 0) ? 0 : (float)Math.Round(m_StartAngle, 2);




                m_MoveDirection = Vector3.Cross(m_Transform.right, m_GroundHit.normal) * m_InputMagnitude;

                if (m_Grounded)
                {

                }
                if (m_SlopeAngle > m_SlopeLimit)
                {

                }
                if (m_OnStep)
                {

                }
            }
        }


        //  Update the rotation forces.
        private void UpdateRotation()
        {
            //  First, do anything that is independent of a character action.



            //  Does the current active character action override this method.
            if (m_UpdateRotation == true)
            {
                if (m_LookDirection == Vector3.zero)
                    m_LookDirection = m_Transform.forward;

                if (m_InputMagnitude > 0.2f && m_LookDirection.sqrMagnitude > 0.2f)
                {
                    var lookDirection = m_LookDirection.normalized;
                    m_LookRotation = Quaternion.LookRotation(lookDirection, m_Transform.up);

                    if (m_Grounded)
                    {
                        m_LookRotation = Quaternion.Slerp(m_Transform.rotation, m_LookRotation, m_RotationSpeed * m_DeltaTime);

                        m_Rigidbody.MoveRotation(m_LookRotation.normalized);
                    }
                }


            }

        }


        //  Apply any movement.
        private void UpdateMovement()
        {
            //  First, do anything that is independent of a character action.

            //  Does the current active character action override this method.
            if (m_UpdateMovement == true)
            {
                m_Velocity = Vector3.SmoothDamp(m_Velocity, m_MoveDirection, ref m_VelocitySmooth, m_Acceleration);
                m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
                if (m_SlopeAngle < -10)
                    m_Velocity += Vector3.down * m_SlopeForceDown;
                m_Rigidbody.AddForce(m_Velocity * m_DeltaTime, ForceMode.VelocityChange);
            }
        }



		private void Move()
		{
            m_Animator.applyRootMotion = m_UseRootMotion;

            if(m_UseRootMotion)
            {
                float angleInDegrees;
                Vector3 rotationAxis;
                m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
                Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad * m_RotationSpeed;
                m_Rigidbody.angularVelocity = angularDisplacement;


                Vector3 velocity = (m_Animator.deltaPosition / m_DeltaTime) ;
                velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
                //m_Rigidbody.velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_VelocitySmooth, m_Moving ? m_Acceleration : m_MotorDamping);
                //m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, m_MovementSpeed);
                m_Rigidbody.velocity = velocity;
            }
		}


        private void OnAnimatorMove()
        {
            // The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            m_Animator.speed = m_RootMotionSpeedMultiplier;

            for (int i = 0; i < m_Actions.Length; i++){
                if (m_Actions[i].IsActive){
                    if (m_Move) m_Move = m_Actions[i].Move();
                }
            }
            if (m_Move) Move();
        }



		private void UpdateAnimator()
        {
            //  First, do anything that is independent of a character action.

            m_Animator.SetFloat(Animator.StringToHash("StartAngle"), m_StartAngle);
            m_Animator.SetBool(HashID.Moving, m_Moving);
            m_Animator.SetFloat(HashID.InputMagnitude, m_InputMagnitude);
            m_Animator.SetFloat(HashID.InputAngle, m_InputAngle);
            //var localVelocity = Quaternion.Inverse(m_Transform.rotation) * (m_Rigidbody.velocity / m_Acceleration);
            if (m_Animator.pivotWeight > 0.9f){
                m_Animator.SetBool(Animator.StringToHash("StopLeftUp"), true);
            }
            else if (m_Animator.pivotWeight < 0.1f){
                m_Animator.SetBool(Animator.StringToHash("StopRightUp"), true);
            }
            else{
                m_Animator.SetBool(Animator.StringToHash("StopRightUp"), false);
                m_Animator.SetBool(Animator.StringToHash("StopLeftUp"), false);
            }
            m_Animator.SetFloat(Animator.StringToHash("RightFootUp"), m_Animator.pivotWeight);


            //  Does a character action override the controllers update animator.
            if (m_UpdateAnimator == true)
            {
                //  Movement Input
                m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
                m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);
            }
        }







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


        int contactCount;
		private void OnCollisionEnter(Collision collision)
		{
            OnCollision(collision);
		}


		private void OnCollisionStay(Collision collision)
		{
            OnCollision(collision);
		}

        private void OnCollisionExit(Collision collision)
        {
            contactCount = 0;
        }

        private void OnCollision(Collision collision)
        {
            contactCount = collision.contactCount;
            for (int i = 0; i < collision.contactCount; i++)
            {
                var start = m_Animator.bodyPosition;
                var contactDir = collision.GetContact(i).point - start;
                Debug.DrawLine(start, collision.GetContact(i).point, Color.yellow);
                //Debug.DrawRay(start, contactDir, Color.yellow);
            }
        }








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
                                m_Actions[i].UpdateAnimator();
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


                //  Next, if current Action is active, update overrides.
                m_CheckMovement = m_UpdateRotation = m_UpdateMovement = m_UpdateAnimator = m_Move = true;
                //  Call Action Update.
                m_Actions[i].UpdateAction();

                if (m_Actions[i].IsActive)
                {
                    if (m_CheckMovement)
                    {
                        m_CheckMovement = m_Actions[i].CheckMovement();
                    }
                    if (m_UpdateRotation)
                    {
                        m_UpdateRotation = m_Actions[i].UpdateRotation();
                    }
                    if (m_UpdateMovement)
                    {
                        m_UpdateMovement = m_Actions[i].UpdateMovement();
                    }
                    if (m_UpdateAnimator)
                    {
                        m_UpdateAnimator = m_Actions[i].UpdateAnimator();
                    }
                }


            }  //  end of for loop
            //  end of
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















		private Vector3 m_DebugHeightOffset = new Vector3(0, 0.25f, 0);
        private Color _Magenta = new Color(0.8f, 0, 0.8f, 0.8f);

        protected void OnDrawGizmos()
        {
            
            #region Slope Check
            //float slopeCheckHeight = 0.5f;
            //Quaternion rotation = Quaternion.AngleAxis(m_SlopeLimit, -transform.right);
            ////  Hypotenuse
            //Gizmos.color = _Magenta;
            //float slopeCheckHypotenuse = slopeCheckHeight / Mathf.Cos(m_SlopeLimit);
            //Vector3 slopeAngleVector = slopeCheckHypotenuse * transform.forward;
            //Gizmos.DrawRay(transform.position + (transform.forward) * 0.3f, rotation * slopeAngleVector - (slopeAngleVector * 0.3f));

            //  Check distance
            //Gizmos.color = Color.magenta;
            //float slopeCheckDistance = Mathf.Tan(m_SlopeLimit) * slopeCheckHeight;
            //Vector3 slopeCheckVector = slopeCheckDistance * transform.forward;
            //Gizmos.DrawRay(transform.position + transform.up * slopeCheckHeight, slopeCheckVector );//- (transform.forward) * 0.3f);
            #endregion

            if(m_DrawDebugLine && Application.isPlaying)
            {
                //Gizmos.color = Color.blue;
                //Gizmos.DrawRay(m_Transform.position + m_DebugHeightOffset, m_Velocity);
                //GizmosUtils.DrawString("m_Velocity", m_Transform.position + m_Velocity, Color.white);

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(m_Transform.position + m_DebugHeightOffset, m_LookDirection);
                GizmosUtils.DrawString("m_LookDirection", m_Transform.position + m_LookDirection, Color.white);

                Gizmos.color = m_OnStep ? Color.yellow : Color.white;
                Gizmos.DrawRay(m_StepRayStart, Vector3.down * (m_MaxStepHeight - m_SkinWidth));
                GizmosUtils.DrawString("Step", m_StepRayStart + Vector3.up * m_MaxStepHeight, Color.white);



            }
        }


        private void DebugMessages()
        {
            debugMsgs = new string[]
            {
                //string.Format("Input Mag: {0}", m_InputMagnitude),
                //string.Format("Input Angle: {0}", m_InputAngle),
                //string.Format("Start Angle Vector: {0}", m_StartAngle),
                //string.Format("Relative Input: {0}", m_RelativeInputVector),
                //string.Format("Pivot Position: {0}", m_Animator.pivotPosition),
                //string.Format("Pivot Weight: {0}", m_Animator.pivotWeight),
                //string.Format("Center of Mass: {0}", m_Animator.bodyPosition),
                //string.Format("Feet Pivot Action: {0}", m_Animator.feetPivotActive),


                string.Format("Next State (short): {0}\n", m_Animator.GetNextAnimatorStateInfo(0).shortNameHash),
                string.Format("Current State (short): {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash),
                string.Format("Next State (full): {0}\n", m_Animator.GetNextAnimatorStateInfo(0).fullPathHash),
                string.Format("Current State (full): {0}\n", m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash),
                string.Format("In Transition?: {0}\n", m_Animator.IsInTransition(0)),
                string.Format("Transition Norm Time: {0}\n", m_Animator.GetAnimatorTransitionInfo(0).normalizedTime),
                string.Format("Transition Duration: {0}\n", m_Animator.GetAnimatorTransitionInfo(0).duration),
            };
        }


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

                rect.y = rectStartHeight;
                //groupRect.y = rect.y + rectStartHeight * 2;
                groupRect.height = labelHeight * debugMsgs.Length + rectStartHeight;

                GUI.BeginGroup(groupRect, GUI.skin.box);
                for (int i = 0; i < debugMsgs.Length; i++)
                {
                    rect.y = rectStartHeight + labelHeight * i;
                    GUI.Label(rect, debugMsgs[i]);
                }
                GUI.EndGroup();
            }

        }






    }

}
