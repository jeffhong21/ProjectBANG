//namespace CharacterController
//{
//    using UnityEngine;
//    using System;
//    using System.Collections.Generic;

//    [RequireComponent(typeof(CharacterLocomotion))]
//    public class CharacterIK_ORIGINAL : MonoBehaviour
//    {
//        private readonly float m_LookAtRayLength = 30;


//        [SerializeField] private bool m_DebugDrawLookRay;
//        [SerializeField] private int m_BaseLayerIndex = 0;
//        [SerializeField] private int m_UpperBodyLayerIndex = 1;
//        [SerializeField] private LayerMask m_LayerMask;

//        [Header("-- Body --")]
//        [SerializeField, Range(0, 1)] protected float m_LookAtAimBodyWeight = 1f;
//        [SerializeField, Range(0, 1)] protected float m_LookAtBodyWeight = 0.05f;
//        [SerializeField, Range(0, 1)] protected float m_LookAtHeadWeight = 0.425f;
//        [SerializeField, Range(0, 1)] protected float m_LookAtEyesWeight = 1f;
//        [SerializeField, Range(0, 1)] protected float m_LookAtClampWeight = 0.35f;
//        [SerializeField] protected float m_LookAtAdjustmentSpeed = 0.2f;
//        [SerializeField] private Vector3 m_LookAtOffset;

//        [Header("-- Hands --")]
//        [SerializeField, Range(0, 1)] protected float m_HandIKWeight = 1f;
//        [SerializeField] protected float m_HandIKAdjustmentSpeed = 0.2f;
//        [SerializeField] protected Vector3 m_HandIKOffset;

//        [Header("States")]
//        [SerializeField] protected bool m_EnableHandIK;



//        private Animator m_Animator;
//        private CharacterLocomotion m_Controller;
//        private LayerManager m_LayerManager;
//        private Transform m_Transform;
//        private GameObject m_GameObject;


//        private float m_TargetLookAtWeight;
//        private float m_TargetHandWeight;
//        private float m_HandIKAdjustmentVelocity;
//        private float m_LookAtAdjustmentVelocity;

//        private Vector3 rightHandPosition, leftHandPosition, rightHandIkPosition, leftHandIkPosition;


//        private Transform m_LookAtPoint;
//        //private Vector3 m_LookAtDirection;

//        private Transform m_RightHand, m_LeftHand, m_UpperChest, m_RightShoulder;
//        private Transform m_AimPivot;
//        [SerializeField] private Transform m_RightHandTarget;
//        [SerializeField] private Item m_CurrentItem;


//        private Vector3 m_TargetDirection;
//        private Quaternion m_TargetRotation;



//        [SerializeField]
//        Transform itemSlot;




//        private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
//        private Quaternion leftFootIkRotation, rightFootIkRotation;
//        private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

//        [Header("Feet Grounder")]
//        public bool enableFeetIk = true;
//        [Range(0, 2)]
//        [SerializeField]
//        private float heightFromGroundRaycast = 1.14f;
//        [Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f;
//        [SerializeField] private float pelvisOffset = 0f;
//        [Range(0, 1)] [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;  //  m_HipsMovingPositionAdjustmentSpeed
//        [Range(0, 1)] [SerializeField] private float feetToIkPositionSpeed = 0.5f;  //  m_FootPositionAdjustmentSpeed

//        public string leftFootAnimVariableName = "LeftFootCurve";
//        public string rightFootAnimVariableName = "RightFootCurve";

//        public bool useProIkFeature = false;
//        public bool showSolverDebug = true;




//        private void Awake()
//        {
//            m_Animator = GetComponent<Animator>();
//            m_Controller = GetComponent<CharacterLocomotion>();
//            m_Transform = transform;
//            m_GameObject = gameObject;



//            m_RightHand = m_Animator.GetBoneTransform(HumanBodyBones.RightHand).transform;
//            m_LeftHand = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand).transform;
//            m_UpperChest = m_Animator.GetBoneTransform(HumanBodyBones.UpperChest).transform;
//            m_RightShoulder = m_Animator.GetBoneTransform(HumanBodyBones.RightShoulder).transform;


