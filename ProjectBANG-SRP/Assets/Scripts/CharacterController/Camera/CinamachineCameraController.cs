namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using System;
    using System.Collections.Generic;
    using Cinemachine;


    public class CinamachineCameraController : CameraController
    {
        [Header("Cinemachine")]
        [SerializeField]
        private CinemachineBrain m_CMBrain;
        [SerializeField]
        private CinemachineFreeLook m_DefaultVCam;
        [SerializeField]
        private CinemachineFreeLook m_AimVCam;
        [Header("Targets")]
        [SerializeField]
        private Transform m_FollowTarget;
        [SerializeField]
        private Transform m_LookAtTarget;


        private CinemachineFreeLook activeVCam;
        private int currentCamera;
        private CinemachineFreeLook[] m_VCameraList;
        private CinemachineFreeLook.Orbit[] originalOrbits;

        //private CinemachineCollider m_CinemachineCollider;
        //private CinemachineImpulseSource m_CinemachineImpulseSource;
        //private PostProcessVolume m_PostProcessVolume;
        //private PostProcessProfile m_PostProcessProfile;




        protected override void Awake()
        {
            base.Awake();
            _instance = this;


            m_CMBrain = GetComponentInChildren<CinemachineBrain>();




//            if (m_DefaultVCam != null)
//            {
//                originalOrbits = new CinemachineFreeLook.Orbit[activeVCam.m_Orbits.Length];
//                for (int i = 0; i < originalOrbits.Length; i++)
//                {
//                    originalOrbits[i].m_Height = activeVCam.m_Orbits[i].m_Height;
//                    originalOrbits[i].m_Radius = activeVCam.m_Orbits[i].m_Radius;
//                }
//#if UNITY_EDITOR
//                SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
//                SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrbits;
//#endif
//            }
            //originalZoom = freeLook.m_Orbits[1].m_Radius;
            //m_CinemachineCollider = freeLook.GetComponent<CinemachineCollider>();
            //m_CinemachineImpulseSource = freeLook.GetComponent<CinemachineImpulseSource>();
            //m_PostProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
            //m_PostProcessProfile = m_PostProcessVolume.profile;
        }


        private void Start()
        {
            m_Camera = m_CMBrain.OutputCamera; 
            var cameraList = new List<CinemachineFreeLook>();

            cameraList.Add(m_DefaultVCam);
            cameraList.Add(m_AimVCam);

            m_VCameraList = cameraList.ToArray();
            for (int i = 0; i < m_VCameraList.Length; i++)
            {
                InitializeVirtualCamera(m_VCameraList[i]);
                m_VCameraList[i].gameObject.SetActive(false);
            }
            
            activeVCam = m_DefaultVCam;
            activeVCam.gameObject.SetActive(true);
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



        //public bool ChangeCameraState(CameraState state)
        //{
        //    if (m_CameraStateLookup.ContainsKey(state.name))
        //    {
        //        m_CameraState = m_CameraStateLookup[state.name];
        //        InitializeState();
        //        Debug.LogFormat("CameraState: {0}", m_CameraState.name);
        //        return true;
        //    }
        //    Debug.LogFormat("** Camera State {0} does not exist", name);
        //    return false;
        //}




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

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                foreach (var vCam in m_VCameraList)
                {
                    Debug.Log(vCam.Name + ": Priority:" + vCam.Priority);
                }
            }
        }


        protected void LateUpdate()
        {

        }

        public override CameraState GetCameraStateWithName(string name)
        {
            throw new NotImplementedException();
        }

        public override bool ChangeCameraState(CameraState state)
        {
            throw new NotImplementedException();
        }

        public override void RotateCamera(float mouseX, float mouseY)
        {
            //throw new NotImplementedException();
        }

        public override void ZoomCamera(float zoomInput)
        {
            //throw new NotImplementedException();
        }





#if UNITY_EDITOR
        private void OnDestroy()
        {
            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
        }

        private void RestoreOriginalOrbits()
        {
            if (originalOrbits != null)
            {
                for (int i = 0; i < originalOrbits.Length; i++)
                {
                    activeVCam.m_Orbits[i].m_Height = originalOrbits[i].m_Height;
                    activeVCam.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius;
                }
            }
        }
#endif



    }
}


