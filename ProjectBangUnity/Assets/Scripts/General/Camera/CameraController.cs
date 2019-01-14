namespace Bang
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


	public class CameraController : MonoBehaviour 
	{
        public bool enableCursor;

        public Transform camTransform;
        //  The character that the camera should follow.
        public Transform target;
        //  The transform of the object to attach the camera relative to.
        public Transform anchor;
        //  The offset between the anchor and the camera.
        public Vector3 anchorOffset;
       
        public bool smoothFollow;

        public bool canZoom;

        public CameraSettings values;

        public Crosshairs crosshairs;



        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private Vector3 cameraVelocity;
        private Vector3 lookRotation;
        [SerializeField, ReadOnly]
        private float distanceFromTarget;
        private float zoomInput;
        private float newDistance;


        private Camera cam;
        private Ray ray;
        private RaycastHit hitInfo;
        private Vector3 mousePosition;

		private void Awake()
		{
            cam = camTransform.GetComponent<Camera>();
            //cam = GetComponentInChildren<Camera>();
            crosshairs = Instantiate(crosshairs, transform.position, crosshairs.transform.rotation, transform);

		}


		private void Start()
		{
            newDistance = values.distanceFromTarget;
            Cursor.visible = enableCursor;
            crosshairs.gameObject.SetActive(!enableCursor);
		}



		private void Update()
		{
            GetInput();

            if(canZoom){
                ZoomInOnTarget();
            }

		}


		private void FixedUpdate()
		{
            if (target){
                MovetoTarget();
                LookAtTarget();
                //OrbitCamera();
            }
            //  If there is a crosshairs object and enable cursor is false.
            if(crosshairs && !enableCursor){
                ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hitInfo, 100, Layers.ground, QueryTriggerInteraction.Ignore)){
                    mousePosition = hitInfo.point;
                    mousePosition.y = 0.1f;
                    crosshairs.transform.position = mousePosition;

                    //DrawDebugRay(mousePosition);
                }
            }
		}


        private void DrawDebugRay(Vector3 _cursorPosition){
            Vector3 lookAtPoint = camTransform.position;
            Vector3 direction = _cursorPosition - lookAtPoint;
            //Debug.DrawRay(lookAtPoint, direction, Color.red);
        }


        public void SetTarget(Transform t)
        {
            target = t;
        }


        private void GetInput()
        {
            //mouseOrbitInput = Input.GetAxisRaw("MouseOrbit");
            zoomInput = Input.GetAxisRaw("Mouse ScrollWheel");
        }


        private void MovetoTarget()
        {
            targetPosition = target.position;
            targetPosition += Quaternion.Euler(values.xRotation, values.yRotation, 0) * -Vector3.forward * values.distanceFromTarget;

            if(smoothFollow){
                camTransform.position = Vector3.SmoothDamp(camTransform.position, targetPosition, ref cameraVelocity, values.smooth);
            }
            else{
                camTransform.position = targetPosition;
            }
        }


        private void LookAtTarget()
        {
            lookRotation = target.position - camTransform.position;
            if(lookRotation != Vector3.zero){
                targetRotation = Quaternion.LookRotation(lookRotation);
                camTransform.rotation = targetRotation;
                //camTransform.rotation = Quaternion.Slerp(camTransform.rotation, targetRotation, values.turnSpeed * Time.deltaTime);
            }

        }

        //Quaternion camTargetRotation;
        //float angle;
        //private void OrbitCamera()
        //{
        //    angle = Mathf.Atan2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //    angle = Mathf.Rad2Deg * angle;
        //    angle += target.eulerAngles.y;

        //    transform.rotation = Quaternion.Slerp(transform.rotation, camTargetRotation, values.turnSpeed * Time.deltaTime);

        //}



        private void ZoomInOnTarget()
        {
            newDistance += values.zoomStep * zoomInput;

            values.distanceFromTarget = Mathf.Lerp(values.distanceFromTarget, newDistance, values.zoomSmooth * Time.deltaTime);

            if(values.distanceFromTarget > values.maxZoom)
            {
                values.distanceFromTarget = values.maxZoom;
                newDistance = values.maxZoom;
            }

            if (values.distanceFromTarget < values.minZooom)
            {
                values.distanceFromTarget = values.minZooom;
                newDistance = values.minZooom;
            }
        }




        //private void MoveWithTarget(float time)
        //{
        //    targetPosition = target.position + anchorOffset;
        //    transform.position = Vector3.Lerp(transform.position, targetPosition, values.moveSpeed * time);
        //}

        //private void MouseOrbitTarget()
        //{
        //    previousMousePosition = currentMousePosition;
        //    currentMousePosition = Input.mousePosition;

        //    if(mouseOrbitInput > 0)
        //    {
        //        values.yRotation += (currentMousePosition.x - previousMousePosition.x) * values.yOrbitSmooth;
        //    }
        //}
	}

}