namespace CharacterController
{
    using UnityEngine;
    using RootMotion.FinalIK;


    [DisallowMultipleComponent]
    public class FinalIKController : MonoBehaviour
    {
        [Header("Full Body IK")]
        public FullBodyBipedIK fbIK;

        [SerializeField]
        protected Transform rightHandTarget, leftHandTarget, bodyEffectorTarget, lookTarget;

        [Header("LookAtIK")]
        public LookAtIK lookAtIK;

        [Header("AimIK")]
        public AimIK aimIK;


        private Animator animator;
        private bool updateFrame;


        public Transform LookTarget { get { return lookTarget; } set { lookTarget = value; } }



        private void Awake()
        {
            animator = GetComponent<Animator>();
        }


        private void Start()
        {
            animator = GetComponent<Animator>();
            if (rightHandTarget == null) rightHandTarget = CreateEffectors("RightHand Effector Target", animator.GetBoneTransform(HumanBodyBones.RightHand).position, animator.GetBoneTransform(HumanBodyBones.RightHand).rotation);
            if (leftHandTarget == null) leftHandTarget = CreateEffectors("LeftHand Effector Target", animator.GetBoneTransform(HumanBodyBones.LeftHand).position, animator.GetBoneTransform(HumanBodyBones.LeftHand).rotation);
            if (bodyEffectorTarget == null) bodyEffectorTarget = CreateEffectors("Body Effector Target", animator.bodyPosition, animator.bodyRotation);

            if (lookTarget == null) {
                lookTarget = CreateEffectors("LookAt Target", animator.GetBoneTransform(HumanBodyBones.Head).position + transform.forward, Quaternion.identity);
                lookTarget.position = animator.GetBoneTransform(HumanBodyBones.Neck).position + transform.forward * 10;
            }


            fbIK.solver.rightHandEffector.target = rightHandTarget;
            fbIK.solver.leftHandEffector.target = leftHandTarget;
            fbIK.solver.bodyEffector.target = bodyEffectorTarget;


            lookAtIK.solver.target = lookTarget;
            lookAtIK.solver.head = new IKSolverLookAt.LookAtBone(animator.GetBoneTransform(HumanBodyBones.Head));
            IKSolverLookAt.LookAtBone[] spineBones =
                {
                new IKSolverLookAt.LookAtBone(animator.GetBoneTransform(HumanBodyBones.Spine)),
                new IKSolverLookAt.LookAtBone(animator.GetBoneTransform(HumanBodyBones.Chest)),
                new IKSolverLookAt.LookAtBone(animator.GetBoneTransform(HumanBodyBones.UpperChest)),
                new IKSolverLookAt.LookAtBone(animator.GetBoneTransform(HumanBodyBones.Neck))
            };
            lookAtIK.solver.spine = spineBones;

            

        }




        //private void FixedUpdate()
        //{
        //    updateFrame = true;
        //}



        //private void Update()
        //{

        //}


        float lookWeight = 1;
        float lookWeightVelocity;
        float weightSmoothTime = 0.3f;
        private void LateUpdate()
        {
            //// Do nothing if FixedUpdate has not been called since the last LateUpdate
            //if (!updateFrame) return;
            //updateFrame = false;

            //// Updating the IK solvers in a specific order. 
            //foreach (IK component in components) component.GetIKSolver().Update();

            var lookTargetDir = lookTarget.position - transform.position;
            var angleDif = Vector3.Angle(transform.forward, lookTargetDir);


            lookAtIK.solver.IKPositionWeight = Mathf.SmoothDamp(lookAtIK.solver.IKPositionWeight, (angleDif > 75) ? 0 : lookWeight, ref lookWeightVelocity, weightSmoothTime);
            if (lookAtIK.solver.IKPositionWeight >= 0.999f) lookAtIK.solver.IKPositionWeight = 1f;
            if (lookAtIK.solver.IKPositionWeight <= 0.001f) lookAtIK.solver.IKPositionWeight = 0f;

            //CharacterDebug.Log("Look Target Angle difference", angleDif);



        }



        private void EquipUnequip(Item item, int slotID)
        {


        }




        private Transform CreateEffectors(string effectorName, Vector3 position, Quaternion rotation, bool hideFlag = true)
        {
            Transform effector = new GameObject(effectorName).transform;
            effector.position = position;
            effector.rotation = rotation;
            effector.parent = transform;

            //if(hideFlag) effector.hideFlags = HideFlags.HideInHierarchy;
            return effector;
        }





        public void SetHideFlags(bool showFlags)
        {
            HideFlags hideFlags = showFlags ? HideFlags.None : HideFlags.HideInHierarchy;

            rightHandTarget.hideFlags = hideFlags;
        }
    }
}














