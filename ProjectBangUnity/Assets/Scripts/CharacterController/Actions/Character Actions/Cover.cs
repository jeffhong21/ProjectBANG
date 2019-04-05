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
        protected float m_TakeCoverRotationSpeed = 4;

        [SerializeField]
        protected float m_ObjectDetectorHeight = 0.5f;
        [SerializeField]
        protected float m_HighCoverHeight = 1.35f;
        [SerializeField]
        protected float m_SideCoverDistance = 0.5f;

        protected Transform m_DetectorHolder;
        protected Transform m_ObjectDetector;
        protected Transform m_HighCoverDetector;
        protected Transform m_RightCoverPopup;
        protected Transform m_LeftCoverPopup;


        protected RaycastHit m_RaycastHit;
        protected Quaternion m_TargetRotation;
        protected Vector3 m_TargetPosition;

       

        protected bool m_HighCover;
        protected float m_PopoutLength = 0.5f;
        [SerializeField]
        protected float m_HorizontalInput, m_ForwardInput;
        protected Vector3 m_StartPosition, m_StopPosition;       //  Stop position when hitting side edge of cover
        [SerializeField]
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
                RaycastHit hit;
                if (Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, out hit, m_TakeCoverDistance, m_CoverLayer))
                {
                    m_TargetPosition = hit.point + (hit.normal * (m_CapsuleCollider.radius * 0.33f));
                    m_TargetPosition.y = m_Controller.transform.position.y;
                    return true;
                }
            }

            return false;
        }


        protected override void ActionStarted()
        {
            if (Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_TakeCoverDistance, m_CoverLayer)){
                m_HighCover = true;
            } else {
                m_HighCover = false;
            }

            //m_Controller.SetPosition(m_TargetPosition);
            var action = m_Controller.GetAction<MoveTowards>();
            if(action != null){
                if (!action.IsActive){
                    action.ActionStartLocation = m_TargetPosition;
                    m_Controller.TryStartAction(action);
                }
            }
        }





		protected override void ActionStopped()
        {

            //  Get merge index.
            var emergeIndex = 0;
            if (m_CanPopLeft || m_CanPopRight) emergeIndex = 1;
            //if(!m_HighCover)
                //emergeIndex = 2;

            m_Animator.SetInteger(HashID.ActionID, 0);
            m_Animator.SetInteger(HashID.ActionIntData, emergeIndex);
            m_Animator.SetFloat(HashID.Height, 1);

            m_Controller.LookRotation = m_Transform.rotation;

            if (m_CanPopLeft || m_CanPopRight){
                var action = m_Controller.GetAction<MoveTowards>();
                if (action != null){
                    if (!action.IsActive){
                        action.ActionStartLocation = m_StopPosition;
                        m_Controller.TryStartAction(action);
                    }
                }
            }

        }



		public override bool UpdateRotation()
        {
            if (Mathf.Abs(m_Controller.InputVector.x) > 0.2)
            {
                //Debug.DrawRay(m_ObjectDetector.position, m_ObjectDetector.forward, Color.magenta);
                if (Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, out m_RaycastHit, m_TakeCoverDistance, m_CoverLayer)){
                    m_TargetRotation = Quaternion.FromToRotation(-m_Transform.forward, m_RaycastHit.normal) * m_Transform.rotation;
                    m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, m_TargetRotation.normalized, m_TakeCoverRotationSpeed * Time.deltaTime));
                }
            }
            return false;
        }


        public override bool UpdateMovement()
        {
            m_HorizontalInput = m_Controller.InputVector.x;

            //  Execute only when the character is moving.
            if (Mathf.Abs(m_HorizontalInput) > 0.2f)
            {
                //  --  Set character height --
                if (Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_PopoutLength, m_CoverLayer)){
                    m_HighCover = true;
                }
                else{
                    m_HighCover = false;
                }


                //  -- Check if character has reached the edge of the cover. --
                // Moving to the Left.
                //  If Left Cover Popup does not hit a cover object, than character cannot move left..  
                if (!Physics.Raycast(m_LeftCoverPopup.position, m_LeftCoverPopup.forward, m_PopoutLength, m_CoverLayer))
                {
                    m_CanPopLeft = true;
                    m_StartPosition = m_Transform.position;
                    m_StopPosition = m_Transform.position - (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                    m_StopPosition = m_StopPosition + m_Transform.forward * 0.892f;
                    // Set the InputVector.
                    m_HorizontalInput = Mathf.Clamp(m_HorizontalInput, 0, 1);
                    m_ForwardInput = m_Controller.InputVector.z;

                    m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, m_ForwardInput);
                    //  Move the character.
                    m_Controller.Velocity = m_Transform.right * m_HorizontalInput * Time.deltaTime;
                    m_Transform.position += m_Controller.Velocity;
                }
                else if (!Physics.Raycast(m_RightCoverPopup.position, m_RightCoverPopup.forward, m_PopoutLength, m_CoverLayer))
                {
                    m_CanPopRight = true;
                    m_StartPosition = m_Transform.position;
                    m_StopPosition = m_Transform.position + (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                    m_StopPosition = m_StopPosition + m_Transform.forward * 0.892f;
                    // Set the InputVector.
                    m_HorizontalInput = Mathf.Clamp(m_HorizontalInput, -1, 0);
                    m_ForwardInput = m_Controller.InputVector.z;

                    m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, m_ForwardInput);
                    //  Move the character.
                    m_Controller.Velocity = m_Transform.right * m_HorizontalInput * Time.deltaTime;
                    m_Transform.position += m_Controller.Velocity;
                }
                else
                {
                    m_CanPopRight = m_CanPopLeft = false;
                    m_StartPosition = m_StopPosition = Vector3.zero;

                    // Set the InputVector.
                    m_HorizontalInput = m_Controller.InputVector.x;
                    m_ForwardInput = 0;
                    m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, m_ForwardInput);
                    //  Move the character.
                    m_Controller.Velocity = m_Transform.right * m_HorizontalInput * Time.deltaTime;
                    m_Transform.position += m_Controller.Velocity;
                }




            }




            if (m_Debug) Debug.DrawRay(m_ObjectDetector.position, m_ObjectDetector.forward * m_PopoutLength, m_DebugSettings.CenterColor);
            if (m_Debug) Debug.DrawRay(m_LeftCoverPopup.position, m_LeftCoverPopup.forward * m_PopoutLength, m_CanPopLeft ? m_DebugSettings.ChangeStateColor : m_DebugSettings.LeftColor);
            if (m_Debug) Debug.DrawRay(m_RightCoverPopup.position, m_RightCoverPopup.forward * m_PopoutLength, m_CanPopRight ? m_DebugSettings.ChangeStateColor : m_DebugSettings.RightColor);
            if (m_Debug) Debug.DrawRay(m_HighCoverDetector.position, m_HighCoverDetector.forward * m_PopoutLength, m_HighCover ? m_DebugSettings.ChangeStateColor : m_DebugSettings.CenterColor);
            return false;
        }




        public override bool UpdateAnimator()
        {
            //m_AnimatorMonitor.SetIntDataValue((int)m_CurrentCoverID);

            m_AnimatorMonitor.SetHorizontalInputValue(m_HorizontalInput);
            m_AnimatorMonitor.SetForwardInputValue(m_ForwardInput);
            m_Animator.SetInteger(HashID.ActionID, m_ActionID);
            m_Animator.SetBool(HashID.Crouching, !m_HighCover);
            m_Animator.SetFloat(HashID.Height, m_HighCover ? 1 : 0.5f);

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
            private float m_GizmoSize = 0.2f;
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



        private void DrawDebugLine(Transform detector, float lineLegnth, bool hitCover, Color defaultColor)
        {
            Debug.DrawLine(detector.position, detector.position + (detector.forward * lineLegnth), hitCover ? Color.green : defaultColor);
        }



        private void DrawGizmoLine(Transform detector, bool hitCover)
        {
            Gizmos.color = hitCover ? Color.green : Color.red;
            Gizmos.DrawLine(detector.position, detector.position + (detector.forward * m_TakeCoverDistance));
        }

        Vector3 d_GizmoCubeSize = new Vector3(0.2f, 0.2f, 0.2f);

        protected virtual void OnDrawGizmosSelected()
        {
            if(Application.isPlaying && m_Debug)
            {
                //if (m_Debug){
                //    if (m_LeftCoverPopup) DrawGizmoLine(m_LeftCoverPopup, _hitLeft);
                //    if (m_RightCoverPopup) DrawGizmoLine(m_RightCoverPopup, _hitRight);
                //}

                if (m_LeftCoverPopup && m_RightCoverPopup)
                {
                    Gizmos.color = m_CanPopLeft ?  m_DebugSettings.ChangeStateColor : m_DebugSettings.LeftColor;
                    Gizmos.DrawCube(m_LeftCoverPopup.position, d_GizmoCubeSize);
                    GizmosUtils.DrawString("Left Cover", m_LeftCoverPopup.position + Vector3.up * 0.5f, Color.black);
                    if ( m_CanPopLeft){
                        Gizmos.color = m_DebugSettings.LeftColor;
                        Gizmos.DrawWireSphere(m_StopPosition, 0.25f);
                    }
                    //Gizmos.DrawRay(m_LeftCoverPopup.position, m_LeftCoverPopup.forward * m_PopoutLength);

                    //Debug.DrawRay(m_LeftCoverPopup.position, m_LeftCoverPopup.forward, m_CanPopLeft ? Color.green : Color.magenta);

                    Gizmos.color = m_CanPopRight ? m_DebugSettings.ChangeStateColor : m_DebugSettings.RightColor;
                    Gizmos.DrawCube(m_RightCoverPopup.position, d_GizmoCubeSize);
                    GizmosUtils.DrawString("Right Cover", m_RightCoverPopup.position + Vector3.up * 0.5f, Color.black);
                    if(m_CanPopRight){
                        Gizmos.color = m_DebugSettings.RightColor;
                        Gizmos.DrawWireSphere(m_StopPosition, 0.25f);
                    }
                    //Gizmos.DrawRay(m_LeftCoverPopup.position, m_LeftCoverPopup.forward * m_PopoutLength);
                }
            }

        }




        //private bool CanEnterCover(bool drawAllLines = false)
        //{
        //    float coverViewAngle = 135;
        //    int numOfRays = 6;

        //    float halfAngle = coverViewAngle / 2.0f;
        //    Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        //    Vector3 leftRayDirection = leftRayRotation * (m_ObjectDetector.forward * m_TakeCoverDistance);
        //    Vector3 direction = leftRayDirection;

        //    float amountEach = coverViewAngle / (numOfRays);
        //    for (int i = 0; i < numOfRays; i++)
        //    {
        //        if (i == 0) continue;
        //        Quaternion desiredRotation = Quaternion.AngleAxis(amountEach, Vector3.up);
        //        direction = desiredRotation * direction;

        //        bool hit = false;
        //        if (Physics.Raycast(m_ObjectDetector.position, direction, out m_RaycastHit, m_TakeCoverDistance, m_CoverLayer))
        //        {
        //            if (!drawAllLines) Debug.DrawRay(m_ObjectDetector.position, direction, hit ? Color.green : Color.magenta, 1);
        //            hit = true;
        //            m_TargetPosition = m_RaycastHit.point + (m_RaycastHit.normal * (m_CapsuleCollider.radius * 1.0f));
        //            m_TargetPosition.y = m_Controller.transform.position.y;
        //            return true;
        //        }
        //        if (drawAllLines) Debug.DrawRay(m_ObjectDetector.position, direction, hit ? Color.green : Color.magenta, 1);
        //    }
        //    return false;
        //}




    }

}

