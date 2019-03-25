namespace CharacterController
{
    using UnityEngine;
    using System;


    public class Cover_Original : CharacterAction
    {
        public enum CoverIDs { None = -1, StandStill, StandPopLeft, StandPopRight, CrouchStill, CrouchPopLeft, CrouchPopRight }

        [Header("-- Cover Settings --")]
        [SerializeField]
        protected float m_TakeCoverDistance = 2f;
        [SerializeField]
        protected LayerMask m_CoverLayer;
        [SerializeField]
        protected float m_TakeCoverRotationSpeed = 4;



        [SerializeField]
        protected CoverIDs m_CurrentCoverID;
        protected Transform m_DetectorHolder;
        protected Transform m_ObjectDetector;
        protected Transform m_HighCoverDetector;
        protected Transform m_RightCoverPopup;
        protected Transform m_LeftCoverPopup;
        protected float m_ObjectDetectorHeight = 0.5f;
        protected float m_HighCoverHeight = 1.35f;
        protected float m_SideCoverDistance = 0.5f;

        protected RaycastHit m_RaycastHit;
        protected Quaternion m_TargetRotation;
        //[SerializeField]
        protected Vector3 m_TargetPosition;

       

        protected bool m_HighCover;
        protected float m_PopoutLength = 0.5f;
        [SerializeField]
        protected float m_HorizontalInput;
        [SerializeField]
        protected float m_ForwardInput;

        //[SerializeField]
        protected Vector3 m_StartPosition;
        //[SerializeField]
        protected Vector3 m_StopPosition;       //  Stop position when hitting side edge of cover
        //[SerializeField]
        protected bool m_CanPopLeft;
        //[SerializeField]
        protected bool m_CanPopRight;
        protected bool m_HasHighCoverLeft;
        protected bool m_HasHighCoverRight;


        [SerializeField]
        protected bool m_DrawCanEnterLines;



        public CoverIDs CurrentCoverID{
            get { return m_CurrentCoverID; }
            private set { m_CurrentCoverID = value; }
        }




		protected override void Awake()
        {
            base.Awake();

            m_DetectorHolder = new GameObject("Cover Detector Holder").transform;
            m_DetectorHolder.parent = m_Transform;
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
            RaycastHit hit;
            if (Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, out hit, m_TakeCoverDistance, m_CoverLayer)){
                m_TargetPosition = hit.point + (hit.normal * (m_CapsuleCollider.radius * 0.33f));
                m_TargetPosition.y = m_Controller.transform.position.y;
                return true;
            }
            return false;
            //return CanEnterCover(m_DrawCanEnterLines);
        }


        protected override void ActionStarted()
        {
            if (Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_TakeCoverDistance, m_CoverLayer)){
                m_HighCover = true;
            } else {
                m_HighCover = false;
            }
            m_CurrentCoverID = m_HighCover ? CoverIDs.StandStill : CoverIDs.CrouchStill;

            m_Animator.SetTrigger(HashID.ActionChange);


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
            m_AnimatorMonitor.SetActionID(0);
            m_AnimatorMonitor.SetIntDataValue((int)m_CurrentCoverID);
            m_AnimatorMonitor.SetHeightValue(1);

            m_Controller.LookRotation = m_Transform.rotation;
            m_CurrentCoverID = CoverIDs.None;

            m_Animator.ResetTrigger(HashID.ActionChange);
        }


        public override string GetDestinationState(int layer)
        {
            return "Cover";
        }



        public override bool UpdateRotation()
        {
            //Debug.DrawRay(m_ObjectDetector.position, m_ObjectDetector.forward, Color.magenta);
            if (Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, out m_RaycastHit, m_TakeCoverDistance, m_CoverLayer)){
                m_TargetRotation = Quaternion.FromToRotation(-m_Transform.forward, m_RaycastHit.normal) * m_Transform.rotation;
                m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, m_TargetRotation.normalized, m_TakeCoverRotationSpeed * Time.deltaTime));
            }
            return false;
        }

        float m_LerpPercentage;
        public override bool UpdateMovement()
        {
            m_HorizontalInput = m_Controller.InputVector.x;

            //  Check if character should be crouched or in high cover.
            if (Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_PopoutLength, m_CoverLayer)){
                m_HighCover = true;
            } else {
                m_HighCover = false;
            }

            //  Check if character has reached the edge of the cover.
            if(Mathf.Abs(m_HorizontalInput) > 0.2 )
            {
                //  If Left Cover Popup does not hit a cover object, than character cannot move left..  (MAGENTA)
                if (!Physics.Raycast(m_LeftCoverPopup.position, m_LeftCoverPopup.forward, m_PopoutLength, m_CoverLayer))
                {   //   - (0.25f * (m_RightCoverPopup.right))

                    if (!m_CanPopLeft)
                    {
                        m_StartPosition = m_Transform.position;
                        m_StopPosition = m_Transform.position - (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                    }
                    m_CanPopLeft = true;

                    m_HorizontalInput = Mathf.Clamp(m_HorizontalInput, 0, 1);
                    m_ForwardInput = m_Controller.InputVector.z;
                    m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, m_ForwardInput);

                    if (m_ForwardInput == 1)
                    {
                        m_CurrentCoverID = m_HighCover ? CoverIDs.StandPopRight : CoverIDs.CrouchPopRight;
                        m_LerpPercentage += m_ForwardInput * Time.deltaTime;
                        m_LerpPercentage = Mathf.Clamp01(m_LerpPercentage);
                        m_TargetPosition = Vector3.Lerp(m_StartPosition, m_StopPosition, m_LerpPercentage);
                        m_Transform.position = m_TargetPosition;

                        if (!Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, m_PopoutLength, m_CoverLayer))
                        {
                            m_CurrentCoverID = CoverIDs.None;
                        }
                    }
                    else
                    {
                        m_Controller.Velocity = m_Transform.right * m_HorizontalInput * Time.deltaTime;
                        m_Transform.position += m_Controller.Velocity;
                    }
                }
                //  If Right Cover Popup does not hit a cover object, than character cannot move right. (Blue)
                else if (!Physics.Raycast(m_RightCoverPopup.position, m_RightCoverPopup.forward, m_PopoutLength, m_CoverLayer))
                {  //  + (0.25f * (m_RightCoverPopup.right))

                    if (!m_CanPopRight)
                    {
                        m_StartPosition = m_Transform.position;
                        m_StopPosition = m_Transform.position + (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                    }
                    m_CanPopRight = true;

                    //m_HorizontalInput = Mathf.Clamp(m_HorizontalInput, -1, 0);
                    m_HorizontalInput = Mathf.Clamp(m_HorizontalInput, -1, 0);
                    m_ForwardInput = m_Controller.InputVector.z;
                    m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, m_ForwardInput);

                    if (m_ForwardInput == 1)
                    {
                        m_CurrentCoverID = m_HighCover ? CoverIDs.StandPopLeft : CoverIDs.CrouchPopLeft;
                        m_LerpPercentage += m_ForwardInput * Time.deltaTime;
                        m_LerpPercentage = Mathf.Clamp01(m_LerpPercentage);
                        m_TargetPosition = Vector3.Lerp(m_StartPosition, m_StopPosition, m_LerpPercentage);
                        m_Transform.position = m_TargetPosition;

                        if (!Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, m_PopoutLength, m_CoverLayer))
                        {
                            m_CurrentCoverID = CoverIDs.None;
                        }
                    }
                    else
                    {
                        m_Controller.Velocity = m_Transform.right * m_HorizontalInput * Time.deltaTime;
                        m_Transform.position += m_Controller.Velocity;
                    }
                }
                //  Character can move left or right
                else
                {
                    m_CurrentCoverID = m_HighCover ? CoverIDs.StandStill : CoverIDs.CrouchStill;
                    m_CanPopRight = m_CanPopLeft = false;

                    m_HorizontalInput = m_Controller.InputVector.x;
                    m_ForwardInput = 0;
                    m_LerpPercentage = 0;

                    m_Controller.InputVector.Set(m_HorizontalInput, m_Controller.InputVector.y, m_ForwardInput);
                    m_Controller.Velocity = m_Transform.right * m_HorizontalInput * Time.deltaTime;
                    m_Transform.position += m_Controller.Velocity;


                    //m_Rigidbody.AddForce(m_Transform.right * m_HorizontalInput * Time.deltaTime * 8, ForceMode.VelocityChange);
                    //if (m_ForwardInput == 1){
                    //    m_CurrentCoverID = CoverIDs.None;
                    //}
                }
            }




            Debug.DrawRay(m_ObjectDetector.position, m_ObjectDetector.forward, Color.yellow);
            Debug.DrawRay(m_LeftCoverPopup.position, m_LeftCoverPopup.forward, m_CanPopLeft ? Color.green : Color.magenta);
            Debug.DrawRay(m_RightCoverPopup.position, m_RightCoverPopup.forward, m_CanPopRight ? Color.green : Color.blue);
            Debug.DrawRay(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_HighCover ? Color.green : Color.yellow);


            return false;
        }


        public override bool UpdateAnimator()
        {
            //m_AnimatorMonitor.SetIntDataValue((int)m_CurrentCoverID);
            m_AnimatorMonitor.SetHeightValue(m_HighCover ? 1 : 0.5f);
            m_AnimatorMonitor.SetHorizontalInputValue(m_HorizontalInput);
            m_AnimatorMonitor.SetForwardInputValue(0);
            m_Animator.SetInteger(HashID.ActionID, m_ActionID);
            return false;
        }









        DebugSettings m_DebugSettings;
        [SerializeField]
        bool m_Debug;

        [Serializable]
        public class DebugSettings
        {
            [SerializeField]
            private float m_GizmoSize = 0.2f;
            [SerializeField]
            private Vector3 m_CubeSize;
            [SerializeField]
            private Color m_CenterColor;
            [SerializeField]
            private Color m_LeftColor;
            [SerializeField]
            private Color m_RightColor;
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


        private void InitializeDebugSettings()
        {
            m_DebugSettings = new DebugSettings();
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
            
            //if (m_Debug){
            //    if (m_LeftCoverPopup) DrawGizmoLine(m_LeftCoverPopup, _hitLeft);
            //    if (m_RightCoverPopup) DrawGizmoLine(m_RightCoverPopup, _hitRight);
            //}

            if(m_LeftCoverPopup && m_RightCoverPopup){
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(m_LeftCoverPopup.position, d_GizmoCubeSize);
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(m_RightCoverPopup.position, d_GizmoCubeSize);
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

