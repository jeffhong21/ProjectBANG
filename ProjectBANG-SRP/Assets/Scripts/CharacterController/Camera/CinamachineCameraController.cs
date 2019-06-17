namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using System;
    using System.Collections.Generic;
    using Cinemachine;


    public class CinamachineCameraController : CameraController
    {

        public CinemachineFreeLook m_FreeLookCamera;


        private CinemachineCollider m_CinemachineCollider;
        private CinemachineImpulseSource m_CinemachineImpulseSource;
        //private PostProcessVolume m_PostProcessVolume;
        //private PostProcessProfile m_PostProcessProfile;




        protected override void Awake()
        {
            base.Awake();
            _instance = this;

            m_Camera = GetComponent<Camera>();
            //originalZoom = m_FreeLookCamera.m_Orbits[1].m_Radius;
            m_CinemachineCollider = m_FreeLookCamera.GetComponent<CinemachineCollider>();
            m_CinemachineImpulseSource = m_FreeLookCamera.GetComponent<CinemachineImpulseSource>();
            //m_PostProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
            //m_PostProcessProfile = m_PostProcessVolume.profile;
        }




        public override void SetMainTarget(GameObject target)
        {
            m_Character = target;

            if (m_Character)
            {
                m_FreeLookCamera.LookAt = m_Character.transform;
                m_FreeLookCamera.Follow = m_Character.transform;

            }
        }




        protected override void LateUpdate()
        {

        }

    }
}


