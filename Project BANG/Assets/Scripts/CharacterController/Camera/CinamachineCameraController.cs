namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using System;
    using System.Collections;
    using Cinemachine;


    public class CinamachineCameraController : CameraController
    {
        public static CinamachineCameraController Controller { get; private set; }

        [Serializable]
        public class VirtualCameraState
        {
            [SerializeField]
            private string stateName;
            [SerializeField]
            private CinemachineVirtualCameraBase virtualCamera;

            private CinemachineImpulseSource impulseSource;

            //
            //  Properties
            //
            public string StateName {
                get { return stateName; }
                set {
                    stateName = value;
                    if (stateName.Length == 0) stateName = virtualCamera.name;
                }
            }

            public CinemachineVirtualCameraBase VirtualCamera { get { return virtualCamera; } set { virtualCamera = value; } }
            
            public CinemachineImpulseSource ImpulseSource { get { return impulseSource; } }

            //
            //  Constructors.
            //
            public VirtualCameraState(string stateName){
                this.stateName = stateName;
            }

            public VirtualCameraState(string stateName, CinemachineVirtualCameraBase virtualCamera)
            {
                this.stateName = stateName;
                this.virtualCamera = virtualCamera;
            }


            public void Initialize()
            {
                impulseSource = virtualCamera.GetComponent<CinemachineImpulseSource>();
            }
        }





        [Header("Cinemachine")]
        [SerializeField]
        private CinemachineBrain CMBrain;
        [Header("Targets")]
        [SerializeField]
        private Transform followTarget;
        [SerializeField]
        private Transform lookAtTarget;


        [SerializeField]
        private VirtualCameraState[] virtualCameras = { new VirtualCameraState("DEFAULT") };
        private int activeCameraIndex;
        private CinemachineVirtualCameraBase activeCamera;

        private CinemachineFreeLook[] freeLookCameras;
        private CinemachineFreeLook.Orbit[] originalOrbits;

        //private CinemachineCollider m_CinemachineCollider;
        //private CinemachineImpulseSource m_CinemachineImpulseSource;
        private PostProcessVolume postProcessVolume;
        private PostProcessProfile postProcessProfile;




        protected override void Awake()
        {
            base.Awake();
            _instance = this;
            Controller = this;

            CMBrain = GetComponentInChildren<CinemachineBrain>();
            //postProcessVolume = m_Camera.GetComponent<PostProcessVolume>();
            //postProcessProfile = postProcessVolume.profile;



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

        }


        private void Start()
        {

            activeCameraIndex = 0;
            if(virtualCameras.Length > 0)
            {
                for (int i = 0; i < virtualCameras.Length; i++)
                {
                    virtualCameras[i].Initialize();
                    virtualCameras[i].VirtualCamera.gameObject.SetActive(false);
                }

                virtualCameras[0].VirtualCamera.gameObject.SetActive(true);
            }

            m_Camera = CMBrain.OutputCamera;

            if(followTarget != null && virtualCameras[0].VirtualCamera is CinemachineFreeLook)
            {
                var cmFreeLook = (CinemachineFreeLook)virtualCameras[0].VirtualCamera;
                cmFreeLook.m_XAxis.Value = followTarget.eulerAngles.y;
            }
        }


        private void OnValidate()
        {
            if(followTarget && lookAtTarget != null)
            {
                for (int i = 0; i < transform.childCount; i++){
                    if (transform.GetChild(i).GetComponent<ICinemachineCamera>() != null){
                        ICinemachineCamera cmCamera = transform.GetChild(i).GetComponent<ICinemachineCamera>();
                        if (cmCamera.Follow == null) cmCamera.Follow = followTarget;
                        if (cmCamera.LookAt == null) cmCamera.LookAt = lookAtTarget;

                    }
                }
            }

        }



        private void InitializeVirtualCamera(CinemachineVirtualCameraBase vCam)
        {
            if(vCam == null) { return; }

            vCam.Follow = followTarget;
            vCam.LookAt = lookAtTarget;
        }


        public override void SetMainTarget(GameObject target)
        {
            if(lookAtTarget == null) lookAtTarget = target.transform;
            if (followTarget == null) followTarget = target.transform;

            //activeVCam.LookAt = lookAtTarget;
            //activeVCam.Follow = followTarget;
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
                    activeCameraIndex = i;
                    foundState = true;
                    break;
                }
                //virtualCameras[i].VirtualCamera.gameObject.SetActive(false);
            }
            activeCamera = virtualCameras[activeCameraIndex].VirtualCamera;
            activeCamera.gameObject.SetActive(true);
            for (int i = 0; i < virtualCameras.Length; i++)
            {
                //if (virtualCameras[i].VirtualCamera == activeCamera) continue;
                virtualCameras[i].VirtualCamera.gameObject.SetActive(i == activeCameraIndex);
                //if (virtualCameras[i].VirtualCamera.Priority > activeCamera.Priority)
                //{
                //    virtualCameras[i].VirtualCamera.gameObject.SetActive(i == activeCameraIndex);
                //}
                
            }

            return foundState;
        }






        public void ToggleNextCamera(bool debugMsg = false)
        {
            activeCameraIndex++;
            if (activeCameraIndex < virtualCameras.Length)
            {
                virtualCameras[activeCameraIndex - 1].VirtualCamera.gameObject.SetActive(false);
                virtualCameras[activeCameraIndex].VirtualCamera.gameObject.SetActive(true);
            }
            else
            {
                //virtualCameras[activeCameraIndex - 1].VirtualCamera.gameObject.SetActive(false);
                activeCameraIndex = 0;
                virtualCameras[activeCameraIndex].VirtualCamera.gameObject.SetActive(true);
            }

            if (debugMsg) Debug.LogFormat("Toggleing <b>{0}</b> on.", virtualCameras[activeCameraIndex].VirtualCamera.Name);
        }



        public CinemachineVirtualCameraBase GetActiveCamera(){
            return virtualCameras[activeCameraIndex].VirtualCamera;
        }



        public void PlayImpulse()
        {
            if (virtualCameras[activeCameraIndex].ImpulseSource != null)
                virtualCameras[activeCameraIndex].ImpulseSource.GenerateImpulse(Vector3.right);
            else Debug.Log("<color=yellow> No Impulse Source</color>");

            //CinemachineVirtualCameraBase vCam = GetActiveCamera() as CinemachineVirtualCameraBase;
            //StartCoroutine(ShakeVCam(GetActiveCamera(), 1, 1, 1));
        }


        public IEnumerator ShakeVCam( CinemachineVirtualCamera vCam, float amp, float freq, float duration )
        {

            CinemachineBasicMultiChannelPerlin noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            noise.m_AmplitudeGain = amp;
            noise.m_FrequencyGain = freq;
            yield return new WaitForSeconds(duration);

            noise.m_AmplitudeGain = 0;
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


//void Update()
//{
//    //if (originalOrbits != null)
//    //{
//    //    zAxis.Update(Time.deltaTime);
//    //    float scale = Mathf.Lerp(minScale, maxScale, zAxis.Value);
//    //    for (int i = 0; i < originalOrbits.Length; i++)
//    //    {
//    //        activeVCam.m_Orbits[i].m_Height = originalOrbits[i].m_Height * scale;
//    //        activeVCam.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * scale;
//    //    }
//    //}

//    //if (Input.GetKeyDown(KeyCode.Tab))
//    //{
//    //    ToggleNextCamera(true);
//    //}
//}