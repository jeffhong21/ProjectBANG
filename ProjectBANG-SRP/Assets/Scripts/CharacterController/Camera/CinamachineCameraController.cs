namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using System;
    using System.Collections.Generic;
    using Cinemachine;


    public class CinamachineCameraController : CameraController
    {

        public CinemachineFreeLook freelook;
        private CinemachineFreeLook.Orbit[] originalOrbits;

        //private CinemachineCollider m_CinemachineCollider;
        //private CinemachineImpulseSource m_CinemachineImpulseSource;
        //private PostProcessVolume m_PostProcessVolume;
        //private PostProcessProfile m_PostProcessProfile;




        protected override void Awake()
        {
            base.Awake();
            _instance = this;

            m_Camera = GetComponent<Camera>();




            freelook = GetComponentInChildren<CinemachineFreeLook>();
            if (freelook != null)
            {
                originalOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];
                for (int i = 0; i < originalOrbits.Length; i++)
                {
                    originalOrbits[i].m_Height = freelook.m_Orbits[i].m_Height;
                    originalOrbits[i].m_Radius = freelook.m_Orbits[i].m_Radius;
                }
#if UNITY_EDITOR
                SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
                SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrbits;
#endif
            }
            //originalZoom = freeLook.m_Orbits[1].m_Radius;
            //m_CinemachineCollider = freeLook.GetComponent<CinemachineCollider>();
            //m_CinemachineImpulseSource = freeLook.GetComponent<CinemachineImpulseSource>();
            //m_PostProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
            //m_PostProcessProfile = m_PostProcessVolume.profile;
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
                    freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height;
                    freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius;
                }
            }
        }
#endif


        //protected void InitializeState()
        //{

        //    freelook.m_XAxis.m_MinValue = m_CameraState.MinYaw;
        //    freelook.m_XAxis.m_MaxValue = m_CameraState.MaxYaw;

        //}



        public override void SetMainTarget(GameObject target)
        {
            freelook.LookAt = target.transform;
            freelook.Follow = target.transform;
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
            //        freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * scale;
            //        freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * scale;
            //    }
            //}
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
    }
}


