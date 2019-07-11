﻿namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using System;
    using System.Collections.Generic;
    using Cinemachine;


    public class CinamachineCameraController : CameraController
    {
        [Serializable]
        public class VirtualCameraState
        {
            [SerializeField] private string stateName;
            [SerializeField] private CinemachineVirtualCameraBase virtualCamera;

            public string StateName {
                get { return stateName; }
                set {
                    stateName = value;
                    if (stateName.Length == 0)
                        stateName = virtualCamera.name;
                }
            }

            public CinemachineVirtualCameraBase VirtualCamera
            {
                get { return virtualCamera; }
                set { virtualCamera = value; }
            }


            public VirtualCameraState(string stateName)
            {
                this.stateName = stateName;
            }

            public VirtualCameraState(string stateName, CinemachineVirtualCameraBase virtualCamera)
            {
                this.stateName = stateName;
                this.virtualCamera = virtualCamera;
            }
        }





        [Header("Cinemachine")]
        [SerializeField]
        private CinemachineBrain m_CMBrain;
        [Header("Targets")]
        [SerializeField]
        private Transform m_FollowTarget;
        [SerializeField]
        private Transform m_LookAtTarget;
        [Header("Virtual Cameras")]
        [SerializeField]
        private VirtualCameraState[] virtualCameras = { new VirtualCameraState("DEFAULT") };
        private int currentCamera;

        private CinemachineVirtualCameraBase activeCamera;
        private CinemachineFreeLook[] freeLookCameras;
        private CinemachineFreeLook.Orbit[] originalOrbits;

        //private CinemachineCollider m_CinemachineCollider;
        private CinemachineImpulseSource m_CinemachineImpulseSource;
        //private PostProcessVolume m_PostProcessVolume;
        //private PostProcessProfile m_PostProcessProfile;




        protected override void Awake()
        {
            base.Awake();
            _instance = this;


            m_CMBrain = GetComponentInChildren<CinemachineBrain>();




            //if (m_DefaultVCam != null)
            //{
            //    originalOrbits = new CinemachineFreeLook.Orbit[activeVCam.m_Orbits.Length];
            //    for (int i = 0; i < originalOrbits.Length; i++)
            //    {
            //        originalOrbits[i].m_Height = activeVCam.m_Orbits[i].m_Height;
            //        originalOrbits[i].m_Radius = activeVCam.m_Orbits[i].m_Radius;
            //    }
            //    #if UNITY_EDITOR
            //    SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
            //    SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrbits;
            //    #endif
            //}
            //originalZoom = freeLook.m_Orbits[1].m_Radius;
            //m_CinemachineCollider = freeLook.GetComponent<CinemachineCollider>();
            //m_CinemachineImpulseSource = freeLook.GetComponent<CinemachineImpulseSource>();
            //m_PostProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
            //m_PostProcessProfile = m_PostProcessVolume.profile;
        }


        private void Start()
        {

            currentCamera = 0;
            if(virtualCameras.Length > 0)
            {
                for (int i = 0; i < virtualCameras.Length; i++)
                {
                    virtualCameras[i].VirtualCamera.gameObject.SetActive(false);
                }

                virtualCameras[0].VirtualCamera.gameObject.SetActive(true);
            }




            m_Camera = m_CMBrain.OutputCamera; 

        }


        private void OnValidate()
        {
            if(m_FollowTarget && m_LookAtTarget != null)
            {
                for (int i = 0; i < transform.childCount; i++){
                    if (transform.GetChild(i).GetComponent<ICinemachineCamera>() != null){
                        ICinemachineCamera cmCamera = transform.GetChild(i).GetComponent<ICinemachineCamera>();
                        if (cmCamera.Follow == null) cmCamera.Follow = m_FollowTarget;
                        if (cmCamera.LookAt == null) cmCamera.LookAt = m_LookAtTarget;
                    }
                }
            }

        }



        private void InitializeVirtualCamera(CinemachineVirtualCameraBase vCam)
        {
            if(vCam == null) { return; }

            vCam.Follow = m_FollowTarget;
            vCam.LookAt = m_LookAtTarget;
        }


        public override void SetMainTarget(GameObject target)
        {
            if(m_LookAtTarget == null) m_LookAtTarget = target.transform;
            if (m_FollowTarget == null) m_FollowTarget = target.transform;

            //activeVCam.LookAt = m_LookAtTarget;
            //activeVCam.Follow = m_FollowTarget;
        }







        void Update()
        {
            //if (originalOrbits != null)
            //{
            //    zAxis.Update(Time.deltaTime);
            //    float scale = Mathf.Lerp(minScale, maxScale, zAxis.Value);
            //    for (int i = 0; i < originalOrbits.Length; i++)
            //    {
            //        activeVCam.m_Orbits[i].m_Height = originalOrbits[i].m_Height * scale;
            //        activeVCam.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * scale;
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.Tab))
            //{
            //    ToggleNextCamera(true);
            //}
        }


        protected void LateUpdate()
        {

        }




        public override void RotateCamera(float mouseX, float mouseY)
        {
            //throw new NotImplementedException();
        }

        public override void ZoomCamera(float zoomInput)
        {
            //throw new NotImplementedException();
        }


        public override bool SetCameraState(string stateName)
        {
            bool foundState = false;
            for (int i = 0; i < virtualCameras.Length; i++)
            {
                if(virtualCameras[i].StateName == stateName)
                {
                    currentCamera = i;
                    foundState = true;
                    break;
                }
                //virtualCameras[i].VirtualCamera.gameObject.SetActive(false);
            }

            for (int i = 0; i < virtualCameras.Length; i++){
                virtualCameras[i].VirtualCamera.gameObject.SetActive(currentCamera == i);
            }

            return foundState;
        }


        public void ToggleNextCamera(bool debugMsg = false)
        {
            currentCamera++;
            if (currentCamera < virtualCameras.Length)
            {
                virtualCameras[currentCamera - 1].VirtualCamera.gameObject.SetActive(false);
                virtualCameras[currentCamera].VirtualCamera.gameObject.SetActive(true);
            }
            else
            {
                //virtualCameras[currentCamera - 1].VirtualCamera.gameObject.SetActive(false);
                currentCamera = 0;
                virtualCameras[currentCamera].VirtualCamera.gameObject.SetActive(true);
            }

            if (debugMsg) Debug.LogFormat("Toggleing <b>{0}</b> on.", virtualCameras[currentCamera].VirtualCamera.Name);
        }



        public void SetActiveCamera()
        {

        }



        public void PlayImpulse()
        {

        }





//#if UNITY_EDITOR
//        private void OnDestroy()
//        {
//            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
//        }

//        private void RestoreOriginalOrbits()
//        {
//            if (originalOrbits != null)
//            {
//                for (int i = 0; i < originalOrbits.Length; i++)
//                {
//                    activeVCam.m_Orbits[i].m_Height = originalOrbits[i].m_Height;
//                    activeVCam.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius;
//                }
//            }
//        }
//#endif



    }
}

