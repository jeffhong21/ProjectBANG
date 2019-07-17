namespace CharacterController
{
    using UnityEngine;
    using RootMotion.FinalIK;


    [DisallowMultipleComponent]
    public class FinalIKController : MonoBehaviour
    {
        private readonly string rightHandEffectorName = "RightHand Effector";
        private readonly string rightShoulderEffectorName = "RightShoulder Effector";
        private readonly string leftHandEffectorName = "LeftHand Effector";
        private readonly string leftShoulderEffectorName = "LeftShoulder Effector";
        private readonly string bodyEffectorName = "Body Effector";
        private readonly string lookTargetName = "Look Target";
        protected Transform rightHandEffector, leftHandEffector, bodyEffector, lookTarget;
        
        public FullBodyBipedIK fbIK;
        public LookAtIK lookAtIK;
        public AimIK aimIK;


        private Animator animator;
        private bool updateFrame;


        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start() {
            // Disable all the IK components so they won't update their solvers. Use Disable() instead of enabled = false, the latter does not guarantee solver initiation.
            foreach (IK component in components)
                component.Disable();

            Initialize();
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
            
            // Updating the IK solvers in a specific order. 
            foreach (IK component in components) component.GetIKSolver().Update();
        }



        protected void Initialize()
        {
            rightHandEffector = CreateEffectors(rightHandEffectorName, new Vector3(0, 1.25f, 0), Quaternion.identity);
            rightShoulderEffector = CreateEffectors(rightHandEffectorName, new Vector3(0, 1.25f, 0), Quaternion.identity);
            leftHandEffector = CreateEffectors(leftHandEffectorName, new Vector3(0, 1.25f, 0), Quaternion.identity);
            leftShoulderEffector = CreateEffectors(leftShoulderEffectorName, new Vector3(0, 1.25f, 0), Quaternion.identity);
            bodyEffector = CreateEffectors(bodyEffectorName, new Vector3(0, 0.8f, 0), Quaternion.identity);
            lookTarget = CreateEffectors(bodyEffectorName,
                animator.GetBoneTransform(HumanBodyBones.Head).position + transform.forward,
                Quaternion.identity);
        }

        protected virtual Transform CreateEffectors(string effectorName, Vector3 position, Quaternion rotation)
        {
            Transform effector = new GameObject(effectorName).transform;
            effector.position = position;
            effector.rotation = rotation;
            effector.parent = transform;

            return effector;
        }






    }

















// public class FinalIKNames
// {
//     private readonly string rightHandEffectorName = "RightHand Effector";
//     private readonly string rightShoulderEffectorName = "RightShoulder Effector";
//     private readonly string leftHandEffectorName = "LeftHand Effector";
//     private readonly string leftShoulderEffectorName = "LeftShoulder Effector";
//     private readonly string bodyEffectorName = "Body Effector";
//     private readonly string lookTargetName = "Look Target";
// }

