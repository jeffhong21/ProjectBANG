    namespace CharacterController
{
    using UnityEngine;
    using System;


    public class Cover : CharacterAction
    {
        private readonly bool hideflags = true;




        [Header("-- Cover Settings --")]
        [SerializeField]
        protected float m_TakeCoverDistance = 2f;
        [SerializeField]
        protected LayerMask m_CoverLayer;
        [SerializeField]
        protected float m_CoverMovementSpeed = 1;
        [SerializeField]
        protected float m_TakeCoverRotationSpeed = 4;

        [SerializeField]
        protected float m_ObjectDetectorHeight = 0.5f;
        [SerializeField]
        protected float m_HighCoverHeight = 1.35f;
        [SerializeField]
        protected float m_SideCoverDistance = 0.5f;


        protected float m_Spacing;

        protected Transform m_DetectorHolder;
        protected Transform m_ObjectDetector;
        protected Transform m_HighCoverDetector;
        protected Transform m_RightCoverPopup;
        protected Transform m_LeftCoverPopup;


        protected RaycastHit m_ObjectDetectorHit;
        protected RaycastHit m_RotationDetectorHit, m_EdgeDetectionHit;

        protected Quaternion m_TargetRotation;
        protected Vector3 m_TargetPosition;


        protected Vector3 m_Velocity;
        protected int m_Direction;

        protected bool m_HighCover;
        protected float m_CheckCoverLength = 0.65f;

        protected float m_HorizontalInput, m_ForwardInput;
        protected Vector3 m_StartPosition, m_StopPosition;       //  Stop position when hitting side edge of cover

        protected bool m_CanPopLeft, m_CanPopRight;





		protected override void Awake()
        {
            base.Awake();

            m_DetectorHolder = new GameObject("Cover Detector Holder").transform;
            m_DetectorHolder.parent = m_Transform;
            if(hideflags) m_DetectorHolder.hideFlags = HideFlags.HideInHierarchy;
            m_DetectorHolder.localPosition = m_DetectorHolder.localEulerAngles = Vector3.zero;
            m_ObjectDetector = CreateDetectors("Object_Detection", Vector3.up * m_ObjectDetectorHeight);
            m_HighCoverDetector = CreateDetectors("High_Cover_Detector", Vector3.up * m_HighCoverHeight);
            m_RightCoverPopup = CreateDetectors("Right_Cover_Detector", Vector3.right * m_SideCoverDistance + (Vector3.up * m_ObjectDetectorHeight));
            m_LeftCoverPopup = CreateDetectors("Left_Cover_Detector", Vector3.left * m_SideCoverDistance + (Vector3.up * m_ObjectDetectorHeight));

            //m_CoverLayer = 9;
		}


        private Transform CreateDetectors(string detectorName, Vector3 localPosition)
        {
            var detector = new GameObject(detectorName).transform;
            detector.parent = m_DetectorHolder;
            detector.localPosition = localPosition;
            detector.localEulerAngles = Vector3.zero;
            return detector;
        }



        //  Start
		public override bool CanStartAction()
        {
            if(base.CanStartAction())
            {
                RaycastHit startLocationHit;
                if (Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, out m_ObjectDetectorHit, m_TakeCoverDistance, m_CoverLayer))
                {
                    m_TargetPosition = m_ObjectDetectorHit.point + (m_ObjectDetectorHit.normal * (m_CapsuleCollider.radius * 0.45f));
                    m_TargetPosition.y = m_Controller.transform.position.y;
                    return true;
                }
            }

            return false;
        }


        protected override void ActionStarted()
        {
            m_HighCover = Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_TakeCoverDistance, m_CoverLayer);
            m_Animator.SetFloat(HashID.Height, m_HighCover ? 1 : 0.5f);

            MoveToTarget(m_TargetPosition, Quaternion.identity, 1, null);
            //Debug.LogFormat("Dot product.  {0}", Vector3.Dot(m_Transform.right, m_ObjectDetectorHit.normal));
            //var directionVector = Vector3.Reflect(m_Transform.forward * 2, m_ObjectDetectorHit.normal);
        }





		protected override void ActionStopped()
        {
            m_HighCover = Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_CheckCoverLength, m_CoverLayer);
            m_Animator.SetFloat(HashID.Height, m_HighCover ? 1 : 0.5f);

            m_Animator.SetInteger(HashID.ActionID, 0);
            //m_Animator.SetInteger(HashID.ActionIntData, emergeIndex);
            //m_Animator.SetFloat(HashID.Height, 1);

            m_Controller.LookRotation = m_Transform.rotation;

        }




		public override bool CheckMovement()
		{
            m_HorizontalInput = m_Controller.InputVector.x;

            if(m_HorizontalInput > 0) m_Direction = 1;
            else if (m_HorizontalInput < 0) m_Direction = -1;
            else m_Direction = 0;
            //m_Direction = Mathf.RoundToInt(Mathf.Clamp(m_HorizontalInput, -1, 1));

            //  Execute only when the character is moving.
            if (Mathf.Abs(m_HorizontalInput) > 0.1f)
            {
                //  -- Check if character has reached the edge of the cover. --
                if (Physics.Raycast(m_LeftCoverPopup.position, m_LeftCoverPopup.forward, out m_EdgeDetectionHit, m_CheckCoverLength, m_CoverLayer) == false)
                {
                    m_CanPopLeft = true;
                    m_StartPosition = m_Transform.position;
                    m_StopPosition = m_Transform.position - (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                    m_StopPosition = m_StopPosition + m_Transform.forward * 0.892f;
                }
                //  If Left Cover Popup does not hit a cover object, than character cannot move left..  
                else if (Physics.Raycast(m_RightCoverPopup.position, m_RightCoverPopup.forward, out m_EdgeDetectionHit, m_CheckCoverLength, m_CoverLayer) == false)
                {
                    m_CanPopRight = true;
                    m_StartPosition = m_Transform.position;
                    m_StopPosition = m_Transform.position + (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                    m_StopPosition = m_StopPosition + m_Transform.forward * 0.892f;
                }
                else
                {
                    m_CanPopRight = m_CanPopLeft = false;
                    m_StartPosition = m_StopPosition = Vector3.zero;
                }

            }
            return true;
		}


        [SerializeField]
        private float alignmentAngle;
        [SerializeField]
        private Vector3 rotationCheckPosition;
        private Vector3 moveDirection;
		public override bool UpdateRotation()
        {
            m_TargetRotation = m_Transform.rotation;
            //rotationCheckPosition = m_ObjectDetector.position + (m_Direction * Vector3.right * m_CapsuleCollider.radius) + (-Vector3.forward * m_CapsuleCollider.radius);
            rotationCheckPosition = m_ObjectDetector.position + (m_Direction * Vector3.right * m_CapsuleCollider.radius);

            if (Mathf.Abs(m_HorizontalInput) > 0.1)
            {
                if (Physics.Raycast(rotationCheckPosition, m_ObjectDetector.forward, out m_RotationDetectorHit, m_CheckCoverLength, m_CoverLayer))
                {
                    //moveDirection = Vector3.Cross(m_RotationDetectorHit.normal, m_Transform.up);
                    //moveDirection = moveDirection.normalized * m_Direction;
                    //m_TargetRotation = Quaternion.LookRotation(moveDirection, m_Transform.up) * m_Transform.rotation;

                    m_TargetRotation = Quaternion.FromToRotation(-m_Transform.forward, m_RotationDetectorHit.normal) * m_Transform.rotation;
                    //m_TargetRotation.Normalize();
                }
            }

            alignmentAngle = Quaternion.Angle(m_Transform.rotation, m_TargetRotation);

            //m_Transform.rotation = m_TargetRotation;
            m_Transform.rotation = Quaternion.Lerp(m_TargetRotation, m_Transform.rotation, m_DeltaTime);
            //m_Transform.rotation = Quaternion.RotateTowards(m_Transform.rotation, m_TargetRotation,  m_DeltaTime);


            return false;
        }



        //  Only allow movement on the relative x axis to prevent the character from moving away from the cover point.
        public override bool UpdateMovement()
        {

            //  Execute only when the character is moving.
            if (Mathf.Abs(m_HorizontalInput) > 0.1f){
                m_HighCover = Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_CheckCoverLength, m_CoverLayer) ? true : false;
            }

            //moveDirection = Vector3.Cross(m_Transform.right * m_Direction, m_Transform.up);
            if (Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, out m_ObjectDetectorHit, m_CheckCoverLength, m_CoverLayer))
            {
                var hitLocation = m_ObjectDetectorHit.point;
                hitLocation.y = m_Transform.position.y;
                m_Spacing = (hitLocation - m_Transform.position).magnitude;
                if (m_Spacing > m_CapsuleCollider.radius + 0.02f)
                {
                    m_Transform.position = Vector3.Lerp(m_Transform.position, m_Transform.position + m_Transform.forward * (m_Spacing - m_CapsuleCollider.radius + 0.02f), m_DeltaTime);
                    //m_Transform.position = m_Transform.position + (Vector3.forward * (m_Spacing - m_CapsuleCollider.radius + 0.02f) * m_DeltaTime);
                }
            }


            if(m_CanPopLeft){
                m_HorizontalInput = Mathf.Clamp(m_HorizontalInput, 0, 1);
                m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, 0);
            }
            else if (m_CanPopRight){
                m_HorizontalInput = Mathf.Clamp(m_HorizontalInput, -1, 0);
                m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, 0);
            }
            else{
                m_HorizontalInput = m_Controller.InputVector.x;
                m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, 0);
            }
            m_Velocity = m_Transform.right * m_HorizontalInput * m_CoverMovementSpeed;




            return false;
        }


        public override bool Move()
        {
            m_Velocity += m_Animator.deltaPosition / m_DeltaTime;
            m_Controller.Velocity = m_Velocity;
            m_Rigidbody.velocity = m_Velocity;
            //m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, m_Controller.Velocity, 20 * m_DeltaTime);

            return false;
        }



        public override bool UpdateAnimator()
        {
            //  Is Moving.
            m_Animator.SetBool(HashID.Moving, Mathf.Abs(m_HorizontalInput) > 0.1f);
            //m_AnimatorMonitor.SetIntDataValue((int)m_CurrentCoverID);

            //m_Animator.SetBool(HashID.Crouching, !m_HighCover);
            m_Animator.SetFloat(HashID.Height, m_HighCover ? 1 : 0.5f, 0.2f, m_DeltaTime);

            m_AnimatorMonitor.SetHorizontalInputValue(m_HorizontalInput);
            //if ((m_CanPopLeft || m_CanPopRight) && m_HorizontalInput == 0 && Mathf.Abs(m_Direction) == 1)
            //{
            //    m_Animator.SetInteger(HashID.ActionIntData, m_Direction);
            //    m_AnimatorMonitor.SetHorizontalInputValue(m_HorizontalInput);
            //} else {
            //    m_Animator.SetInteger(HashID.ActionIntData, 0);
            //    m_AnimatorMonitor.SetHorizontalInputValue(m_HorizontalInput);
            //}

            m_ForwardInput = 0;
            m_AnimatorMonitor.SetForwardInputValue(m_ForwardInput);

            m_Animator.SetInteger(HashID.ActionID, m_ActionID);


            //m_Animator.SetInteger(HashID.ActionIntData, m_ActionID);
            return false;
        }








        [Header("--  Debug Settings --")]
        [SerializeField] DebugSettings m_DebugSettings = new DebugSettings();
        [SerializeField] bool m_Debug;

        [Serializable]
        public class DebugSettings
        {
            [SerializeField]
            private float m_GizmoSize = 0.1f;
            [SerializeField]
            private Vector3 m_CubeSize;
            [SerializeField]
            private Color m_CenterColor = Color.cyan;
            [SerializeField]
            private Color m_LeftColor = Color.green;
            [SerializeField]
            private Color m_RightColor = Color.red;
            [SerializeField]
            private Color m_ChangeStateColor = Color.green;


            public float GizmoSize{ get { return m_GizmoSize; } }

            public Vector3 CubeSize{ get { return m_CubeSize; }}

            public Color CenterColor { get { return m_CenterColor; } }
            public Color LeftColor { get { return m_LeftColor; } }
            public Color RightColor { get { return m_RightColor; } }
            public Color ChangeStateColor { get { return m_ChangeStateColor; } }

            public DebugSettings()
            {
                m_CubeSize = new Vector3(m_GizmoSize, m_GizmoSize, m_GizmoSize);
            }
        }


        protected virtual void OnDrawGizmosSelected()
        {
            if(Application.isPlaying && m_Debug)
            {
                Gizmos.color = m_DebugSettings.CenterColor;
                Gizmos.DrawRay(m_ObjectDetector.position, m_ObjectDetector.forward * m_CheckCoverLength);

                Gizmos.color = m_HighCover ? m_DebugSettings.CenterColor : m_DebugSettings.ChangeStateColor;
                Gizmos.DrawRay(m_HighCoverDetector.position, m_HighCoverDetector.forward * m_CheckCoverLength);

                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(rotationCheckPosition, 0.1f);

                Gizmos.DrawRay(rotationCheckPosition, m_ObjectDetector.forward);


                Gizmos.color = Color.red;
                Gizmos.DrawRay(m_ObjectDetector.position, moveDirection);

                if (m_LeftCoverPopup && m_RightCoverPopup)
                {
                    Gizmos.color = m_CanPopLeft ?  m_DebugSettings.ChangeStateColor : m_DebugSettings.LeftColor;
                    Gizmos.DrawRay(m_LeftCoverPopup.position, m_LeftCoverPopup.forward * m_CheckCoverLength);
                    //Gizmos.DrawCube(m_LeftCoverPopup.position, d_GizmoCubeSize);
                    Gizmos.DrawSphere(m_LeftCoverPopup.position, m_DebugSettings.GizmoSize);
                    GizmosUtils.DrawString("Left Cover", m_LeftCoverPopup.position + Vector3.up * 0.5f, Color.black);
                    if ( m_CanPopLeft){
                        Gizmos.color = m_DebugSettings.LeftColor;
                        Gizmos.DrawWireSphere(m_StopPosition, 0.25f);
                    }


                    Gizmos.color = m_CanPopRight ? m_DebugSettings.ChangeStateColor : m_DebugSettings.RightColor;
                    Gizmos.DrawRay(m_RightCoverPopup.position, m_RightCoverPopup.forward * m_CheckCoverLength);
                    //Gizmos.DrawCube(m_RightCoverPopup.position, d_GizmoCubeSize);
                    Gizmos.DrawSphere(m_RightCoverPopup.position, m_DebugSettings.GizmoSize);
                    GizmosUtils.DrawString("Right Cover", m_RightCoverPopup.position + Vector3.up * 0.5f, Color.black);
                    if(m_CanPopRight){
                        Gizmos.color = m_DebugSettings.RightColor;
                        Gizmos.DrawWireSphere(m_StopPosition, 0.25f);
                    }

                }
            }

        }


        GUIStyle style = new GUIStyle();
        GUIContent content = new GUIContent();
        Vector2 size;
        Color debugTextColor = new Color(0, 0.6f, 1f, 1);
        GUIStyle textStyle = new GUIStyle();
        Rect location = new Rect();
        private void OnGUI()
        {
            if (Application.isPlaying && m_Debug && m_IsActive)
            {
                GUI.color = debugTextColor;
                textStyle.fontStyle = FontStyle.Bold;
                content.text = string.Format("Alignment Angle: {0}\n", alignmentAngle.ToString());
                content.text += string.Format("Direction: {0} | Horizontal Input: {1}\n", m_Direction.ToString(), m_HorizontalInput.ToString());
                content.text += string.Format("Spacing: {0} | Distance: {1}\n", m_Spacing, m_ObjectDetectorHit.distance);
                size = new GUIStyle(GUI.skin.label).CalcSize(content);
                location.Set(5, 15, size.x * 2, size.y * 2);
                GUILayout.BeginArea(location);
                GUILayout.Label(content);
                //GUILayout.Label(string.Format("Normalized Time: {0}", normalizedTime.ToString()));
                GUILayout.EndArea();
            }

        }


    }

}

