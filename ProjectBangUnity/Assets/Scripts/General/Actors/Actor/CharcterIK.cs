namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;


    public class CharcterIK : MonoBehaviour
    {
        [Serializable]
        public class CharacterIKHands
        {
            [Range(0, 1)]
            public float m_HandWeight = 1f;
            public float HandAdjustmentSpeed = 0.2f;
            public Vector3 m_HandPositionOffset;

            private Transform m_MainHandTarget;
            private Transform m_OffHandTarget;
        }

        [Serializable]
        public class CharacterIKBody
        {
            [Range(0, 1)]
            public float LookAtBodyWeight = 0.05f;
            [Range(0, 1)]
            public float LookAtHeadWeight = 0.425f;
            [Range(0, 1)]
            public float LookAtEyesWeight = 1f;
            [Range(0, 1)]
            public float LookAtClampWeight = 0.35f;

            public float LookAtAdjustmentSpeed = 0.2f;
        }

        public bool m_DebugDrawLookRay;
        public int m_BaseLayerIndex ;
        public int m_UpperBodyLayerIndex = 5;
        public LayerMask m_LayerMask;
        public Vector3 m_LookAtOffset;
        //public CharacterIKHands m_Hands = new CharacterIKHands();
        //public CharacterIKBody m_Body = new CharacterIKBody();





        private Transform mainHandTarget;
        private Transform offHandTarget;
        private Vector3 mainHandOffset;
        private Vector3 offHandOffset;
        [Header("Character IK variables")]
        [SerializeField, Range(0, 1)]
        private float mainHandWeight;
        [SerializeField, Range(0, 1)]
        private float offHandWeight;
        [SerializeField, Range(0, 1)]
        private float lookAtWeight;
        [SerializeField, Range(0, 1)]
        private float bodyWeight;


        private ActorController actor;
        private Animator anim;
        private Transform aimPivot;
        private Transform shoulder;


        private Vector3 targetDirection;
        private Quaternion targetRotation;

        private Vector3 mainHandTargetPosition;


        private void Awake()
        {
            actor = GetComponent<ActorController>();
            anim = GetComponent<Animator>();

            shoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder).transform;
            aimPivot = new GameObject().transform;
            aimPivot.name = "Aim Pivot";
            aimPivot.transform.parent = this.gameObject.transform;
            aimPivot.position = shoulder.position;
        }


        private void FixedUpdate()
        {
            HandleShoulder();
        }


        private void HandleShoulder()
        {
            aimPivot.position = shoulder.position;

            targetDirection = actor.AimPosition - aimPivot.position;

            if (targetDirection == Vector3.zero)
                targetDirection = aimPivot.forward;
            
            targetRotation = Quaternion.LookRotation(targetDirection);
            aimPivot.rotation = Quaternion.Slerp(aimPivot.rotation, targetRotation, Time.deltaTime * 15);
        }




        private void OnAnimatorIK()
        {
            if (anim.GetBool(HashID.IsReloading)){
                mainHandWeight = 0f;
                offHandWeight = 0f;
                //lookAtWeight = 0f;
            }

            //bodyWeight = 0.4f;
            //anim.SetLookAtWeight(lookAtWeight, bodyWeight, 1, 1, 1);
            //anim.SetLookAtPosition(states.inp.aimPosition);


            if(offHandTarget != null){
                //  Update offhand
                UpdateIK(AvatarIKGoal.LeftHand, offHandTarget, offHandWeight);
            }

            if(mainHandTarget != null){
                //  Update mainhand
                UpdateIK(AvatarIKGoal.RightHand, mainHandTarget, mainHandWeight);
            }
        }


        private void UpdateIK(AvatarIKGoal goal, Transform target, float w)
        {
            if (target != null)
            {
                anim.SetIKPositionWeight(goal, w);
                anim.SetIKRotationWeight(goal, w);
                anim.SetIKPosition(goal, target.position);
                anim.SetIKRotation(goal, target.rotation);
            }
        }



        public void EquidWeapon(Transform mainHandTarget, float mainHandWeight, Transform offHandTarget, float offHandWeight)
        {
            this.mainHandTarget = mainHandTarget;
            this.mainHandWeight = mainHandWeight;
            this.offHandTarget = offHandTarget;
            this.offHandWeight = offHandWeight;
        }

        public void EquidWeapon(Transform mainHandTarget, Transform offHandTarget)
        {
            this.mainHandTarget = mainHandTarget;
            this.offHandTarget = offHandTarget;
            mainHandWeight = 0f;
            offHandWeight = 1f;
        }



        protected virtual void PositionLowerBody()
        {

        }


        protected virtual void LookAtTarget()
        {

        }


        protected virtual void RotateDominantHand()
        {

        }

        protected virtual void RotateNonDominantHand()
        {

        }

        protected virtual void PositionHands()
        {


            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, mainHandWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, mainHandTargetPosition);
        }

    }
}

