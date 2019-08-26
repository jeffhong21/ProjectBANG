using UnityEngine;
using RootMotion.FinalIK;
using System.Collections.Generic;

namespace CharacterController
{
    public class FinalIKController : CharacterIKBase
    {
        [Header("Full Body IK")]
        public FullBodyBipedIK fbIK;

        [SerializeField]
        protected Transform rightHandTarget, leftHandTarget, bodyEffectorTarget, lookTarget;
        [Header("GrounderIK")]
        public GrounderFBBIK grounderIK;

        [Header("LookAtIK")]
        public LookAtIK lookAtIK;

        [Header("AimIK")]
        public AimIK aimIK;

        [Header("Debug")]
        [SerializeField] private bool hideInInspector;


        private bool updateFrame;


        public Transform LookTarget { get { return lookTarget; } set { lookTarget = value; } }






        float lookWeight = 1;
        float lookWeightVelocity;
        float weightSmoothTime = 0.3f;


        protected override void Start()
        {
            base.Start();

        }


        protected override void Initialize()
        {
            if (rightHandTarget == null) rightHandTarget = CreateIKEffectors("RightHand Effector Target", animator.GetBoneTransform(HumanBodyBones.RightHand).position, animator.GetBoneTransform(HumanBodyBones.RightHand).rotation);
            if (leftHandTarget == null) leftHandTarget = CreateIKEffectors("LeftHand Effector Target", animator.GetBoneTransform(HumanBodyBones.LeftHand).position, animator.GetBoneTransform(HumanBodyBones.LeftHand).rotation);
            if (bodyEffectorTarget == null) bodyEffectorTarget = CreateIKEffectors("Body Effector Target", animator.bodyPosition, animator.bodyRotation);

            if (lookTarget == null)
            {
                lookTarget = CreateIKEffectors("LookAt Target", animator.GetBoneTransform(HumanBodyBones.Head).position + transform.forward, Quaternion.identity);
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





        private void OnValidate()
        {
            var ikComponents = new List<IK>();
            if (fbIK != null) ikComponents.Add(fbIK);
            //if (grounderIK != null) ikComponents.Add(grounderIK);
            if (lookAtIK != null) ikComponents.Add(lookAtIK);
            if (aimIK != null) ikComponents.Add(aimIK);


            for (int i = 0; i < ikComponents.Count; i++)
            {
                ikComponents[i].hideFlags = hideInInspector ? HideFlags.HideInInspector : HideFlags.None;
            }
            if (grounderIK != null) grounderIK.hideFlags = hideInInspector ? HideFlags.HideInInspector : HideFlags.None;


            //iks = GetComponent<IK>();
        }



        private void FixedUpdate()
        {
            updateFrame = true;
        }



        private void Update()
        {

        }



        private void LateUpdate()
        {



            LookAtIKSolver();
        }



        protected void OnAnimatorIK(int layerIndex)
        {
            
        }




        private void EquipUnequip(Item item, int slotID)
        {


        }





        private void LookAtIKSolver()
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




    }
}














