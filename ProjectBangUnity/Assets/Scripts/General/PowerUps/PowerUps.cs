namespace Bang
{
    using UnityEngine;
    using System.Collections;

    public class PowerUps : EntityBase
    {
        
        public GameObject item;
        [Tooltip("How fast the item rotates.")]
        public float degreesPerSecond = 30f;
        public bool isRotating = true;
        public bool isRotatingCounterClockwise;
        [Tooltip("How high the item bobbles up and down.  Value should be the same or greater than the item Y position.")]
        public float amplitude = 0.25f;
        [Tooltip("How fast the item bobbles up and down.")]
        public float frequency = 2f;

        public ParticleSystem appearVfx;
        public ParticleSystem pickupVfx;

        //[Header("On Item Pickup")]
        //public float endHeight = 2f;
        //public float endDegreesPerSecond = 30f;
        //public float disappearSpeed = 1f;

        ParticleSystem[] particleSystems;
        Collider trigger;
        // Position Storage Variables
        Vector3 posOffset = new Vector3();
        Vector3 tempRot = new Vector3();
        Vector3 tempPos = new Vector3();


		private void Awake()
		{
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            trigger = GetComponent<Collider>();
		}


		// Use this for initialization
		private void Start()
        {
            // Store the starting position & rotation of the object
            posOffset = item.transform.position;
        }


        private void Update()
        {
            //  Set the item rotation.
            if(isRotating)
            {
                SetRotation(degreesPerSecond);
            }

            // Float up/down with a Sin()
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

            //  Set the item group position.
            item.transform.position = tempPos;
        }



        private void SetRotation(float degrees, Space space = Space.World)
        {
            tempRot.x = 0;
            tempRot.y = Time.deltaTime * degrees;
            tempRot.z = 0;

            if (isRotatingCounterClockwise)
                tempRot = -tempRot;

            // Spin object around Y-Axis
            //item.transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
            item.transform.Rotate(tempRot, space);  
        }



        private void DisableParticleSystems(ParticleSystem[] ps)
        {
            
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                this.gameObject.SetActive(false);
            }
                
        }
	}
}


