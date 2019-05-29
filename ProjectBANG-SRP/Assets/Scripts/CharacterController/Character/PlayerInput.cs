namespace CharacterController
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class PlayerInput : MonoBehaviour
    {
        private string m_HorizontalInputName = "Horizontal";
        private string m_VerticalInputName = "Vertical";
        private string m_RotateCameraXInput = "Mouse X";
        private string m_RotateCameraYInput = "Mouse Y";
        private string m_MouseScrollInput = "Mouse ScrollWheel";

        [SerializeField]
        private bool m_AxisRaw = true;


        [Header("-- Debug Settings --")]
        public ShakeTransformEventData data;
        //[SerializeField, DisplayOnly]
        private float m_Horizontal, m_Forward;
        //[SerializeField, DisplayOnly]
        private Vector3 m_InputVector;
        [SerializeField, DisplayOnly]
        private float m_MouseHorizontal, m_MouseVertical;
        private Vector3 m_MouseInputVector;
        [SerializeField]
        private float m_LookDistance = 50f;
        //[SerializeField]
        //private LayerMask m_LayerMask;


        private Vector3 m_CameraFwd;
        private Vector3 m_LookDirection;




        [SerializeField]
        private CameraController m_CameraController;
        private Transform m_Camera;
        private CharacterLocomotion m_Controller;
        private ItemActionManager m_ItemAction;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private float m_DeltaTime;



        public string HorizontalInputName{
            get { return m_HorizontalInputName; }
        }

        public string VerticalInputName{
            get { return m_VerticalInputName; }
        }





        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_ItemAction = GetComponent<ItemActionManager>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;
            //m_LayerMask = ~(1 << gameObject.layer);
        }

		private void OnEnable()
		{
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}


		private void OnDisable()
		{
            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAimActionStart);
		}


		private void Start()
		{
            if (m_CameraController || CameraController.Instance != null)
            {
                if (CameraController.Instance == null){
                    m_CameraController = Instantiate(m_CameraController) as CameraController;
                    Debug.Log("Instantiating a new Camera Controller through the player.");
                }
                else{
                    m_CameraController = CameraController.Instance;
                }
                m_CameraController.SetMainTarget(m_GameObject);
                m_Camera = m_CameraController.Camera.transform;
            }
            else{
                Debug.LogError("Player has no Camera");
            }

		}



		public virtual float GetAxis(string name, bool useRaw = false){
            return useRaw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
        }



        private void FixedUpdate()
		{
            //  LOCK CAMERA
            if (LockCameraRotation() == true) return;


            //  -----------
            //  Character Input
            m_Horizontal = GetAxis(m_HorizontalInputName, m_AxisRaw);
            m_Forward = GetAxis(m_VerticalInputName, m_AxisRaw);

            m_InputVector.Set(m_Horizontal, 0, m_Forward);
            m_Controller.InputVector = m_InputVector;

            m_LookDirection = m_CameraController == null ? m_Transform.forward : Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;
            m_Controller.LookDirection = m_LookDirection;


            //  -----------
            //  Camera Input
            if (m_CameraController != null)
            {
                m_MouseHorizontal = Input.GetAxis(m_RotateCameraXInput);
                m_MouseVertical = Input.GetAxis(m_RotateCameraYInput);

                m_CameraController.RotateCamera(m_MouseHorizontal, m_MouseVertical);
                m_CameraController.ZoomCamera(Input.GetAxisRaw(m_MouseScrollInput));
            }


            DebugButtonPress();
		}




        private bool LockCameraRotation()
        {
            if (Input.GetKeyDown(KeyCode.L)){
                if (CameraController.Instance != null){
                    CameraController.LockRotation = !CameraController.LockRotation;
                }

                return true;
            }
            return false;
        }



        private void DebugButtonPress()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                var health = GetComponent<CharacterHealth>();
                health.TakeDamage(10, m_Transform.position + Vector3.up, -m_Transform.right, m_GameObject);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                m_CameraController.Camera.GetComponentInParent<CameraShake>().AddShakeEvent(data);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                //EventHandler.LogAllRegistered();
                UnityEditor.Selection.activeGameObject = gameObject;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPlaying;
                Debug.Break();
            }
        }




        private void OnAimActionStart(bool aim)
        {
            if (aim == true)
            {
                CameraState aimState = CameraController.Instance.GetCameraStateWithName("TPS_Aim");
                CameraController.Instance.ChangeCameraState("TPS_Aim");
            }
            else
            {
                CameraController.Instance.ChangeCameraState("TPS_Default");
            }
        }










        private void OnGUI()
        {
            //if (Application.isPlaying)
            //{

            //    GUI.color = Color.black;
            //    GUI.Label(new Rect(10, Screen.height / 2, 200, 20), string.Format("Input Mag: {0}", m_InputVector.magnitude));
            //    GUI.Label(new Rect(10, Screen.height / 2 + 30, 200, 20), string.Format("Input Angle: {0}", Vector3.Angle((Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized), m_Transform.forward)));

            //}

        }
	}
}


