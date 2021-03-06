﻿//namespace CharacterController
//{
//	using UnityEngine;
//	using System;

//	[DisallowMultipleComponent]
//	[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
//	public class RigidbodyCharacterController : MonoBehaviour
//	{
//		public enum MovementType { Adventure, Combat };

//		//  Locomotion variables
//		[SerializeField, HideInInspector]
//		protected bool m_UseRootMotion = true;
//		[SerializeField, HideInInspector]
//		protected float m_RootMotionSpeedMultiplier = 1;
//		[SerializeField, HideInInspector]
//		protected float m_GroundAcceleration = 0.12f;
//		[SerializeField, HideInInspector]
//		protected float m_AirborneAcceleration = 0.2f;                   //  NEED TO ADD
//		[SerializeField, HideInInspector]
//		protected float m_MotorDamping = 0.3f;
//		[SerializeField, HideInInspector]
//		protected float m_AirborneDamping = 0.3f;
//		[SerializeField, HideInInspector]
//		protected float m_GroundSpeed = 1f;
//		[SerializeField, HideInInspector]
//		protected float m_AirborneSpeed = 0.5f;                          //  NEED TO ADD
//		[SerializeField, HideInInspector]
//		protected float m_RotationSpeed = 10f;
//		[SerializeField, HideInInspector]
//		protected float m_SlopeForceUp = 1f;
//		[SerializeField, HideInInspector]
//		protected float m_SlopeForceDown = 1.25f;
//		[SerializeField, HideInInspector, Range(-1, 1)]
//		[Tooltip("A -1 to 1 threshold for when the character should stop moving if another object collides with the character. " +
//		"A value of 1 indicates the object is directly in front of the character's move direction while a value of -1 indicates the " +
//		"object is directly behind the character's move direction")]
//		protected float m_StopMovementThreshold = 0.5f;


//		//  -- Physics variables
//		[SerializeField, HideInInspector]
//		protected float m_DetectObjectHeight = 0.4f;
//		[SerializeField, HideInInspector]
//		protected float m_Mass = 100;
//		[SerializeField, HideInInspector]
//		protected float m_SkinWidth = 0.08f;
//		[SerializeField, HideInInspector, Range(0, 90)]
//		protected float m_SlopeLimit = 45f;
//		[SerializeField, HideInInspector]
//		protected float m_MaxStepHeight = 0.25f;
//		[SerializeField, HideInInspector]
//		protected float m_GroundStickiness = 6f;
//		[SerializeField, HideInInspector]
//		protected float m_ExternalForceDamping = 0.1f;
//		[SerializeField, HideInInspector, Range(0, 0.3f), Tooltip("Minimum height to consider a step.")]
//		protected float m_StepOffset = 0.15f;
//		[SerializeField, HideInInspector]
//		protected float m_StepSpeed = 4f;
//		[SerializeField, HideInInspector]
//		protected float m_GravityModifier = 2f;


//		//  -- Collision detection
//		[SerializeField, HideInInspector]
//		protected bool mDetectHorizontalCollision = true;
//		[SerializeField, HideInInspector]
//		protected int m_HorizontalCollisionCount = 10;
//		[SerializeField, HideInInspector]
//		protected int m_VerticalCollisionCount = 10;
//		[SerializeField, HideInInspector]
//		protected LayerMask m_ColliderLayerMask;
//		[SerializeField, HideInInspector]
//		protected int m_MaxCollisionCount = 50;


//		//  -- Animation
//		[Header("Animations")]
//		[SerializeField, HideInInspector]
//		protected float m_IdleRotationMultiplier = 2f;
//		[SerializeField, HideInInspector]
//		protected string m_MovingStateName = "Movement.Movement";
//		[SerializeField, HideInInspector]
//		protected string m_AirborneStateName = "Fall";



//		protected int m_TimeScale = 1;
//		//  Internal variable to to notify if Move() was called from outside, so it doesn't get called twice.
//		protected bool m_UpdateMove = true;
//		protected RaycastHit[] m_Collisions;





//		protected MovementType m_MovementType = MovementType.Adventure;
//		[SerializeField, DisplayOnly]
//		protected bool m_Moving;
//		protected bool m_Aiming;
//		protected float m_InputAngle;
//		protected Vector3 m_Velocity, m_PreviousVelocity;
//		[SerializeField, DisplayOnly]
//		protected Vector3 m_InputVector;
//		protected Quaternion m_LookRotation;



