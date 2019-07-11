namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;





    public abstract class CameraController : MonoBehaviour
    {
        protected static CameraController _instance;
        protected static bool _lockRotation;

        public static CameraController Instance{
            get { return _instance; }
        }

        public static bool LockRotation{
            get { return _lockRotation; }
            set { _lockRotation = value; }
        }



        protected Camera m_Camera;

        public Camera Camera{
            get { return m_Camera; }
            private set{ m_Camera = value; }
        }





        //
        //  Methods
        //
        protected virtual void Awake()
        {
            _instance = this;

            m_Camera = GetComponent<Camera>();
            if(m_Camera == null)
                m_Camera = GetComponentInChildren<Camera>();


        }


        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public abstract void SetMainTarget(GameObject target);


        public abstract void RotateCamera(float mouseX, float mouseY);


        public abstract void ZoomCamera(float zoomInput);


        public virtual bool SetCameraState(string stateName)
        {
            return false;
        }



        //protected float ClampAngle(float clampAngle, float min, float max)
        //{
        //    do
        //    {
        //        if (clampAngle < -360)
        //            clampAngle += 360;
        //        if (clampAngle > 360)
        //            clampAngle -= 360;
        //    } while (clampAngle < -360 || clampAngle > 360);

        //    return Mathf.Clamp(clampAngle, min, max);
        //}










    }
}