//            m_LookAtPoint = new GameObject("Look At Point").transform; //.parent = gameObject.transform;
//            m_LookAtPoint.transform.parent = gameObject.transform;
//            m_LookAtPoint.position = (m_Transform.forward * m_LookAtRayLength) + (Vector3.up * m_UpperChest.position.y);
//            //var _position = new Vector3(1, 0, 0);
//            m_LookAtPoint.localPosition = m_LookAtPoint.localPosition;// + _position;

//            m_AimPivot = new GameObject("Aim Pivot").transform;
//            m_AimPivot.transform.parent = gameObject.transform;
//            m_AimPivot.position = m_RightShoulder.position;




//            //ItemEquipSlot[] itemSlots = GetComponentsInChildren<ItemEquipSlot>();
//            //for (int i = 0; i < itemSlots.Length; i++)
//            //{
//            //    if (itemSlots[i].ID == 0) m_RightHandTarget = itemSlots[i].transform;
//            //    //if (itemSlots[i].ID == 0) itemSlot = itemSlots[i].transform;
//            //}

//            m_RightHandTarget = new GameObject("Right Hand Target").transform;
//            m_RightHandTarget.transform.parent = m_AimPivot;
//            m_RightHandTarget.position = Vector3.zero;
//        }




//        private void OnEnable()
//        {
//            EventHandler.RegisterEvent<Item>(m_GameObject, "OnInventoryEquip", HandleItem);
//            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAim);
//        }


//        private void OnDisable()
//        {
//            EventHandler.UnregisterEvent<Item>(m_GameObject, "OnInventoryEquip", HandleItem);
//            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAim);
//        }



//        private void FixedUpdate()
//        {
//            m_AimPivot.position = m_RightShoulder.position;
//            m_TargetDirection = m_LookAtPoint.position - m_AimPivot.position;
//            if (m_TargetDirection == Vector3.zero)
//                m_TargetDirection = m_AimPivot.forward;

//            m_TargetRotation = Quaternion.LookRotation(m_TargetDirection);
//            m_AimPivot.rotation = Quaternion.Slerp(m_AimPivot.rotation, m_TargetRotation, Time.deltaTime * 15);


//            if (enableFeetIk == false) { return; }
//            if (m_Animator == null) { return; }

//            AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
//            AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

//            //find and raycast to the ground to find positions
//            FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation); // handle the solver for right foot
//            FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation); //handle the solver for the left foot

//        }


//        private void Update()
//        {
//            if (m_Controller.Aiming)
//            {
//                m_LookAtPoint.position = m_Controller.LookPosition;
//            }
//            else
//            {
//                //m_LookAtPoint.position.Set()
//            }
//        }



//        private void OnAnimatorIK()
//        {
//            LookAtTarget();
//            RotateDominantHand();
//            RotateNonDominantHand();
//            PositionHands();


//            if (enableFeetIk == false) { return; }
//            if (m_Animator == null) { return; }

//            MovePelvisHeight();

//            //right foot ik position and rotation -- utilise the pro features in here
//            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
//            if (useProIkFeature)
//            {
//                m_Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, m_Animator.GetFloat(rightFootAnimVariableName));
//            }
//            MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);


//            //left foot ik position and rotation -- utilise the pro features in here
//            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
//            if (useProIkFeature)
//            {
//                m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, m_Animator.GetFloat(leftFootAnimVariableName));
//            }
//            MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
//        }




//        private void HandleItem(Item item)
//        {
//            //Debug.LogFormat("{0} currently equipped item is {1}.", gameObject.name, item == null ? "<null>" : item.ItemAnimName);
//            if (item == null)
//            {
//                m_CurrentItem = null;
//                m_RightHandTarget.localPosition = m_RightHand.localPosition;
//                m_RightHandTarget.localEulerAngles = m_RightHand.localEulerAngles;
//            }
//            else
//            {
//                m_CurrentItem = item;
//                //m_RightHandTarget.localPosition = item.HandIKPosition.position;
//                //m_RightHandTarget.localEulerAngles = item.HandIKPosition.rotation;
//                m_RightHandTarget.localPosition = item.PositionOffset;
//                m_RightHandTarget.localEulerAngles = item.RotationOffset;
//            }
//        }


