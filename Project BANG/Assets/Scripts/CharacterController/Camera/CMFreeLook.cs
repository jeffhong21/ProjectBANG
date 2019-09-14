///   https://forum.unity.com/threads/free-look-camera-and-mouse-responsiveness.642886/

namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using System;
    using System.Collections;
    using Cinemachine;

    
    public class CMFreeLook
    {
        public string m_stateName;

        [SerializeField]
        private CinemachineBrain m_cmBrain;


        [Tooltip("The minimum scale for the orbits")]
        [Range(0.01f, 1f)]
        public float minScale = 0.5f;
        [Tooltip("The maximum scale for the orbits")]
        [Range(1F, 5f)]
        public float maxScale = 1;
        private CinemachineFreeLook m_cmFreeLook;
        private CinemachineFreeLook.Orbit[] m_originalOrbits;

        [Tooltip("The zoom axis.  Value is 0..1.  How much to scale the orbits")]
        [AxisStateProperty]
        public AxisState zAxis = new AxisState(0, 1, false, true, 50f, 0.1f, 0.1f, "Mouse ScrollWheel", false);


        [Tooltip("This depends on your Free Look rigs setup, use to correct Y sensitivity,"
            + " about 1.5 - 2 results in good Y-X square responsiveness")]
        public float yCorrection = 2f;

        private float xAxisValue;
        private float yAxisValue;

        //private CinemachineCollider m_CinemachineCollider;
        //private CinemachineImpulseSource m_CinemachineImpulseSource;
        // private PostProcessVolume m_postProcessVolume;
        // private PostProcessProfile m_postProcessProfile;



        public void Initialize()
        {
            m_cmFreeLook = GetComponentInChildren<CinemachineFreeLook>();
            if(m_cmFreeLook != null)
            {
                m_originalOrbits = new CinemachineFreeLook.Orbit[m_cmFreeLook.m_Orbits.Length];
                for (int i = 0; i < freelook.m_Orbits.Length; i++)
                {
                    m_originalOrbits[i].m_Height = m_cmFreeLook.m_Orbits[i].m_Height;
                    m_originalOrbits[i].m_Radius = m_cmFreeLook.m_Orbits[i].m_Radius;
                }
#if UNITY_EDITOR
                SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
                SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrbits;
#endif
            }

        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
        }
 
        private void RestoreOriginalOrbits()
        {
            if (m_originalOrbits != null)
            {
                for (int i = 0; i < m_originalOrbits.Length; i++)
                {
                    m_cmFreeLook.m_Orbits[i].m_Height = m_originalOrbits[i].m_Height;
                    m_cmFreeLook.m_Orbits[i].m_Radius = m_originalOrbits[i].m_Radius;
                }
            }
        }
#endif


        public void UpdateInput(float mouseX, float mouseY)
        {
            // Correction for Y
            mouseY /= 360f;
            mouseY *= yCorrection;
    
            xAxisValue += mouseX;
            yAxisValue = Mathf.Clamp01(yAxisValue - mouseY);
    
            m_cmFreeLook.m_XAxis.Value = xAxisValue;
            m_cmFreeLook.m_YAxis.Value = yAxisValue;
        }


        public void UpdateOrbit()
        {
            // for (int i = 0; i < m_cmFreeLook.m_Orbits.Length; i++)
            // {
            //     m_cmFreeLook.m_Orbits[i].m_Height = m_originalOrbits[i].m_Height * zoomPercent;
            //     m_cmFreeLook.m_Orbits[i].m_Radius = m_originalOrbits[i].m_Radius * zoomPercent;
            // }
            if (m_originalOrbits != null)
            {
                zAxis.Update(Time.deltaTime);
                float scale = Mathf.Lerp(minScale, maxScale, zAxis.Value);
                for (int i = 0; i < m_originalOrbits.Length; i++)
                {
                    m_cmFreeLook.m_Orbits[i].m_Height = m_originalOrbits[i].m_Height * scale;
                    m_cmFreeLook.m_Orbits[i].m_Radius = m_originalOrbits[i].m_Radius * scale;
                }
            }
        }

        public void FreeLook(bool freeLook)
        {
            if(freeLook){
                m_cmFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
                
            }

            if(freeLook == false){
                m_cmFreeLook.m_XAxis.m_InputAxisValue = 0;
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


        private float GetInputAxis(string axisName)
        {
            return !_freeLookActive ? 0 : Input.GetAxis(axisName == "Mouse Y" ? "Mouse Y" : "Mouse X");
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