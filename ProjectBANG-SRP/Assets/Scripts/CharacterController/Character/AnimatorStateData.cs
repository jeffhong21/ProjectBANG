namespace CharacterController
{
    using UnityEngine;
    using System;

    [Serializable]
    public class AnimatorStateData
    {
        [SerializeField]
        private string stateName = "Idle";
        [SerializeField]
        private float transitionDuration = 0.2f;
        [SerializeField]
        private float speedMultiplier = 1f;
        [SerializeField, DisplayOnly]
        private int stateID;

        //
        // Properties
        //  
        public string StateName { get { return stateName; } set { stateName = value; } }
        public float TransitionDuration { get { return transitionDuration; } set { transitionDuration = value; } }
        public float SpeedMultiplier { get { return speedMultiplier; } set { speedMultiplier = value; } }
        public float NameHash { get { return stateID; } }

        //
        // Constructor
        //  
        public AnimatorStateData(string stateName, float transitionDuration)
        {
            this.stateName = stateName;
            this.transitionDuration = transitionDuration;
        }

        public AnimatorStateData(int stateID, string stateName, float transitionDuration)
        {
            this.stateID = stateID;
            this.stateName = stateName;
            this.transitionDuration = transitionDuration;
        }

    }
}