//        private void OnAim(bool aim)
//        {
//            //Debug.LogFormat("{0} aiming is {1}.", gameObject.name, aim);
//            if (aim)
//            {
//                m_LookAtAimBodyWeight = 1;
//                m_HandIKWeight = m_CurrentItem == null ? 0 : 1;
//            }
//            else
//            {
//                m_LookAtAimBodyWeight = 0;
//                m_HandIKWeight = 0;
//            }
//        }





//        protected virtual void LookAtTarget()
//        {
//            //Vector3 directionTowardsTarget = m_LookAtPoint.position - m_Transform.position;
//            //float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);
//            //if (angle < 76)
//            //    m_LookAtAimBodyWeight = 1;
//            //else
//            //    m_LookAtAimBodyWeight = 0;


//            //m_HandIKWeight = Mathf.SmoothDamp(m_HandIKWeight, m_TargetHandWeight, ref m_HandIKAdjustmentVelocity, m_HandIKAdjustmentSpeed);
//            m_TargetHandWeight = Mathf.SmoothDamp(m_TargetHandWeight, m_HandIKWeight, ref m_HandIKAdjustmentVelocity, m_HandIKAdjustmentSpeed);
//            //m_LookAtAimBodyWeight = Mathf.SmoothDamp(m_LookAtAimBodyWeight, m_TargetLookAtWeight, ref m_LookAtAdjustmentVelocity, m_LookAtAdjustmentSpeed);
//            m_TargetLookAtWeight = Mathf.SmoothDamp(m_TargetLookAtWeight, m_LookAtAimBodyWeight, ref m_LookAtAdjustmentVelocity, m_LookAtAdjustmentSpeed);


//            m_Animator.SetLookAtWeight(m_TargetLookAtWeight, m_LookAtBodyWeight, m_LookAtHeadWeight, m_LookAtEyesWeight, m_LookAtClampWeight);
//            m_Animator.SetLookAtPosition(m_LookAtPoint.position + m_LookAtOffset);
//        }



//        protected virtual void RotateDominantHand()
//        {
//            //Vector3 direction = m_LookAtPoint.position - m_RightHandTarget.position;
//            //Quaternion rotation = Quaternion.FromToRotation(m_RightHandTarget.forward, direction);
//            //rotation *= Quaternion.Euler(0, 0, -90);
//            //m_RightHandTarget.rotation = Quaternion.Slerp(m_Transform.rotation, rotation, Time.deltaTime * m_HandIKAdjustmentSpeed);


//            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_TargetHandWeight);
//            m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_RightHandTarget.rotation);
//        }


//        protected virtual void RotateNonDominantHand()
//        {
//            if (m_CurrentItem != null)
//            {
//                if (m_CurrentItem.NonDominantHandPosition != null)
//                {
//                    m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_TargetHandWeight);
//                    m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, m_CurrentItem.NonDominantHandPosition.rotation);
//                }
//            }
//        }


//        protected virtual void PositionHands()
//        {
//            if (m_CurrentItem != null)
//            {
//                if (m_CurrentItem.NonDominantHandPosition != null)
//                {
//                    m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_TargetHandWeight);
//                    m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_CurrentItem.NonDominantHandPosition.position);
//                }
//            }

//            Vector3 targetPosition = m_RightHandTarget.position + m_RightHand.rotation * m_HandIKOffset;
//            //targetPosition += m_RightHand.position + m_RightHand.rotation * m_HandIKOffset;

//            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_TargetHandWeight);
//            //m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_RightHandTarget.TransformPoint(m_RightHandTarget.localPosition));
//            m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_RightHandTarget.position);
//            //m_Animator.SetIKPosition(AvatarIKGoal.RightHand, targetPosition);

//        }