//		[SerializeField, DisplayOnly]
//		protected bool m_Grounded = true;
//		protected float m_GroundAngle;
//		protected Vector3 m_Gravity;
//		protected RaycastHit m_GroundHit;
//		protected float m_GroundDistance;
//		protected float m_AirbornThreshold = 0.3f;
//		protected float m_SlopeAngle;
//		protected float m_Stickiness;


//		protected CapsuleCollider m_CapsuleCollider;
//		protected Collider[] m_LinkedColliders = new Collider[0];
//		protected PhysicMaterial m_GroundIdleFrictionMaterial;
//		protected PhysicMaterial m_GroundedMovingFrictionMaterial;
//		protected PhysicMaterial m_StepFrictionMaterial;
//		protected PhysicMaterial m_SlopeFrictionMaterial;
//		protected PhysicMaterial m_AirFrictionMaterial;

//		protected bool m_CheckGround = true;
//		protected bool m_CheckMovement = true;
//		protected bool m_SetPhysicsMaterial = true;
//		protected bool m_UpdateRotation = true;
//		protected bool m_UpdateMovement = true;
//		protected bool m_UpdateAnimator = true;
//		protected bool m_Move = true;


//		private float m_StartAngle, m_StartAngleSmooth;


//		private Vector3 m_VelocitySmooth;
//		private float m_RotationSmoothDamp;




//		protected Animator m_Animator;
//		protected AnimatorMonitor m_AnimationMonitor;
//		protected LayerManager m_Layers;
//		protected Rigidbody m_Rigidbody;
//		protected GameObject m_GameObject;
//		protected Transform m_Transform;
//		protected float m_DeltaTime;
//		protected float deltaTime;
//		protected float fixedDeltaTime;
//		//  For Editor.
//		//  Debug parameters.
//		public bool DebugMode { get { return m_Debug; } set { m_Debug = value; } }
//		[SerializeField, HideInInspector]
//		protected bool m_Debug;
//		[SerializeField, HideInInspector]
//		protected bool m_DebugCollisions;
//		[SerializeField, HideInInspector]
//		protected bool m_DrawDebugLine;
//		[SerializeField, HideInInspector]
//		protected bool displayMovement = true, displayPhysics = true, displayAnimations = true, displayActions = true;






//		#region Properties

//		public MovementType Movement
//		{
//			get { return m_MovementType; }
//		}

//		public bool Moving
//		{
//			get { return m_Moving; }
//			set { m_Moving = value; }
//		}

//		public bool Aiming
//		{
//			get
//			{
//				if (m_Aiming && Grounded)
//					return true;
//				return false;
//			}
//			set { m_Aiming = value; }
//		}

//		public bool Grounded
//		{
//			get { return m_Grounded; }
//			set { m_Grounded = value; }
//		}

//		public float RotationSpeed
//		{
//			get { return m_RotationSpeed; }
//			set { m_RotationSpeed = value; }
//		}

//		public Vector3 InputVector
//		{
//			get { return m_InputVector; }
//			set
//			{
//				m_InputVector = value;
//				if (m_InputVector.sqrMagnitude > 1)
//					m_InputVector.Normalize();
//			}
//		}

//		public Vector3 Velocity
//		{
//			get { return m_Velocity; }
//			set { m_Velocity = value; }
//		}

//		public Quaternion LookRotation
//		{
//			get { return m_LookRotation; }
//			set { m_LookRotation = value; }
//		}

//		public bool UseRootMotion
//		{
//			get { return m_UseRootMotion; }
//			set { m_UseRootMotion = value; }
//		}

//		public Vector3 RaycastOrigin
//		{
//			get { return m_Transform.position + Vector3.up * m_SkinWidth; }
//		}

//		public RaycastHit GroundHit
//		{
//			get { return m_GroundHit; }
//		}



//		#endregion



//		protected virtual void Awake()
//		{
//			m_AnimationMonitor = GetComponent<AnimatorMonitor>();
//			m_Animator = GetComponent<Animator>();

//			m_Rigidbody = GetComponent<Rigidbody>();
//			if (m_CapsuleCollider == null)
//				m_CapsuleCollider = GetComponent<CapsuleCollider>();
//			m_Layers = GetComponent<LayerManager>();

