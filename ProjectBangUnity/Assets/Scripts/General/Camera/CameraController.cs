namespace Bang
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


	public class CameraController : MonoBehaviour 
	{
        public Transform target;
        [SerializeField]
        Vector3 defaultPosition = new Vector3(0, 12, -9);
        [SerializeField]
        bool useExtendedPosition;
        [SerializeField]
        Vector3 extendedPosition = new Vector3(0, 18, -12);
        [SerializeField]
        float smoothSpeed = 0.125f;
        [SerializeField, ReadOnly]
        Vector3 currentPosition;


        public void SetTarget(Transform target)
        {
            this.target = target;
        }


		private void FollowTarget()
        {
            currentPosition = useExtendedPosition ? extendedPosition : defaultPosition;
            //cameraDirection = target.position - transform.position;
            //Vector3 desiredPosition = transform.position + target.position + defaultPosition;

            Vector3 desiredPosition = target.position + currentPosition;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.LookAt(target);
        }


        private void LateUpdate()
		{
            if(target){
                FollowTarget();
                //ZoomOnScroll();
            }
		}



        //[SerializeField, Range(1f, 1000f), Tooltip("How fast the camera moves.")]
        //private float _moveSpeed = 100f;

        //[SerializeField, Range(1f, 1000f), Tooltip("How fast the camera zooms.")]
        //private float _scrollSpeed = 200f;

        //[SerializeField, Range(1f, 100f), Tooltip("The minimum zoom level (Y-position) for the camera.")]
        //private float _minZoom = 10f;

        //[SerializeField, Range(10f, 1000f), Tooltip("The maximum zoom level (Y-position) for the camera.")]
        //private float _maxZoom = 150f;

        //private Vector3 cameraDirection;


        //private void ZoomOnScroll()
        //{
        //    var scroll = Input.GetAxis("Mouse ScrollWheel");
        //    if (scroll == 0f){
        //        return;
        //    }

        //    var y = Mathf.Clamp(this.transform.position.y + (-Mathf.Sign(scroll) * _scrollSpeed * Time.unscaledDeltaTime), _minZoom, _maxZoom);
        //    this.transform.position = new Vector3(this.transform.position.x, y, this.transform.position.z);
        //}


	}

}