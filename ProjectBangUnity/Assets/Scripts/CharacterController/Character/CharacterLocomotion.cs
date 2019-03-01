namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;


    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class CharacterLocomotion : MonoBehaviour
    {
        public event Action<bool> OnAim = delegate {};


        public enum MovementType {Adventure, TopDown };

        [SerializeField, HideInInspector]
        protected bool m_UseRootMotion = true;
        [SerializeField, HideInInspector]
        protected float m_RootMotionSpeedMultiplier = 1;
        [SerializeField, HideInInspector]
        protected float m_RotationSpeed = 4f;
        [SerializeField, HideInInspector]
        protected float m_AimRotationSpeed = 8f;
        [SerializeField, HideInInspector]
        protected bool m_AlignToGround = true;
        [SerializeField, HideInInspector]
        protected float m_AlignToGroundDepthOffset = 0.5f;
        [SerializeField, HideInInspector]
        protected Vector3 m_GroundSpeed = new Vector3(1, 0, 1);
        [SerializeField, HideInInspector]
        protected float m_SkinWidth = 0.08f;
        [SerializeField, HideInInspector]
        protected float m_SlopeLimit = 30f;

        [SerializeField, HideInInspector]
        protected float m_MaxStepHeight = 0.65f;
        [SerializeField, HideInInspector, Range(0, 0.3f) ]
        protected float m_StepOffset = 0.15f;
        [SerializeField, HideInInspector]
        protected float m_StepSpeed = 4f;
        [SerializeField, HideInInspector]
        protected float m_Acceleration = 20f;

        [SerializeField]
        protected CharacterAction[] m_Actions;
        [SerializeField]
        private CharacterAction m_ActiveAction;
        [SerializeField, HideInInspector]
        private CharacterAction[] m_ActiveActions;
        private CharacterAction m_CurrentAction;


        protected PhysicMaterial m_GroundedIdleFrictionMaterial; 
        protected PhysicMaterial m_GroundedMovingFrictionMaterial;
        protected PhysicMaterial m_StepFrictionMaterial;
        protected PhysicMaterial m_SlopeFrictionMaterial;
        protected PhysicMaterial m_AirFrictionMaterial;



        [Header("-- Debug --")]
        private bool m_Grounded = true;
        private bool m_Moving, m_Aiming, m_Running;
        private float m_Speed;
        Vector3 m_Velocity, m_CurrentVelocity, m_TargetVelocity;
        Vector3 m_MoveDirection;
        [SerializeField]
        Vector3 m_InputVector;
        Quaternion m_LookRotation;

        [Header("-- Rigidbody Debug --")]

        [SerializeField] private Vector3 _rigidbodyVelocity;
        [SerializeField] private Vector3 _rigidbodyAngularVelocity;


        PlayerInput m_Input;
        AnimatorMonitor m_AnimationMonitor;
        LayerManager m_Layers;
        Animator m_Animator;
        CapsuleCollider m_CapsuleCollider;
        Rigidbody m_Rigidbody;
        Camera m_Camera;
        GameObject m_GameObject;
        Transform m_Transform;
        Transform m_LookAtPoint;
        float m_DeltaTime;

        bool m_UpdateRotation = true, m_UpdateMovement = true, m_UpdateAnimator = true;



        RaycastHit m_GroundHit;
        RaycastHit m_StepHit;


        float m_SlopeAngle;






        [SerializeField] bool m_DrawDebugLine;
        [SerializeField] bool m_ShowComponents;
        bool m_DebugActiveActions;
        [SerializeField, HideInInspector] CharacterAction m_SelectedAction;




        #region Properties

        public bool Moving
        {
            get{
                m_Moving = Mathf.Abs(InputVector.x) < 1 && Mathf.Abs(InputVector.z) < 1;
                return m_Moving;
            }
            set { m_Moving = value; }
        }

        public bool Running{
            get { return m_Running; }
            set { m_Running = value; }
        }

        public float RotationSpeed
        {
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }

        public float AimRotationSpeed
        {
            get { return m_AimRotationSpeed; }
            set { m_AimRotationSpeed = value; }
        }

        public bool AimState
        {
            get { return m_Aiming; }
            set { m_Aiming = value; }
        }

        public bool Aiming{
            get {
                if(m_Aiming){
                    if(Grounded){
                        return true;
                    }
                }
                return false;
            }
        }

        public Vector3 InputVector
        {
            get { return m_InputVector; }
            set { m_InputVector = value; }
        }

        public Vector3 RelativeInputVector
        {
            get { return m_InputVector.normalized; }
        }

        public Quaternion LookRotation
        {
            get { return m_LookRotation; }
            set { m_LookRotation = value; }
        }

        public bool Grounded
        {
            get { return m_Grounded; }
            set { m_Grounded = value; }
        }

        public Vector3 Velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }


        public Vector3 LookPosition{
            get { return m_LookAtPoint.position; }
            set { m_LookAtPoint.position = value; }
        }

        public CharacterAction[] CharActions
        {
            get { return m_Actions; }
            set { m_Actions = value; }
        }


        #endregion



        protected void Awake()
        {
            m_Input = GetComponent<PlayerInput>();
            m_AnimationMonitor = GetComponent<AnimatorMonitor>();
            m_Layers = GetComponent<LayerManager>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;


            if(m_Rigidbody == null) m_Rigidbody = m_GameObject.AddComponent<Rigidbody>();
            if(m_Layers == null) m_Layers = m_GameObject.AddComponent<LayerManager>();


            m_LookAtPoint = new GameObject("LookAtPoint").transform; //.parent = gameObject.transform;
            m_LookAtPoint.transform.parent = m_GameObject.transform;
            m_LookAtPoint.position = (m_Transform.forward * 10) + (Vector3.up * 1.35f);


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


        public void PlayInteractAnimation()
        {
            var actionIntId = UnityEngine.Random.Range(0, 3);
            m_Animator.SetInteger(HashID.ActionIntData, actionIntId);
            //m_Animator.CrossFade("Pickup_Item.Pickup_Item_Ground", 0.2f, 0);
            m_Animator.CrossFade("Pickup_Item.OnEntry", 0.2f, 0);
            Debug.Log("Playing Animation " + Animator.StringToHash("Pickup_Item.Pickup_Item_Ground") + " | ID: " + actionIntId );
        }


		protected void Start()
        {
            m_ActiveActions = new CharacterAction[m_Actions.Length];

            if(m_GroundedMovingFrictionMaterial == null){
                // slides the character through walls and edges
                m_GroundedMovingFrictionMaterial = new PhysicMaterial();
                m_GroundedMovingFrictionMaterial.name = "GroundedMovingPhysics";
                m_GroundedMovingFrictionMaterial.staticFriction = .25f;
                m_GroundedMovingFrictionMaterial.dynamicFriction = .25f;
                m_GroundedMovingFrictionMaterial.frictionCombine = PhysicMaterialCombine.Multiply;
            }


            if (m_GroundedIdleFrictionMaterial == null){
                // prevents the collider from slipping on ramps
                m_GroundedIdleFrictionMaterial = new PhysicMaterial();
                m_GroundedIdleFrictionMaterial.name = "GroundedIdlePhysics";
                m_GroundedIdleFrictionMaterial.staticFriction = 1f;
                m_GroundedIdleFrictionMaterial.dynamicFriction = 1f;
                m_GroundedIdleFrictionMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
            }


            if (m_AirFrictionMaterial == null){
                // air physics 
                m_AirFrictionMaterial = new PhysicMaterial();
                m_AirFrictionMaterial.name = "AirFrictionPhysics";
                m_AirFrictionMaterial.staticFriction = 0f;
                m_AirFrictionMaterial.dynamicFriction = 0f;
                m_AirFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            }


            if (m_Input) m_Camera = CameraController.Instance.Camera;
        }




        private void Update()
		{
            if (m_DeltaTime == 0) return;


            m_Grounded = CheckGround();


            if (m_Actions != null)
            {
                m_UpdateRotation = true;
                m_UpdateMovement = true;
                m_UpdateAnimator = true;

                for (int i = 0; i < m_Actions.Length; i++)
                {
                    //  STARTING
                    //  If active actions list slot is null, that means that action can attempt to be started.
                    //  If it contains an item, that means that action is currently active.
                    if (m_ActiveActions[i] == null)
                    {
                        //  Can start if there is no active action.
                        if (m_ActiveAction == null)
                        {
                            if (m_Actions[i].CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_Actions[i].StartAction();
                                m_Actions[i].UpdateAnimator();

                                //  Cache the active action.
                                m_ActiveAction = m_Actions[i];
                                m_ActiveActions[i] = m_Actions[i];
                                //  Move on to the next Action.
                                continue;
                            }
                        }
                        //  Can start if action is concurrent
                        else if (m_Actions[i].IsConcurrentAction())
                        {
                            if (m_Actions[i].CanStartAction())
                            {
                                //  Start the Action and update the animator.
                                m_Actions[i].StartAction();
                                m_Actions[i].UpdateAnimator();
                                //  Cache the active action.
                                m_ActiveActions[i] = m_Actions[i];
                                //  Move on to the next Action.
                                continue;
                            }
                        }
                        //  What to do if there is an active action.
                        else if (m_ActiveAction != null)
                        {

                        }
                    }
                    //  STOPPING
                    //  If the active actions slot is equal to the current action, than that action can be stopped.
                    else if (m_ActiveActions[i] == m_Actions[i])
                    {
                        if (m_Actions[i].CanStopAction())
                        {
                            //  Start the Action and update the animator.
                            m_Actions[i].StopAction();
                            m_Actions[i].UpdateAnimator();
                            //  Cache the active action.
                            if (m_Actions[i] == m_ActiveAction) m_ActiveAction = null;
                            m_ActiveActions[i] = null;
                            //  Move on to the next Action.
                            continue;
                        }
                    }
                    //  UPDATING
                    //  Can't stop or start action, so will update.
                    m_Actions[i].UpdateAction();
                    if (m_ActiveAction == m_Actions[i])
                    {
                        m_UpdateRotation = m_ActiveAction.UpdateRotation();
                        m_UpdateMovement = m_ActiveAction.UpdateMovement();
                        m_UpdateAnimator = m_ActiveAction.UpdateAnimator();
                    }
                }


            }


		}



        private void FixedUpdate()
        {
            _rigidbodyVelocity = m_Rigidbody.velocity;
            _rigidbodyAngularVelocity = m_Rigidbody.angularVelocity;
            if (m_UpdateRotation) UpdateRotation();
            if (m_UpdateMovement) UpdateMovement();
            if (m_UpdateAnimator) UpdateAnimator();
            //MoveCharacter(RelativeInputVector.x, RelativeInputVector.z, m_LookRotation);
        }






        private bool CheckGround()
        {
            Color _stepColor = Color.blue;

            // change the physics material to very slip when not grounded or maxFriction when is
            if (m_Grounded && m_InputVector == Vector3.zero)
                m_CapsuleCollider.material = m_GroundedIdleFrictionMaterial;
            else if (m_Grounded && m_InputVector != Vector3.zero)
                m_CapsuleCollider.material = m_GroundedMovingFrictionMaterial;
            else
                m_CapsuleCollider.material = m_AirFrictionMaterial;
            

            if(m_Grounded)
            {
                m_MoveDirection = m_Moving ? Vector3.Cross(m_Transform.right, m_GroundHit.normal) : Vector3.zero;
                m_SlopeAngle = Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90;
            }
            else{
                m_MoveDirection = Vector3.zero;
                m_SlopeAngle = 0;
            }

            Ray groundCheck = new Ray(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, Vector3.down);
            if(m_DrawDebugLine) Debug.DrawRay(m_Transform.position + Vector3.up * m_AlignToGroundDepthOffset, Vector3.down, Color.white);

            if (Physics.Raycast(groundCheck, out m_GroundHit, m_AlignToGroundDepthOffset + m_SkinWidth, m_Layers.SolidLayer))
            {
                var rayStart = (m_Transform.position + Vector3.up * m_MaxStepHeight) + m_Transform.forward * (m_CapsuleCollider.radius + m_SkinWidth);
                var rayEnd = Vector3.down * (m_MaxStepHeight - m_StepOffset);

                if (m_DrawDebugLine) Debug.DrawRay(rayStart, rayEnd, Color.yellow);

                if(m_InputVector.sqrMagnitude > 0.1f && m_Grounded)
                {
                    if (Physics.Raycast(rayStart, rayEnd, out m_StepHit, m_MaxStepHeight - m_StepOffset, m_Layers.SolidLayer))
                    {
                        if (m_StepHit.point.y >= (m_Transform.position.y) && m_StepHit.point.y <= (m_Transform.position.y + m_StepOffset + m_SkinWidth))
                        {
                            m_MoveDirection = (m_StepHit.point - m_Transform.position).normalized * m_StepSpeed * (m_Speed > 1 ? m_Speed : 1);
                            m_Rigidbody.velocity = m_MoveDirection; // * m_StepSpeed * (m_Speed > 1 ? m_Speed : 1);// + (Vector3.up * m_Step);

                            _stepColor = Color.magenta;
                        }
                        else
                        {
                            _stepColor = Color.cyan;
                        }

                    }
                }

                if (m_DrawDebugLine)
                    Debug.DrawRay( m_Transform.position, m_MoveDirection * m_StepSpeed * (m_Speed > 1 ? m_Speed : 1), _stepColor);

                return true;
            }


            return false;
        }



        //  Update the rotation forces.
        private void UpdateRotation()
        {
            //var angle = Mathf.Atan2(InputVector.x, Mathf.Abs(InputVector.z));
            //angle = Mathf.Rad2Deg * m_RotationSpeed;
            //angle += m_Transform.eulerAngles.y;
            //LookRotation = Quaternion.Euler(0, angle, 0);

            //m_LookRotation *= Quaternion.AngleAxis(m_RotationSpeed * m_InputVector.x * m_DeltaTime, Vector3.up);
            //m_Rigidbody.MoveRotation(m_LookRotation.normalized);

            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, m_LookRotation.normalized, m_RotationSpeed * m_DeltaTime));
        }


        //  Apply any movement.
        private void UpdateMovement()
        {
            //if (m_SlopeAngle >= m_SlopeLimit) return;


            if(m_Aiming)
            {
                m_Speed = Mathf.Clamp01(Mathf.Abs(m_InputVector.x) + Mathf.Abs(m_InputVector.z));
                m_Speed = Mathf.Clamp(m_Speed, 0, 1);
                if (m_Running)
                    m_Speed += 1f;
                //m_Speed = Mathf.Clamp(m_InputVector.z, -1, 1);
                //if (m_Running)
                    //m_Speed += 1f;
            }
            else{
                m_Speed = Mathf.Clamp01(Mathf.Abs(m_InputVector.x) + Mathf.Abs(m_InputVector.z));
                m_Speed = Mathf.Clamp(m_Speed, 0, 1);
                if (m_Running)
                    m_Speed += 1f;
            }


            if (m_UseRootMotion)
            {
                m_Velocity = (m_Animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_DeltaTime;
                m_Velocity.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, m_Acceleration * m_DeltaTime);
            }
            else
            {
                if(m_Aiming)
                {
                    m_Velocity = (m_Transform.TransformDirection(m_InputVector) * m_Speed);
                    m_Velocity.y = m_Rigidbody.velocity.y;
                    m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Velocity, m_Acceleration * m_DeltaTime);
                }
                else
                {
                    m_Velocity = m_MoveDirection + m_Transform.forward * m_Speed;
                    m_Rigidbody.velocity = m_Velocity;
                    m_Rigidbody.AddForce(m_MoveDirection * (m_Speed) * m_DeltaTime, ForceMode.VelocityChange);
                }

            }

            m_Moving = m_InputVector.sqrMagnitude > 0.1f;
        }



        private void UpdateAnimator()
        {
            m_AnimationMonitor.SetForwardInputValue(m_InputVector.z * m_Speed);
            m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x * m_Speed);
            m_Animator.SetFloat(HashID.Speed, m_Speed);
            m_Animator.SetBool(HashID.Moving, m_Moving);
        }






		public void MoveCharacter(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
        {
            m_InputVector.x = horizontalMovement;
            m_InputVector.y = m_Rigidbody.velocity.y;
            m_InputVector.z = forwardMovement;
            m_LookRotation = lookRotation;

            //if (Mathf.Abs(horizontalMovement) < 1 && Mathf.Abs(forwardMovement) < 1) return;

            m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, lookRotation.normalized, m_RotationSpeed * m_DeltaTime));
            if (m_UseRootMotion)
            {

                m_Velocity = m_Transform.forward * forwardMovement;
                m_Velocity += m_Transform.right * horizontalMovement;
                m_Velocity.Normalize();
            }
            else
            {
                m_Velocity = m_Transform.forward * forwardMovement * m_GroundSpeed.z;
                m_Velocity += m_Transform.right * horizontalMovement * m_GroundSpeed.x;
                m_Velocity.Normalize();
                m_Rigidbody.velocity = m_Velocity * m_RootMotionSpeedMultiplier;
            }



            //  Play Animation
            UpdateAnimator();
            //if(Aiming){
            //    m_AnimationMonitor.SetHorizontalInputValue(horizontalMovement);
            //}
        }




        public void StopMovement()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }


        private void OnAnimatorMove()
        {
            //if (m_UseRootMotion)
            //{
            //    //m_Rigidbody.velocity = (m_Animator.deltaPosition * m_RootMotionSpeedMultiplier) / m_DeltaTime;
            //    m_Rigidbody.velocity = m_Velocity;
            //    //Debug.Log(m_Animator.deltaPosition);
            //}

        }




        private void OnAimActionStart(bool aim)
        {
            m_Aiming = aim;
            OnAim(aim);
            //Debug.LogFormat("Aiming is {0}", aim);
        }


        private void OnActionActive(CharacterAction action, bool activated)
        {
            if(m_DebugActiveActions){
                if (action is Aim) return;
                Debug.Log(action + " activated: " + activated);
            }
        }







        public T GetAction<T>() where T : CharacterAction
        {
            return GetComponent<T>();
        }


        public bool TryStartAction(CharacterAction ability)
        {
            if (ability.CanStartAction())
            {
                ability.StartAction();
                return true;
            }
            return false;
        }



        public void TryStopAllActions()
        {

        }

        public void TryStopAction()
        {

        }

        public void ActionStopped()
        {

        }













		#region Debug 





		private Vector3 m_DebugHeightOffset = new Vector3(0, 0.25f, 0);

        protected void OnDrawGizmos()
        {
            //if (m_DrawDebugLine)
            //{
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, m_Velocity.normalized );
            //    //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, m_GroundSpeed + Vector3.up * heightOffset);

            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.forward );
            //    Gizmos.color = Color.yellow;
            //    Gizmos.DrawRay(transform.position + m_DebugHeightOffset, transform.right );
            //    //Gizmos.DrawRay(transform.position + Vector3.up * heightOffset, transform.right + Vector3.up * heightOffset);
            //}

            //if(Aiming && m_DrawDebugLine){
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawSphere(LookPosition, 0.1f);
            //    Gizmos.DrawLine(transform.position + (transform.up * 1.35f), LookPosition);
            //}
        }

        #endregion





    }

}