//			m_GameObject = gameObject;
//			m_Transform = transform;

//			m_DeltaTime = Time.deltaTime;

//			if (m_Layers == null)
//				m_Layers = m_GameObject.AddComponent<LayerManager>();

//			m_Gravity = Physics.gravity;

//			deltaTime = Time.deltaTime;
//			fixedDeltaTime = Time.fixedDeltaTime;
//		}



//		protected virtual void OnEnable()
//		{
//			m_CapsuleCollider.enabled = true;
//		}


//		protected virtual void OnDisable()
//		{
//			m_CapsuleCollider.enabled = false;
//		}


//		protected void Start()
//		{
//			m_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
//			m_Rigidbody.mass = m_Mass;

//			m_Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
//			m_Animator.applyRootMotion = m_UseRootMotion;

//			m_ColliderLayerMask = m_Layers.SolidLayers;

//			m_Collisions = new RaycastHit[m_MaxCollisionCount];



//			// slides the character through walls and edges
//			m_GroundedMovingFrictionMaterial = new PhysicMaterial
//			{
//				name = "GroundedMovingFrictionMaterial",
//				staticFriction = .25f,
//				dynamicFriction = .25f,
//				frictionCombine = PhysicMaterialCombine.Multiply
//			};

//			// prevents the collider from slipping on ramps
//			m_GroundIdleFrictionMaterial = new PhysicMaterial
//			{
//				name = "GroundIdleFrictionMaterial",
//				staticFriction = 1f,
//				dynamicFriction = 1f,
//				frictionCombine = PhysicMaterialCombine.Maximum
//			};

//			// air physics 
//			m_AirFrictionMaterial = new PhysicMaterial
//			{
//				name = "AirFrictionMaterial",
//				staticFriction = 0f,
//				dynamicFriction = 0f,
//				frictionCombine = PhysicMaterialCombine.Minimum
//			};
//		}


//		protected virtual void Update()
//		{
//			m_TimeScale = Mathf.RoundToInt(Time.timeScale);
//			if (m_TimeScale == 0) return;
//			m_DeltaTime = deltaTime;

//		}



//		protected virtual void FixedUpdate()
//		{
//			if (m_TimeScale == 0) return;
//			m_DeltaTime = fixedDeltaTime;

//			if (m_Move) Move();

//			if (m_CheckGround) CheckGround();

//			if (m_CheckMovement) CheckMovement();

//			if (m_SetPhysicsMaterial) SetPhysicsMaterial();

//			if (m_UpdateRotation) UpdateRotation();

//			if (m_UpdateMovement) UpdateMovement();

//			if (m_UpdateAnimator) UpdateAnimator();
//		}


//		private void LateUpdate()
//		{
//			m_PreviousVelocity = m_Velocity;

//			CharacterDebug.Log("m_InputAngle", m_InputAngle);
//			CharacterDebug.Log("m_SlopeAngle", m_SlopeAngle);
//			CharacterDebug.Log("m_Velocity", m_Velocity);

//			CharacterDebug.Log("rb_AngularVelocity", m_Rigidbody.angularVelocity);
//			CharacterDebug.Log("rb_Velocity", m_Rigidbody.velocity);
//		}


//		protected virtual void OnAnimatorMove()
//		{
//			// The anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
//			// which affects the movement speed because of the root motion.
//			m_Animator.speed = m_RootMotionSpeedMultiplier;

//		}






//		public void SetMovementType(MovementType movementType)
//		{
//			m_MovementType = movementType;
//		}


//		//  If true, the character looks independently of the camera.  AI Agents do not need to use camera rotation.
//		public bool IndependentLook()
//		{
//			if (m_Moving || m_Aiming)
//			{
//				return false;
//			}
//			//if(m_Aiming) return true;
//			return true;
//		}





//		protected virtual void Move()
//		{


//			m_Velocity = m_Transform.TransformDirection(m_InputVector);
//			//m_Velocity = Quaternion.Inverse(m_Transform.rotation) * m_InputVector * m_DeltaTime;

//			//m_Velocity.x *= (m_Grounded ? m_GroundSpeed : m_AirborneSpeed);
//			//m_Velocity.z *= (m_Grounded ? m_GroundSpeed : m_AirborneSpeed);

