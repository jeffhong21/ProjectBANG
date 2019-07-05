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
        [SerializeField, Tooltip("Has the item been dropped ablready.")]
        protected bool m_Dropped;


        protected BoxCollider m_Trigger;
        protected CapsuleCollider m_Collider;
        protected Rigidbody m_Rigidbody;
        protected Animator m_Animator;
        protected Transform m_Transform;
        protected ParticleSystem[] m_ParticleSystems;


        float m_DeltaTime;



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

            Initialize(false);
        }


        public void Initialize(bool isDropped)
        {
            //  Set starting random default values.
            m_Transform.eulerAngles = new Vector3(0, Random.Range(-360, 360), 0);


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


        //protected void Update()
        //{
        //    if (CameraController.Instance == null)
        //        return;
        //    var horizontalForward = Vector3.Scale(CameraController.Instance.Camera.transform.forward, new Vector3(1, 0, 1)).normalized;
        //    m_TooltipUI.forward = horizontalForward;

        //}






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




	}







}
