namespace CharacterController
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


    public class ThirdPersonCamera : MonoBehaviour
    {

        public float mouseSensitivity = 10;
        public Transform target;
        public float dstFromTarget = 5;
        public Vector2 pitchMinMax = new Vector2(-40, 85);

        public float rotationSmoothTime = 0.12f;

        public Camera cam;

        Vector3 rotationSmoothVelocity;
        Vector3 currentRotation;

        float yaw;
        float pitch;



		private void Awake()
		{
            cam = GetComponent<Camera>();

		}


		private void LateUpdate()
		{
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            //Vector3 targetRotation = new Vector3(pitch, yaw);
            transform.eulerAngles = currentRotation;

            transform.position = target.position - transform.forward * dstFromTarget;


		}
	}

}