//			Debug.DrawRay(RaycastOrigin, m_Velocity, Color.green);
//		}







//		protected virtual void CheckGround()
//		{
//			float groundDistance = 10f;
//			float checkHeight = m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2 + m_SkinWidth;
//			Vector3 rayOrigin = m_Transform.position + Vector3.up * checkHeight;
//			// Reduce our radius by Tolerance squared to avoid failing the SphereCast due to clipping with walls
//			float smallerRadius = m_CapsuleCollider.radius - (m_SkinWidth * m_SkinWidth);
//			float rayDist = m_CapsuleCollider.radius;
//			//rayDist = 1;
//			//  GroundHit Normal is used by the Character footstep.
//			//RaycastHit hit;
//			if (Physics.Raycast(rayOrigin, Vector3.down, out m_GroundHit, rayDist, m_Layers.SolidLayers))
//			{
//				groundDistance = Vector3.Project(m_Rigidbody.position - m_GroundHit.point, m_Transform.up).magnitude;

//			}
//			if (DebugMode) Debug.DrawRay(rayOrigin, Vector3.down * rayDist, m_Grounded ? Color.green : Color.red);

//          Vector3 sphereCastOrigin = m_Rigidbody.position + Vector3.up * m_CapsuleCollider.radius;
//			if (Physics.SphereCast(sphereCastOrigin, smallerRadius, Vector3.down, out m_GroundHit, rayDist, m_Layers.SolidLayers))
//			{
//				// check if sphereCast distance is small than the ray cast distance
//				if (groundDistance > (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f))
//					groundDistance = (m_GroundHit.distance - m_CapsuleCollider.radius * 0.1f);

//				if (DebugMode) DebugDraw.Sphere(sphereCastOrigin + Vector3.down * m_GroundHit.distance, smallerRadius, m_Grounded ? Color.green : Color.red);
//			}

//			if (m_GroundHit.transform == null) Debug.Log(" Ground hit did not hit anything");
//			else Debug.Log(m_GroundHit.transform);

//			groundDistance = (float)Math.Round(groundDistance, 2);
//			var groundCheckDistance = 0.2f;

//			//  Character is grounded.
//			if (m_GroundDistance < 0.05f)
//			{
//				Vector3 horizontalVelocity = Vector3.Project(m_Velocity, m_Gravity);
//				m_Stickiness = m_GroundStickiness * horizontalVelocity.magnitude * m_AirbornThreshold;
//				m_GroundAngle = Vector3.Angle(m_Transform.forward, m_GroundHit.normal) - 90;

//				//m_Velocity = Vector3.ProjectOnPlane(m_Velocity, m_GroundHit.normal * m_Stickiness);

//				m_Grounded = true;
//			}
//			else
//			{
//				if (groundDistance >= groundCheckDistance)
//				{
//					m_GroundAngle = 0;
//					m_Grounded = false;
//				}

//				//if (m_Rigidbody.velocity.y > -0.01f || m_Rigidbody.velocity.y <= 0f)
//				//{
//				//    m_Grounded = false;
//				//}
//			}


//		}


//		//  Ensure the current movement direction is valid.
//		protected virtual void CheckMovement()
//		{
//			m_Moving = m_InputVector != Vector3.zero;
//			if (m_Moving == false) return;


//			float direction = Mathf.Clamp(m_InputVector.z, -1, 1);
//			if (direction < 0.01f && direction > 0.01f) direction = 1;
//			float rayLength = 2f + m_SkinWidth;
//			float dstBetweenRays = 0.4f;

//			float colliderRadius = m_CapsuleCollider.radius - m_SkinWidth;
//			float colliderHeight = m_CapsuleCollider.height - (colliderRadius * 2);
//			int horizontalRayCount = mDetectHorizontalCollision ? Mathf.RoundToInt(colliderHeight / dstBetweenRays) : 3;
//			float horizontalRaySpacing = colliderHeight / (horizontalRayCount - 1);


//			bool hitDetected = false;
//			RaycastHit hit;
//			for (int i = 0; i < horizontalRayCount; i++)
//			{
//				Vector3 rayOrigin = m_Rigidbody.position;
//				rayOrigin += Vector3.up * colliderRadius;
//				rayOrigin += m_Transform.forward * colliderRadius;
//				rayOrigin += Vector3.up * (horizontalRaySpacing * i);

