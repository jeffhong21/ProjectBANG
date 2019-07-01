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






    /// <summary>
    /// Maintains FBBIK hands on a 2-handed prop, regardless of position offset of the hand effectors
    /// </summary>
    public class FBIKHandsOnProp : MonoBehaviour
    {

        public FullBodyBipedIK ik; // Reference to the FBBIK component
        public bool leftHanded;

        void Awake()
        {
            // Add to OnPreUpdate delegate to get a call before the solver starts updating
            ik.solver.OnPreRead += OnPreRead;
        }

        private void OnPreRead()
        {
            if (leftHanded) HandsOnProp(ik.solver.leftHandEffector, ik.solver.rightHandEffector);
            else HandsOnProp(ik.solver.rightHandEffector, ik.solver.leftHandEffector);
        }

        private void HandsOnProp(IKEffector mainHand, IKEffector otherHand)
        {
            // Get the animated direction from the main hand to the other hand
            Vector3 toOtherHand = otherHand.bone.position - mainHand.bone.position;

            // Get the hand direction relative to the main hand's rotation
            Vector3 otherHandRelativeDirection = Quaternion.Inverse(mainHand.bone.rotation) * toOtherHand;

            // Get the center point of two hands
            Vector3 handsCenter = mainHand.bone.position + (toOtherHand * 0.5f);

            // Get the other hand's rotation relative to the main hand's rotation
            Quaternion otherHandRelativeRotation = Quaternion.Inverse(mainHand.bone.rotation) * otherHand.bone.rotation;

            // Get the direction from the main hand to the other hand that icludes effector position offsets
            Vector3 toOtherHandWithOffset = (otherHand.bone.position + otherHand.positionOffset) - (mainHand.bone.position + mainHand.positionOffset);

            // Get the center point of two hands that includes effector position offsets
            Vector3 handsCenterWithOffset = (mainHand.bone.position + mainHand.positionOffset) + (toOtherHand * 0.5f);

            // Main hand position
            mainHand.position = (mainHand.bone.position + mainHand.positionOffset) + (handsCenterWithOffset - handsCenter);
            mainHand.positionWeight = 1f;

            // Main hand rotation
            Quaternion rotationOffset = Quaternion.FromToRotation(toOtherHand, toOtherHandWithOffset);
            mainHand.bone.rotation = rotationOffset * mainHand.bone.rotation;

            // Other hand position
            otherHand.position = mainHand.position + mainHand.bone.rotation * otherHandRelativeDirection;
            otherHand.positionWeight = 1f;

            // Other hand rotation
            otherHand.bone.rotation = mainHand.bone.rotation * otherHandRelativeRotation;

            ik.solver.leftArmMapping.maintainRotationWeight = 1f;
            ik.solver.rightArmMapping.maintainRotationWeight = 1f;
        }

        // Clean up delegates
        void OnDestroy()
        {
            if (ik != null)
            {
                ik.solver.OnPreRead -= OnPreRead;
            }
        }
    }










}


