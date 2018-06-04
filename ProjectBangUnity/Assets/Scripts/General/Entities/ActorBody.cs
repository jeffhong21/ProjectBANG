namespace Bang
{
    using UnityEngine;
    using System;

    public class ActorBody : MonoBehaviour
    {

        [SerializeField, Tooltip("Used for sight")]
        private Transform _head;
        [SerializeField, Tooltip("Used for holding weapons")]
        private Transform _leftHand;
        [SerializeField, Tooltip("Used for holding weapons")]
        private Transform _rightHand;
        [SerializeField, Tooltip("Main renderer")]
        private SkinnedMeshRenderer[] _mainRenderer;
        [SerializeField, Tooltip("Particle System when hit")]
        private ParticleSystem _hitVfx;





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


        public ParticleSystem HitVfx{
            get { return _hitVfx; }
        }


		private void Awake()
		{
            //if(_weaponHolder == null) throw new ArgumentNullException("Player has no weapon location marker.");
		}


	}
}


