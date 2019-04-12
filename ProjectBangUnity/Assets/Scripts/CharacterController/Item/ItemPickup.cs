namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    public class ItemPickup : MonoBehaviour
    {
        [Header("--  ItemType Pickup Settings --")]
        [SerializeField]
        protected ItemType[] m_Items;
        [SerializeField]
        protected Transform m_TooltipUI;
        [SerializeField]
        protected Transform m_ObjectHolder;
        [SerializeField]
        protected ParticleSystem m_DefaultVFX;
        [SerializeField]
        protected ParticleSystem m_PickupVFX;
        [SerializeField]
        protected Material m_DefaultMaterial;
        [SerializeField]
        protected Material m_DropMaterial;
        [SerializeField, Tooltip("Has the item been dropped ablready.")]
        protected bool m_Dropped;
        [Header("--  Animation Settings --")]
        [SerializeField]
        protected bool m_PlayDefaultAnimation = true;
        [SerializeField]
        protected bool m_IsRotating = true;
        [SerializeField]
        protected bool m_RotateCounterClockwise;
        [SerializeField, Range(-360, 360)]
        protected float m_DegreesPerSecond = 30f;
        [SerializeField]
        protected bool m_IsBouncing = true;
        [SerializeField, Range(0, 1), Tooltip("How high the itemHolder bobbles up and down.  Value should be the same or greater than the itemHolder Y position.")]
        protected float m_Amplitude = 0.25f;
        [SerializeField, Range(0,2), Tooltip("How fast the itemHolder bobbles up and down.")]
        protected float m_Frequency = .75f;

        //[SerializeField]
        //private AnimationCurve plot = new AnimationCurve();
        //protected Transform m_Camera;



        protected BoxCollider m_Trigger;
        protected CapsuleCollider m_Collider;
        protected Rigidbody m_Rigidbody;
        protected Animator m_Animator;
        protected Transform m_Transform;
        protected ParticleSystem[] m_ParticleSystems;


        float m_DeltaTime;
        Vector3 m_DefaultPosition;
        Vector3 m_PositionOffset;
        Vector3 m_TargetRotation;
        Vector3 m_TargetPosition;

        //Transform target;
        //Vector3 targetPosition;

        public ItemType[] Items{
            get { return m_Items; }
            set { m_Items = value; }
        }

        public bool Dropped
        {
            get{ return m_Dropped; }
            set{
                if(value == false && m_Dropped == true){
                    m_Dropped = true;
                }
                else{
                    m_Dropped = value; 
                }
            }
        }

        public Collider Trigger{
            get { return m_Trigger; }
        }







        protected virtual void Awake()
        {
            m_Trigger = GetComponent<BoxCollider>();
            m_Collider = GetComponent<CapsuleCollider>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();
            m_Transform = transform;
            m_ParticleSystems = transform.GetComponentsInChildren<ParticleSystem>();

            m_DeltaTime = Time.deltaTime;

            m_DefaultPosition = m_ObjectHolder.localPosition;
            m_DefaultPosition.y += m_Amplitude;


            Initialize(false);
        }


        public void Initialize(bool isDropped)
        {
            //  Set starting random default values.
            m_Transform.eulerAngles = new Vector3(0, Random.Range(-360, 360), 0);
            m_ObjectHolder.localPosition = m_DefaultPosition;

            // Store the starting position & rotation of the object
            m_PositionOffset = m_ObjectHolder.transform.position;
            //m_Transform.localScale = Vector3.one;

            m_TooltipUI.gameObject.SetActive(false);
            //target = CameraController.Instance.transform;
        }


        protected virtual bool ObjectPickup(Collider other)
        {
            Inventory inventory = other.GetComponent<Inventory>();

            bool itemAdded = false;
            int amount = 1;
            if (inventory != null){
                for (int i = 0; i < m_Items.Length; i++){
                    if (m_Items[i] is ConsumableItem)
                        amount = ((ConsumableItem)m_Items[i]).Amount;

                    if (inventory.PickupItem(m_Items[i], amount, false))
                        itemAdded = true;
                }
            }
            return itemAdded;
        }



        protected virtual void OnEnable()
		{
            m_Transform.localScale = Vector3.one;
            //m_Camera = CameraController.Instance.transform;
		}

        protected virtual void OnDisable()
        {

        }


        protected void Update()
        {
            //if(m_PlayDefaultAnimation){
            //    if (m_IsRotating) UpdateRotation(m_DegreesPerSecond, m_RotateCounterClockwise);
            //    if (m_IsBouncing)UpdateYPosition(m_Frequency, m_Amplitude);
            //}

            if (CameraController.Instance == null)
                return;
            var horizontalForward = Vector3.Scale(CameraController.Instance.Camera.transform.forward, new Vector3(1, 0, 1)).normalized;
            m_TooltipUI.forward = horizontalForward;

        }






        //protected void DisableParticleSystems(ParticleSystem[] ps)
        //{
        //    for (int i = 0; i < ps.Length; i++){
        //        ps[i].gameObject.SetActive(false);
        //    }
        //}








        protected void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player")){
                m_TooltipUI.gameObject.SetActive(true);
            }

            if(ObjectPickup(other))
            {
                if(m_PickupVFX){

                    var pickupVfx = Instantiate(m_PickupVFX, m_Transform.position + m_Transform.up * 0.5f, Quaternion.identity);
                    var ps = pickupVfx.GetComponentInChildren<ParticleSystem>();
                    ps.Play();
                    Destroy(ps.gameObject, ps.main.duration);
                }


                StartCoroutine(ExitAnimation(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 2f));
            }
            else{
                //  Nothing has been added to the inventory.
            }
        }



        protected void OnTriggerExit(Collider other)
		{
            if (other.CompareTag("Player")){
                m_TooltipUI.gameObject.SetActive(false);
            }
		}



        private IEnumerator ExitAnimation(Vector3 targetScale, float time, float speed)
        {
            float i = 0.0f;
            float rate = (1.0f / time) * speed;
            Vector3 startScale = transform.localScale;
            while (i < 1.0f)
            {
                i += Time.deltaTime * rate;
                transform.localScale = Vector3.Lerp(startScale, targetScale, i);
                yield return null;
            }

            gameObject.SetActive(false);
        }







        //protected void UpdateRotation(float degrees, bool rotateCounterClockwise, Space space = Space.World)
        //{
        //    m_TargetRotation.x = 0;
        //    m_TargetRotation.y = degrees * m_DeltaTime;
        //    m_TargetRotation.z = 0;

        //    if (rotateCounterClockwise)
        //        m_TargetRotation = -m_TargetRotation;

        //    // Spin object around Y-Axis
        //    //itemHolder.transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
        //    m_ObjectHolder.transform.Rotate(m_TargetRotation, space);
        //}


        //protected void UpdateYPosition(float frequency, float amplitude)
        //{
        //    // Float up/down with a Sin()
        //    var height = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        //    m_TargetPosition = m_PositionOffset;
        //    m_TargetPosition.y += height;

        //    //  Set the itemHolder group position.
        //    m_ObjectHolder.transform.position = m_TargetPosition;

        //    //plot.AddKey(Time.realtimeSinceStartup, height);
        //}

	}







}
