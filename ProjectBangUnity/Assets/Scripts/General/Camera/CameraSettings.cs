namespace Bang
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    [CreateAssetMenu(menuName =  "Controller/Camera Settings")]
    public class CameraSettings : ScriptableObject
    {
        
        public float moveSpeed = 9;


        public float distanceFromTarget = -16;

        public float zoomSmooth = 100;

        public float zoomStep = 2;

        public float maxZoom = -10;

        public float minZooom = -30;

        public float smooth = 0.05f;


        public float xRotation = -65;

        public float yRotation = -180;

        public float yOrbitSmooth = 0.5f;
    }

}