//				if (Physics.Raycast(rayOrigin, m_Transform.forward * direction, out hit, rayLength, m_ColliderLayerMask))
//				{
//					float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

//					if (i == 0 && slopeAngle <= m_SlopeLimit)
//					{
//						float distanceToSlope = 0;
//						if (Math.Abs(slopeAngle - m_GroundAngle) > float.Epsilon)
//						{
//							distanceToSlope = hit.distance - colliderRadius;
//						}

//						Vector3 slopeDirection = Vector3.Cross(m_Transform.right, hit.normal).normalized;
//						//slopeDirection = slopeDirection - slopeDirection * distanceToSlope;
//						Vector3 targetVelocity = m_Velocity;
//						targetVelocity = Vector3.Project(targetVelocity, slopeDirection);

//						//if (DebugMode) Debug.DrawRay(rayOrigin, targetVelocity, Color.blue);
//						if (DebugMode) DebugDraw.Arrow(rayOrigin, targetVelocity, Color.blue);
//						//m_Velocity = targetVelocity;
//					}

//					if (slopeAngle > m_SlopeLimit)
//					{
//						rayLength = hit.distance;

//						m_Moving = false;
//					}
//					hitDetected = true;

//					//m_Velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(m_Velocity.x) * Mathf.Sign(m_Velocity.x);
//					//m_Velocity.z = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(m_Velocity.z) * Mathf.Sign(m_Velocity.z);

//				}


//				if (DebugMode) Debug.DrawRay(rayOrigin, m_Transform.forward * direction * rayLength, hitDetected == true ? Color.blue : Color.grey);
//			}


//		}





//		//  Update the rotation forces.
//		protected virtual void UpdateRotation()
//		{
//			//float targetAngle = Quaternion.Angle(m_Transform.rotation, m_LookRotation);
//			float targetAngle = Mathf.Atan2(m_InputVector.x, m_InputVector.z) * Mathf.Rad2Deg;
//			CharacterDebug.Log("targetAngle", targetAngle);

//			//Vector3 lookDirection = m_LookRotation * m_Transform.forward;
//			targetAngle *= m_RotationSpeed * m_DeltaTime;
//			Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, m_Transform.up);
//			//targetRotation = Quaternion.Slerp(m_Rigidbody.rotation, targetRotation, m_RotationSpeed * m_DeltaTime);
//			m_Rigidbody.MoveRotation(targetRotation * m_Rigidbody.rotation);


//			if (m_UseRootMotion)
//			{
//				float angleInDegrees;
//				Vector3 rotationAxis;
//				m_Animator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
//				Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad * m_RotationSpeed;
//				m_Rigidbody.angularVelocity = angularDisplacement;

//				CharacterDebug.Log("m_Animator.deltaRotation", m_Animator.deltaRotation);
//			}
//		}



//		//  Apply any movement.
//		protected virtual void UpdateMovement()
//		{
//			if (m_UseRootMotion)
//			{
//				m_Velocity = (m_Animator.deltaPosition / m_DeltaTime);
//				//m_Velocity = m_Velocity.normalized * (m_Grounded ? m_GroundSpeed : m_AirborneSpeed);
//			}
//			else
//			{
//				if (m_Moving)
//					m_Velocity = Vector3.SmoothDamp(m_Rigidbody.velocity, m_Velocity, ref m_VelocitySmooth,
//						m_Grounded ? m_GroundAcceleration : m_AirborneAcceleration);
//				else
//					m_Velocity = Vector3.SmoothDamp(m_Velocity, Vector3.zero, ref m_VelocitySmooth,
//						m_Grounded ? m_MotorDamping : m_AirborneDamping);
//			}


//			if (m_Grounded)
//			{

//			}
//			//  If character is airborne.
//			else
//			{
//				//  Vertical velocity;
//				if (m_Rigidbody.velocity.y < 0.01f)
//				{
//					m_Velocity.y += m_Gravity.y * m_GravityModifier * m_DeltaTime;
//				}
//				else if (m_Rigidbody.velocity.y > 0.01f)
//				{
//					m_Velocity.y += m_Gravity.y * m_DeltaTime;
//				}
//			}



//			m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
//			//m_Velocity.y = m_Grounded ? 0 : m_Rigidbody.velocity.y;
//			m_Rigidbody.velocity = m_Velocity;

