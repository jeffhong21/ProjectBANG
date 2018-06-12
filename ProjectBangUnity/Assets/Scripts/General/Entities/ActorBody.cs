namespace Bang
{
    using UnityEngine;
    using System;

    [Serializable]
    public class ActorBody
    {

        [SerializeField, Tooltip("Used for sight")]
        private Transform _head;
        [SerializeField, Tooltip("Used for holding weapons")]
        private Transform _leftHand;
        [SerializeField, Tooltip("Used for holding weapons")]
        private Transform _rightHand;
        [SerializeField, Tooltip("Main renderer")]
        private SkinnedMeshRenderer[] _mainRenderer;


        public Transform Head{
            get { return _head; }
        }

        public Transform LeftHand
        {
            get { return _leftHand; }
        }

        public Transform RightHand
        {
            get { return _rightHand; }
        }

        public SkinnedMeshRenderer[] MainRenderer
        {
            get { return _mainRenderer; }
        }


	}
}


