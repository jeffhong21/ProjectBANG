namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [RequireComponent(typeof(CharacterLocomotion))]
    public class CharacterIK : MonoBehaviour
    {
        private readonly float m_LookAtRayLength = 30;

        [SerializeField]
        private bool m_DebugDrawLookRay;
        [SerializeField]
        private int m_BaseLayerIndex = 0;
        [SerializeField]
        private int m_UpperBodyLayerIndex = 1;
        [SerializeField]
        private LayerMask m_LayerMask;
        [Header("Body")]
        [SerializeField, Range(0, 1)]
        protected float m_LookAtAimBodyWeight = 1f;
        [SerializeField, Range(0, 1)]
        protected float m_LookAtBodyWeight = 0.05f;
        [SerializeField, Range(0, 1)]
        protected float m_LookAtHeadWeight = 0.425f;
        [SerializeField, Range(0, 1)]
        protected float m_LookAtEyesWeight = 1f;
        [SerializeField, Range(0, 1)]
        protected float m_LookAtClampWeight = 0.35f;
        [SerializeField]
        protected float m_LookAtAdjustmentSpeed = 0.2f;
        [SerializeField]
        private Vector3 m_LookAtOffset;
        [Header("Hands")]
        [SerializeField, Range(0, 1)]
        protected float m_HandIKWeight = 1f;
        [SerializeField]
        protected float m_HandIKAdjustmentSpeed = 0.2f;
        [SerializeField]
        protected Vector3 m_HandIKOffset;

        [Header("States")]
        [SerializeField]
        protected bool m_EnableHandIK;



        private Animator m_Animator;
        private CharacterLocomotion m_Controller;
        private Transform m_Transform;
        private GameObject m_GameObject;


        private float m_TargetLookAtWeight;
        private float m_TargetHandWeight;
        private float m_HandIKAdjustmentVelocity;
        private float m_LookAtAdjustmentVelocity;

        private Transform m_LookAtPoint;
        //private Vector3 m_LookAtDirection;

        private Transform m_RightHand;
        private Transform m_LeftHand;
        private Transform m_UpperChest;
        private Transform m_RightShoulder;

        private Transform m_AimPivot;
        [SerializeField]
        private Transform m_RightHandTarget;
        [SerializeField]
        private ItemObject m_CurrentItem;


        private Vector3 m_TargetDirection;
        private Quaternion m_TargetRotation;



        [SerializeField]
        Transform itemSlot;

		private void Awake()
		{
            m_Animator = GetComponent<Animator>();
            m_Controller = GetComponent<CharacterLocomotion>();
            m_Transform = transform;
            m_GameObject = gameObject;



            m_RightHand = m_Animator.GetBoneTransform(HumanBodyBones.RightHand).transform;
            m_LeftHand = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand).transform;
            m_UpperChest = m_Animator.GetBoneTransform(HumanBodyBones.UpperChest).transform;
            m_RightShoulder = m_Animator.GetBoneTransform(HumanBodyBones.RightShoulder).transform;


            m_LookAtPoint = new GameObject("Look At Point").transform; //.parent = gameObject.transform;
            m_LookAtPoint.transform.parent = gameObject.transform;
            m_LookAtPoint.position = (m_Transform.forward * m_LookAtRayLength) + (Vector3.up * m_UpperChest.position.y);
            //var _position = new Vector3(1, 0, 0);
            m_LookAtPoint.localPosition = m_LookAtPoint.localPosition;// + _position;

            m_AimPivot = new GameObject("Aim Pivot").transform;
            m_AimPivot.transform.parent = gameObject.transform;
            m_AimPivot.position = m_RightShoulder.position;




            //ItemEquipSlot[] itemSlots = GetComponentsInChildren<ItemEquipSlot>();
            //for (int i = 0; i < itemSlots.Length; i++)
            //{
            //    if (itemSlots[i].ID == 0) m_RightHandTarget = itemSlots[i].transform;
            //    //if (itemSlots[i].ID == 0) itemSlot = itemSlots[i].transform;
            //}

            m_RightHandTarget = new GameObject("Right Hand Target").transform;
            m_RightHandTarget.transform.parent = m_AimPivot;
            m_RightHandTarget.position = Vector3.zero;
		}




        private void OnEnable()
        {
            EventHandler.RegisterEvent<ItemObject>(m_GameObject, "OnInventoryEquip", HandleItem);
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAim);
        }


        private void OnDisable()
        {
            EventHandler.UnregisterEvent<ItemObject>(m_GameObject, "OnInventoryEquip", HandleItem);
            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAim);
        }



		private void FixedUpdate()
        {
            m_AimPivot.position = m_RightShoulder.position;
            m_TargetDirection = m_LookAtPoint.position - m_AimPivot.position;
            if (m_TargetDirection == Vector3.zero)
                m_TargetDirection = m_AimPivot.forward;

            m_TargetRotation = Quaternion.LookRotation(m_TargetDirection);
            m_AimPivot.rotation = Quaternion.Slerp(m_AimPivot.rotation, m_TargetRotation, Time.deltaTime * 15);

        }


		private void Update()
		{
            if(m_Controller.Aiming){
                m_LookAtPoint.position = m_Controller.LookPosition;
            }
            else{
                //m_LookAtPoint.position.Set()
            }
		}



		private void OnAnimatorIK()
		{
            LookAtTarget();
            RotateDominantHand();
            RotateNonDominantHand();
            PositionHands();
		}




        private void HandleItem(ItemObject item)
        {
            //Debug.LogFormat("{0} currently equipped item is {1}.", gameObject.name, item == null ? "<null>" : item.ItemName);

            if(item == null)
            {
                m_CurrentItem = null;
                m_RightHandTarget.localPosition = m_RightHand.localPosition;
                m_RightHandTarget.localEulerAngles = m_RightHand.localEulerAngles;
            }
            else
            {
                m_CurrentItem = item;
                //m_RightHandTarget.localPosition = item.HandIKPosition.position;
                //m_RightHandTarget.localEulerAngles = item.HandIKPosition.rotation;
                m_RightHandTarget.localPosition = item.PositionOffset;
                m_RightHandTarget.localEulerAngles = item.RotationOffset;
            }

        }


        private void OnAim(bool aim)
        {
            //Debug.LogFormat("{0} aiming is {1}.", gameObject.name, aim);

            if (aim)
            {
                m_LookAtAimBodyWeight = 1;
                m_HandIKWeight = m_CurrentItem == null ? 0 : 1;
            }
            else
            {
                m_LookAtAimBodyWeight = 0;
                m_HandIKWeight = 0;
            }
        }





        protected virtual void LookAtTarget()
        {
            //Vector3 directionTowardsTarget = m_LookAtPoint.position - m_Transform.position;
            //float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);
            //if (angle < 76)
            //    m_LookAtAimBodyWeight = 1;
            //else
            //    m_LookAtAimBodyWeight = 0;


            //m_HandIKWeight = Mathf.SmoothDamp(m_HandIKWeight, m_TargetHandWeight, ref m_HandIKAdjustmentVelocity, m_HandIKAdjustmentSpeed);
            m_TargetHandWeight = Mathf.SmoothDamp(m_TargetHandWeight, m_HandIKWeight, ref m_HandIKAdjustmentVelocity, m_HandIKAdjustmentSpeed);
            //m_LookAtAimBodyWeight = Mathf.SmoothDamp(m_LookAtAimBodyWeight, m_TargetLookAtWeight, ref m_LookAtAdjustmentVelocity, m_LookAtAdjustmentSpeed);
            m_TargetLookAtWeight = Mathf.SmoothDamp(m_TargetLookAtWeight, m_LookAtAimBodyWeight, ref m_LookAtAdjustmentVelocity, m_LookAtAdjustmentSpeed);


            m_Animator.SetLookAtWeight(m_TargetLookAtWeight, m_LookAtBodyWeight, m_LookAtHeadWeight, m_LookAtEyesWeight, m_LookAtClampWeight);
            m_Animator.SetLookAtPosition(m_LookAtPoint.position + m_LookAtOffset);
        }



        protected virtual void RotateDominantHand()
        {
            //Vector3 direction = m_LookAtPoint.position - m_RightHandTarget.position;
            //Quaternion rotation = Quaternion.FromToRotation(m_RightHandTarget.forward, direction);
            //rotation *= Quaternion.Euler(0, 0, -90);
            //m_RightHandTarget.rotation = Quaternion.Slerp(m_Transform.rotation, rotation, Time.deltaTime * m_HandIKAdjustmentSpeed);


            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_TargetHandWeight);
            m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_RightHandTarget.rotation);
        }


        protected virtual void RotateNonDominantHand()
        {
            if (m_CurrentItem != null){
                if(m_CurrentItem.NonDominantHandPosition != null){
                    m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_TargetHandWeight);
                    m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, m_CurrentItem.NonDominantHandPosition.rotation);
                }
            }
        }


        protected virtual void PositionHands()
        {
            if (m_CurrentItem != null)
            {
                if (m_CurrentItem.NonDominantHandPosition != null)
                {
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_TargetHandWeight);
                    m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_CurrentItem.NonDominantHandPosition.position);
                }
            }

            Vector3 targetPosition = m_RightHandTarget.position + m_RightHand.rotation * m_HandIKOffset;
            //targetPosition += m_RightHand.position + m_RightHand.rotation * m_HandIKOffset;

            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_TargetHandWeight);
            //m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_RightHandTarget.TransformPoint(m_RightHandTarget.localPosition));
            m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_RightHandTarget.position);
            //m_Animator.SetIKPosition(AvatarIKGoal.RightHand, targetPosition);

        }











		private void OnDrawGizmos()
		{
            if(m_DebugDrawLookRay){
                if(m_LookAtPoint){
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(m_UpperChest.position, m_LookAtPoint.position);
                    Gizmos.DrawSphere(m_LookAtPoint.position, 0.1f);
                }

            }
		}





        private void HandleWeights()
        {
            //if (m_Aiming)
            //{
            //    Vector3 directionTowardsTarget = m_LookAtPoint.position - m_Transform.position;
            //    float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);
            //    if (angle < 90)
            //    {
            //        m_LookAtAimBodyWeight = 1;
            //    }
            //    else
            //    {
            //        m_LookAtAimBodyWeight = 0;
            //    }
            //}
            //else
            //{
            //    m_LookAtAimBodyWeight = 0;
            //}

            float targetLookAtWeight = 0;

            m_LookAtAimBodyWeight = Mathf.Lerp(m_LookAtAimBodyWeight, targetLookAtWeight, Time.deltaTime * 3);

            float targetMainHandWeight = 0;  //  target main hand weight

            //if (states.states.isAiming){
            //    t_m_weight = 1;
            //    m_LookAtBodyWeight = 0.4f;
            //}
            //else{
            //    m_LookAtBodyWeight = 0.3f;
            //}


            m_LookAtBodyWeight = 0.3f;


            //if (lh_target != null) o_h_weight = 1;
            //else o_h_weight = 0;

            //o_h_weight = 0;

            Vector3 directionTowardsTarget = m_LookAtPoint.position - m_Transform.position;
            float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);

            if (angle < 76)
                targetLookAtWeight = 1;
            else
                targetLookAtWeight = 0;
            if (angle > 45)
                targetMainHandWeight = 0;

            m_LookAtAimBodyWeight = Mathf.Lerp(m_LookAtAimBodyWeight, targetLookAtWeight, Time.deltaTime * 3);
            m_HandIKWeight = Mathf.Lerp(m_HandIKWeight, targetMainHandWeight, Time.deltaTime * 9);
        }

	}
}