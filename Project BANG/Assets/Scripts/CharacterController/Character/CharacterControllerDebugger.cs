namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CharacterControllerDebugger
    {
        RigidbodyCharacterController character;
        Transform transform;

        [Serializable]
        public class DebugModeStates
        {
            public bool showGroundCheck;
            public bool showCollisions;
            public bool showMotion;
        }

        [Serializable]
        public class DebugColors
        {
            public Color moveDirectionColor = Color.blue;
            public Color velocityColor = Color.green;
            public Color magenta = new Color(0.75f, 0, 0.75f, 0.9f);
        }


        public bool debugMode;

        public DebugModeStates states;
        public DebugColors colors;
        [Range(0.01f, 1f)]
        public float heightOffset = 0.05f;


        private Queue<Delegate> drawActions;





        protected Vector3 VectorOffset { get { return new Vector3(0, heightOffset, 0); } }



        public CharacterControllerDebugger(){ }

        public CharacterControllerDebugger(CharacterControllerDebugger debugger)
        {
            Initialize(debugger.character);
        }

        public void Initialize(RigidbodyCharacterController character)
        {
            this.character = character;
            this.transform = character.transform;
        }


        public void DrawRayFromOrigin(Vector3 direction, float heightOffset)
        {
            var start = transform.position + Vector3.up * heightOffset;
            var end = transform.TransformDirection(direction) + start;
            DrawRay(start, end, Color.blue);
        }



        private void DrawRay(Vector3 start, Vector3 end, Color color, bool drawEndPosition = true)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);
        }


        public void OnDrawGizmos()
        {
            if (!debugMode) return;
        }

    }
}


