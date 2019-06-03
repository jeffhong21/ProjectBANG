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

        protected Transform rightHandEffector;
        protected Transform rightShoulderEffector;
        protected Transform leftHandEffector;
        protected Transform leftShoulderEffector;
        protected Transform bodyEffector;

        // Array of IK components that you can assign from the inspector. 
        // IK is abstract, so it does not matter which specific IK component types are used.
        public IK[] components;


        private bool updateFrame;
            
        void Start() {
            // Disable all the IK components so they won't update their solvers. Use Disable() instead of enabled = false, the latter does not guarantee solver initiation.
            foreach (IK component in components) component.Disable();
        }
        void FixedUpdate() {
            updateFrame = true;
        }
        void LateUpdate() {
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
}


