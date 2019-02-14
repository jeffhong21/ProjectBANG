namespace Bang
{
    using UnityEngine;
    using System.Collections;

    public class ItemPickup : EntityBase
    {
        
        public WeaponObject equipableItem;
        [SerializeField]
        private ParticleSystem appearVfx;
        [SerializeField]
        private ParticleSystem pickupVfx;
        [SerializeField]
        private Material itemMaterial;


        [SerializeField]
        private GameObject itemHolder;
        [SerializeField, Tooltip("How fast the itemHolder rotates.")]
        private float degreesPerSecond = 30f;
        [SerializeField]
        private bool isRotating = true;
        [SerializeField]
        private bool isRotatingCounterClockwise;
        [SerializeField, Tooltip("How high the itemHolder bobbles up and down.  Value should be the same or greater than the itemHolder Y position.")]
        private float amplitude = 0.25f;
        [SerializeField, Tooltip("How fast the itemHolder bobbles up and down.")]
        private float frequency = 2f;


        //[Header("On itemHolder Pickup")]
        //public float endHeight = 2f;
        //public float endDegreesPerSecond = 30f;
        //public float disappearSpeed = 1f;


        //  Cached variables
        ParticleSystem[] particleSystems;
        Collider trigger;
        Vector3 posOffset = new Vector3();
        Vector3 tempRot = new Vector3();
        Vector3 tempPos = new Vector3();




		private void Awake()
		{
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            trigger = GetComponent<Collider>();

            var _transforms = itemHolder.GetComponentsInChildren<Transform>();
            for (int i = 0; i < _transforms.Length; i++){
                if(_transforms[i] != itemHolder.transform)
                    _transforms[i].gameObject.SetActive(false);
            }
        }


		// Use this for initialization
		private void Start()
        {
            InitializeItem(equipableItem);

            // Store the starting position & rotation of the object
            posOffset = itemHolder.transform.position;
        }


        private void Update()
        {
            //  Set the itemHolder rotation.
            if(isRotating){
                SetRotation(degreesPerSecond);
            }

            // Float up/down with a Sin()
            tempPos = posOffset;
            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

            //  Set the itemHolder group position.
            itemHolder.transform.position = tempPos;
        }


        private void InitializeItem(WeaponObject w)
        {
            GameObject item = Instantiate(w.prefab.gameObject, itemHolder.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.localScale = Vector3.one;

            var ps = item.GetComponentsInChildren<ParticleSystem>();
            DisableParticleSystems(ps);


            if(itemMaterial != null){
                MeshRenderer[] item_mat = item.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < item_mat.Length; i++){
                    item_mat[i].material = itemMaterial;
                }
            }
            else{
                Debug.Log("  Pickup Items have no Material");
            }
        }


        private void SetRotation(float degrees, Space space = Space.World)
        {
            tempRot.x = 0;
            tempRot.y = Time.deltaTime * degrees;
            tempRot.z = 0;

            if (isRotatingCounterClockwise)
                tempRot = -tempRot;

            // Spin object around Y-Axis
            //itemHolder.transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
            itemHolder.transform.Rotate(tempRot, space);  
        }



        private void DisableParticleSystems(ParticleSystem[] ps)
        {
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].gameObject.SetActive(false);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == Tags.Actor)
            {
                EquipActor(other.GetComponent<ActorController>());

                StartCoroutine(ExitAnimation(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 2f));
            }
        }


        private void EquipActor(ActorController actor)
        {
            actor.EquipWeapon(equipableItem.nameID);
            Debug.LogFormat("{0} has just equipped a {1}", actor.gameObject.name, equipableItem.nameID);
        }



        private IEnumerator ExitAnimation(Vector3 targetScale, float time, float speed)
        {
            float i = 0.0f;
            float rate = (1.0f / time) * speed;
            Vector3 startScale = transform.localScale;
            while(i < 1.0f){
                i += Time.deltaTime * rate;
                transform.localScale = Vector3.Lerp(startScale, targetScale, i);
                yield return null;
            }

            gameObject.SetActive(false);
        }

	}
}