//        void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
//        {
//            Vector3 targetIkPosition = m_Animator.GetIKPosition(foot);

//            if (positionIkHolder != Vector3.zero)
//            {
//                targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
//                positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

//                float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feetToIkPositionSpeed);
//                targetIkPosition.y += yVariable;

//                lastFootPositionY = yVariable;

//                targetIkPosition = transform.TransformPoint(targetIkPosition);

//                m_Animator.SetIKRotation(foot, rotationIkHolder);
//            }

//            m_Animator.SetIKPosition(foot, targetIkPosition);
//        }


//        private void MovePelvisHeight()
//        {
//            if (rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
//            {
//                lastPelvisPositionY = m_Animator.bodyPosition.y;
//                return;
//            }

//            float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
//            float rOffsetPosition = rightFootIkPosition.y - transform.position.y;
//            float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

//            Vector3 newPelvisPosition = m_Animator.bodyPosition + Vector3.up * totalOffset;

//            newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

//            m_Animator.bodyPosition = newPelvisPosition;

//            lastPelvisPositionY = m_Animator.bodyPosition.y;
//        }


//        private void FeetPositionSolver(Vector3 footPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
//        {
//            //raycast handling section 
//            RaycastHit feetOutHit;

//            if (showSolverDebug)
//                Debug.DrawLine(footPosition, footPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);

//            if (Physics.Raycast(footPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, m_LayerManager.SolidLayer))
//            {
//                //finding our feet ik positions from the sky position
//                feetIkPositions = footPosition;
//                feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
//                feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
//                return;
//            }
//            feetIkPositions = Vector3.zero; //it didn't work :(
//        }



//        private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
//        {
//            feetPositions = m_Animator.GetBoneTransform(foot).position;
//            feetPositions.y = transform.position.y + heightFromGroundRaycast;
//        }












//        private void OnDrawGizmos()
//        {
//            if (m_DebugDrawLookRay)
//            {
//                if (m_LookAtPoint)
//                {
//                    Gizmos.color = Color.green;
//                    Gizmos.DrawLine(m_UpperChest.position, m_LookAtPoint.position);
//                    Gizmos.DrawSphere(m_LookAtPoint.position, 0.1f);
//                }

//            }
//        }





//        private void HandleWeights()
//        {
//            //if (m_Aiming)
//            //{
//            //    Vector3 directionTowardsTarget = m_LookAtPoint.position - m_Transform.position;
//            //    float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);
//            //    if (angle < 90)
//            //    {
//            //        m_LookAtAimBodyWeight = 1;
//            //    }
//            //    else
//            //    {
//            //        m_LookAtAimBodyWeight = 0;
//            //    }
//            //}
//            //else
//            //{
//            //    m_LookAtAimBodyWeight = 0;
//            //}

//            float targetLookAtWeight = 0;

//            m_LookAtAimBodyWeight = Mathf.Lerp(m_LookAtAimBodyWeight, targetLookAtWeight, Time.deltaTime * 3);

//            float targetMainHandWeight = 0;  //  target main hand weight

//            //if (states.states.isAiming){
//            //    t_m_weight = 1;
//            //    m_LookAtBodyWeight = 0.4f;
//            //}
//            //else{
//            //    m_LookAtBodyWeight = 0.3f;
//            //}


//            m_LookAtBodyWeight = 0.3f;


//            //if (lh_target != null) o_h_weight = 1;
//            //else o_h_weight = 0;

//            //o_h_weight = 0;

//            Vector3 directionTowardsTarget = m_LookAtPoint.position - m_Transform.position;
//            float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);

//            if (angle < 76)
//                targetLookAtWeight = 1;
//            else
//                targetLookAtWeight = 0;
//            if (angle > 45)
//                targetMainHandWeight = 0;

//            m_LookAtAimBodyWeight = Mathf.Lerp(m_LookAtAimBodyWeight, targetLookAtWeight, Time.deltaTime * 3);
//            m_HandIKWeight = Mathf.Lerp(m_HandIKWeight, targetMainHandWeight, Time.deltaTime * 9);
//        }

//    }
//}