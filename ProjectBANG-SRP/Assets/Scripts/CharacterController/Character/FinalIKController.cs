namespace CharacterController
{
    using UnityEngine;
    using RootMotion.FinalIK;


    [DisallowMultipleComponent]
    public class FinalIKController : MonoBehaviour
    {
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



        
	}
}