//			//if (m_Grounded)
//			//{
//			//    m_Rigidbody.velocity = Vector3.ProjectOnPlane(m_Rigidbody.velocity, m_GroundNormal * m_Stickiness);
//			//}


//		}








//		protected void UpdateAnimator()
//		{
//			//m_Animator.SetFloat(HashID.Rotation, (m_InputAngle * Mathf.Deg2Rad));
//			//  1 means left foot is up.
//			m_Animator.SetFloat(HashID.StartAngle, m_StartAngle);
//			m_Animator.SetFloat(HashID.Rotation, m_InputAngle);
//			m_Animator.SetFloat(HashID.LegUpIndex, m_Animator.pivotWeight);
//			m_Animator.SetBool(HashID.Moving, m_Moving);

//			//  -----------
//			//  Does a character action virtual the controllers update animator.
//			//  -----------
//			if (m_Grounded)
//			{
//				//  Movement Input
//				m_AnimationMonitor.SetForwardInputValue(m_InputVector.z);
//				m_AnimationMonitor.SetHorizontalInputValue(m_InputVector.x);
//			}
//			else
//			{
//				m_Animator.SetFloat(HashID.ForwardInput, 0);
//				m_Animator.SetFloat(HashID.HorizontalInput, 0);
//			}
//		}



//		protected virtual void SetPhysicsMaterial()
//		{
//			// change the physics material to very slip when not grounded or maxFriction when is
//			if (m_Grounded && !m_Moving)
//				m_CapsuleCollider.material = m_GroundIdleFrictionMaterial;
//			else if (m_Grounded && m_Moving)
//				m_CapsuleCollider.material = m_GroundedMovingFrictionMaterial;
//			else
//				m_CapsuleCollider.material = m_AirFrictionMaterial;
//		}




//		//protected void OnCollisionEnter(Collision collision)
//		//{
//		//    //for (int i = 0; i < collision.contacts.Length; i++)
//		//    //{
//		//    //    ContactPoint contact = collision.contacts[i];
//		//    //    if(DebugMode) Debug.DrawRay(contact.point, contact.normal, Color.white, 1.25f);
//		//    //}
//		//}




//		#region Public Functions


//		public virtual void Move(float horizontalMovement, float forwardMovement, Quaternion lookRotation)
//		{


//			switch (m_MovementType)
//			{
//				case (MovementType.Adventure):

//					m_InputVector.x = Mathf.Clamp(horizontalMovement, -1, 1);
//					m_InputVector.y = 0;
//					m_InputVector.z = Mathf.Clamp(forwardMovement, -1, 1);

//					float turnAmount = Mathf.Atan2(m_InputVector.x, m_InputVector.z);
//					//m_Velocity.x = turnAmount;
//					m_LookRotation = lookRotation;
//					break;

//				case (MovementType.Combat):

//					m_InputVector.x = Mathf.Clamp(horizontalMovement, -1, 1);
//					m_InputVector.y = 0;
//					m_InputVector.z = Mathf.Clamp(forwardMovement, -1, 1);

//					//float turnAmount = Mathf.Atan2(m_InputVector.x, m_InputVector.z);
//					m_LookRotation = lookRotation;
//					break;
//			}



//			//   ---
//		}



//		protected void VerticalCollision()
//		{
//			//  We want the cos angle since we know "adjacent" and "hypotenuse".
//			float startAngle = Mathf.Acos(m_StopMovementThreshold) * Mathf.Rad2Deg;
//			float checkHeight = m_CapsuleCollider.height / 2;
//			float rayDistance = 2;
//			float verticalAngle = 15;

//			Quaternion startRayRotation = Quaternion.AngleAxis(-startAngle, Vector3.up);
//			Vector3 startRay = startRayRotation * m_Transform.forward;
//			Vector3 startDir = startRay;

//			float angleAmount = (startAngle * 2) / (m_VerticalCollisionCount - 1);

//			bool detectEdge = false;

//			Vector3 raycastOrigin = m_Rigidbody.position + Vector3.up * checkHeight;

//			for (int i = 0; i < m_VerticalCollisionCount; i++)
//			{
//				if (i == 0) continue;
//				startDir = Quaternion.AngleAxis(angleAmount, Vector3.up) * startDir;
//				Vector3 hitDirection = startDir;

