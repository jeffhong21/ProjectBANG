namespace CharacterController
{
    using UnityEngine;
    using RootMotion.FinalIK;


    [DisallowMultipleComponent]
    public class FinalIKController : MonoBehaviour
    {

        protected Transform rightHandTarget, leftHandTarget, bodyEffectorTarget, lookAtTarget;

        public FullBodyBipedIK fbIK;
        public LookAtIK lookAtIK;
        public AimIK aimIK;


        private Animator animator;
        private bool updateFrame;




        private void Start()
        {
            animator = GetComponent<Animator>();
            if(rightHandTarget == null) rightHandTarget = CreateEffectors("RightHand Effector Target", animator.GetBoneTransform(HumanBodyBones.RightHand).position, animator.GetBoneTransform(HumanBodyBones.RightHand).rotation);
            if(leftHandTarget == null) leftHandTarget = CreateEffectors("LeftHand Effector Target", animator.GetBoneTransform(HumanBodyBones.LeftHand).position, animator.GetBoneTransform(HumanBodyBones.LeftHand).rotation);
            if(bodyEffectorTarget == null) bodyEffectorTarget = CreateEffectors("Body Effector Target", animator.bodyPosition, animator.bodyRotation);
            if(lookAtTarget == null) lookAtTarget = CreateEffectors("LookAt Target", animator.GetBoneTransform(HumanBodyBones.Head).position + transform.forward, Quaternion.identity);

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
            // Do nothing if FixedUpdate has not been called since the last LateUpdate
            if (!updateFrame) return;
            updateFrame = false;

            //// Updating the IK solvers in a specific order. 
            //foreach (IK component in components) component.GetIKSolver().Update();
        }



        private void EquipUnequip(Item item, int slotID)
        {

        }




        private virtual Transform CreateEffectors(string effectorName, Vector3 position, Quaternion rotation)
        {
            Transform effector = new GameObject(effectorName).transform;
            effector.position = position;
            effector.rotation = rotation;
            effector.parent = transform;

            return effector;
        }






    }
}