//				Quaternion angleRotation = Quaternion.AngleAxis(verticalAngle, m_Transform.right);
//				hitDirection = Vector3.ClampMagnitude(angleRotation * hitDirection, rayDistance);


//				if (Physics.Raycast(raycastOrigin, hitDirection + Vector3.up * checkHeight, rayDistance, m_ColliderLayerMask) == false)
//				{

//					Vector3 start = m_Rigidbody.position + (m_Transform.forward * m_CapsuleCollider.radius);
//					start.y += m_DetectObjectHeight;
//					float maxDetectEdgeDistance = 1 + m_DetectObjectHeight;
//					if (Physics.Raycast(start, Vector3.down, maxDetectEdgeDistance, m_ColliderLayerMask) == false)
//					{
//						detectEdge = true;
//					}
//				}

//				if (m_DebugCollisions) Debug.DrawRay(raycastOrigin, hitDirection * rayDistance, detectEdge == true ? Color.red : Color.grey);
//			}


//		}



//		public bool DetectEdge()
//		{
//			if (!m_Grounded)
//				return false;


//			bool detectEdge = false;
//			Vector3 start = m_Rigidbody.position + (m_Transform.forward * m_CapsuleCollider.radius);
//			start.y = start.y + m_DetectObjectHeight;
//			//start.y = start.y + 0.05f + (Mathf.Tan(m_SlopeAngle) * start.magnitude);

//			Vector3 dir = Vector3.down;
//			float maxDetectEdgeDistance = 1 + m_DetectObjectHeight;



//			if (Physics.Raycast(m_Rigidbody.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward, 2, m_Layers.SolidLayers) == false)
//			{

//				if (Physics.Raycast(start, dir, maxDetectEdgeDistance, m_Layers.SolidLayers) == false)
//				{
//					detectEdge = true;
//				}
//			}



//			//if (Debug && hitObject == false) Debug.DrawRay(m_Rigidbody.position + (Vector3.up * m_DetectObjectHeight), m_Transform.forward * 2, hitObject ? Color.red : Color.green);
//			if (DebugMode) Debug.DrawRay(start, dir * maxDetectEdgeDistance, detectEdge ? Color.green : Color.gray);

//			return detectEdge;
//		}



//		public bool DetectObject(Vector3 dir, out RaycastHit raycastHit, float maxDistance, LayerMask layerMask, int rayCount = 1, float maxAngle = 0)
//		{
//			bool detectObject = false;
//			Vector3 start = m_Rigidbody.position + (Vector3.up * m_DetectObjectHeight);

//			if (rayCount < 1) rayCount = 1;

//			if (Physics.Raycast(start, dir, out raycastHit, maxDistance, layerMask))
//			{
//				detectObject = true;
//			}

//			if (DebugMode) Debug.DrawRay(start, dir * maxDistance, detectObject ? Color.red : Color.green);
//			return detectObject;
//		}









//		public void SetPosition(Vector3 position)
//		{
//			m_Rigidbody.MovePosition(position);
//		}


//		public void SetRotation(Quaternion rotation)
//		{
//			m_Rigidbody.MoveRotation(rotation.normalized);
//		}


//		public void StopMovement()
//		{
//			m_Rigidbody.velocity = Vector3.zero;
//			m_Moving = false;
//		}


//		#endregion













//		protected string[] debugMsgs;
//		//Camera mainCamera;
//		protected Vector3 debugHeightOffset = new Vector3(0, 0.25f, 0);
//		protected Color _Magenta = new Color(0.75f, 0, 0.75f, 0.9f);




//		protected virtual void DrawGizmos()
//		{

//		}


//		protected virtual void DrawOnGUI()
//		{
//			GUI.color = CharacterControllerUtility.DebugTextColor;
//			Rect rect = CharacterControllerUtility.CharacterControllerRect;
//			GUI.BeginGroup(rect, GUI.skin.box);

//			GUI.Label(rect, CharacterDebug.Write());

//			GUI.EndGroup();
//		}



//		protected void OnGUI()
//		{
//			if (Application.isPlaying && DebugMode && Time.timeScale != 0)
//			{
//				DrawOnGUI();
//			}

//		}

//		protected void OnDrawGizmos()
//		{
//			if (DebugMode && Application.isPlaying)
//			{
//				DrawGizmos();
//			}

//		}


//	}

//}
