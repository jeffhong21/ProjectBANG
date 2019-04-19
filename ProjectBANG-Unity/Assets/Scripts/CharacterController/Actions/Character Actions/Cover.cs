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
        protected float m_CoverMovementSpeed = 3;
        [SerializeField]
        protected float m_TakeCoverRotationSpeed = 4;

        [SerializeField]
        protected float m_ObjectDetectorHeight = 0.5f;
        [SerializeField]
        protected float m_HighCoverHeight = 1.35f;
        [SerializeField]
        protected float m_SideCoverDistance = 0.5f;

        [SerializeField, DisplayOnly]
        protected float m_Spacing;

        protected Transform m_DetectorHolder;
        protected Transform m_ObjectDetector;
        protected Transform m_HighCoverDetector;
        protected Transform m_RightCoverPopup;
        protected Transform m_LeftCoverPopup;


        protected RaycastHit m_ObjectDetectorHit;
        protected RaycastHit m_RotationDetectorHit;

        protected Quaternion m_TargetRotation;
        protected Vector3 m_TargetPosition;

        [SerializeField, DisplayOnly]
        protected Vector3 m_Velocity;
        [SerializeField, DisplayOnly]
        protected float m_Direction;
        [SerializeField, DisplayOnly]
        protected bool m_HighCover;
        protected float m_PopoutLength = 0.65f;
        [SerializeField, DisplayOnly]
        protected float m_HorizontalInput, m_ForwardInput;
        protected Vector3 m_StartPosition, m_StopPosition;       //  Stop position when hitting side edge of cover
        [SerializeField, DisplayOnly]
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
            m_HighCover = Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_PopoutLength, m_CoverLayer) ? true : false;


            //m_Controller.SetPosition(m_TargetPosition);
            var action = m_Controller.GetAction<MoveTowards>();
            if(action != null){
                if (!action.IsActive){
                    action.ActionStartLocation = m_TargetPosition;
                    m_Controller.TryStartAction(action);
                }
            }

            //Debug.LogFormat("Dot product.  {0}", Vector3.Dot(m_Transform.forward, -m_ObjectDetectorHit.normal));
            //var directionVector = Vector3.Reflect(m_Transform.forward * 2, m_ObjectDetectorHit.normal);
        }





		protected override void ActionStopped()
        {
            m_HighCover = Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_PopoutLength, m_CoverLayer);
            m_Animator.SetFloat(HashID.Height, m_HighCover ? 1 : 0.5f);

            m_Animator.SetInteger(HashID.ActionID, 0);
            //m_Animator.SetInteger(HashID.ActionIntData, emergeIndex);
            //m_Animator.SetFloat(HashID.Height, 1);

            m_Controller.LookRotation = m_Transform.rotation;

            //if (m_CanPopLeft || m_CanPopRight){
            //    var action = m_Controller.GetAction<MoveTowards>();
            //    if (action != null){
            //        if (!action.IsActive){
            //            action.ActionStartLocation = m_StopPosition;
            //            m_Controller.TryStartAction(action);
            //        }
            //    }
            //}

            //m_Animator.SetFloat(HashID.Height, 1f);
        }




		public override bool CheckMovement()
		{
            m_HorizontalInput = m_Controller.InputVector.x;
            m_Direction = Mathf.Clamp(m_HorizontalInput, -1, 1);;
            //  Execute only when the character is moving.
            if (Mathf.Abs(m_HorizontalInput) > 0.1f)
            {
                ////  -- Check if character has reached the edge of the cover. --
                //// Moving to the Left.
                //if(m_HorizontalInput < 0)
                //{
                //    if (Physics.Raycast(m_LeftCoverPopup.position, m_LeftCoverPopup.forward, out m_RotationDetectorHit, m_PopoutLength, m_CoverLayer))
                //    {
                //        m_TargetRotation = Quaternion.FromToRotation(-m_Transform.forward, m_RotationDetectorHit.normal);
                //        //m_TargetRotation = Quaternion.LookRotation(m_ObjectDetectorHit.normal, m_Transform.up);
                //    }
                //    //  If Left Cover Popup does not hit a cover object, than character cannot move left..  
                //    else{
                //        m_CanPopLeft = true;
                //        m_StartPosition = m_Transform.position;
                //        m_StopPosition = m_Transform.position - (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                //        m_StopPosition = m_StopPosition + m_Transform.forward * 0.892f;
                //    }
                //}
                //else if (m_HorizontalInput > 0)
                //{
                //    if (Physics.Raycast(m_RightCoverPopup.position, m_RightCoverPopup.forward, out m_RotationDetectorHit, m_PopoutLength, m_CoverLayer))
                //    {
                //        m_TargetRotation = Quaternion.FromToRotation(-m_Transform.forward, m_RotationDetectorHit.normal);
                //        //m_TargetRotation = Quaternion.LookRotation(m_ObjectDetectorHit.normal, m_Transform.up);
                //    }
                //    else{
                //        m_CanPopRight = true;
                //        m_StartPosition = m_Transform.position;
                //        m_StopPosition = m_Transform.position + (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                //        m_StopPosition = m_StopPosition + m_Transform.forward * 0.892f;
                //    }
                //}
                //else
                //{
                //    m_TargetRotation = m_Transform.rotation;
                //    m_CanPopRight = m_CanPopLeft = false;
                //    m_StartPosition = m_StopPosition = Vector3.zero;
                //}


                if (Physics.Raycast(m_LeftCoverPopup.position, m_LeftCoverPopup.forward, out m_RotationDetectorHit, m_PopoutLength, m_CoverLayer) == false)
                {
                    m_CanPopLeft = true;
                    m_StartPosition = m_Transform.position;
                    m_StopPosition = m_Transform.position - (m_Transform.right * 1.1f); //m_RightCoverPopup.position + (0.25f * (m_RightCoverPopup.right));
                    m_StopPosition = m_StopPosition + m_Transform.forward * 0.892f;
                }
                //  If Left Cover Popup does not hit a cover object, than character cannot move left..  
                else if (Physics.Raycast(m_RightCoverPopup.position, m_RightCoverPopup.forward, out m_RotationDetectorHit, m_PopoutLength, m_CoverLayer) == false)
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
		public override bool UpdateRotation()
        {
            if (Mathf.Abs(m_HorizontalInput) > 0.1)
            {
                //m_Transform.rotation = m_TargetRotation * m_Transform.rotation;
                alignmentAngle = Mathf.Abs(Quaternion.Angle(m_Transform.rotation, m_TargetRotation));
            } else {
                alignmentAngle = 0;
            }


            //if(alignmentAngle > 0){
            //    m_Transform.rotation = m_TargetRotation * m_Transform.rotation;
            //}
            //m_Transform.rotation = m_TargetRotation * m_Transform.rotation;
            //m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_TargetRotation * m_Transform.rotation, m_TakeCoverRotationSpeed * m_DeltaTime);
            ////m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, m_TargetRotation * m_Transform.rotation, m_TakeCoverRotationSpeed * m_DeltaTime);

            if (Mathf.Abs(m_HorizontalInput) > 0.1)
            {

                //Debug.DrawRay(m_ObjectDetector.position, m_ObjectDetector.forward, Color.magenta);
                if (Physics.Raycast(m_ObjectDetector.position, m_ObjectDetector.forward, out m_ObjectDetectorHit, m_TakeCoverDistance, m_CoverLayer)){
                    m_TargetRotation = Quaternion.FromToRotation(-m_Transform.forward, m_ObjectDetectorHit.normal) * m_Transform.rotation;
                    m_Transform.rotation = m_TargetRotation;
                    //m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Transform.rotation, m_TargetRotation.normalized, m_TakeCoverRotationSpeed * m_DeltaTime));
                }
            }
            return false;
        }



        //  Only allow movement on the relative x axis to prevent the character from moving away from the cover point.
        public override bool UpdateMovement()
        {
            //m_Spacing = m_ObjectDetectorHit.distance;

            //  Execute only when the character is moving.
            if (Mathf.Abs(m_HorizontalInput) > 0.1f)
            {
                m_HighCover = Physics.Raycast(m_HighCoverDetector.position, m_HighCoverDetector.forward, m_PopoutLength, m_CoverLayer) ? true : false;
                //  Character is switching directions.
                if ((m_Direction >= 0 && m_HorizontalInput < 0))
                {
                    m_Direction = Mathf.Lerp(1, -1, m_DeltaTime);
                }
                else if (m_Direction < 0 && m_HorizontalInput >= 0)
                {
                    m_Direction = Mathf.Lerp(-1, 1, m_DeltaTime);
                }
            }

            var hitLocation = m_ObjectDetectorHit.point;
            hitLocation.y = m_Transform.position.y;
            m_Spacing = (hitLocation - m_Transform.position).magnitude;
            if (m_Spacing > m_CapsuleCollider.radius + 0.02f){
                m_Transform.position = Vector3.Lerp(m_Transform.position, m_Transform.position + m_Transform.forward * (m_Spacing - m_CapsuleCollider.radius + 0.02f), m_DeltaTime);
                //m_Transform.position = Vector3.Lerp(m_Transform.position, m_Transform.position + m_Transform.forward * (m_Spacing - m_CapsuleCollider.radius + 0.02f), m_DeltaTime);
                //m_Transform.position = m_Transform.position + (Vector3.forward * (m_Spacing - m_CapsuleCollider.radius + 0.02f) * m_DeltaTime);
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

            if(Mathf.Abs(m_HorizontalInput) >= 1)
                m_Velocity = m_Transform.right * m_Controller.InputVector.x * m_CoverMovementSpeed;
            else
                m_Velocity = m_Transform.right * m_HorizontalInput;




            if (m_Debug) Debug.DrawRay(m_ObjectDetector.position, m_ObjectDetector.forward * m_PopoutLength, m_DebugSettings.CenterColor);
            if (m_Debug) Debug.DrawRay(m_LeftCoverPopup.position, m_LeftCoverPopup.forward * m_PopoutLength, m_CanPopLeft ? m_DebugSettings.ChangeStateColor : m_DebugSettings.LeftColor);
            if (m_Debug) Debug.DrawRay(m_RightCoverPopup.position, m_RightCoverPopup.forward * m_PopoutLength, m_CanPopRight ? m_DebugSettings.ChangeStateColor : m_DebugSettings.RightColor);
            if (m_Debug) Debug.DrawRay(m_HighCoverDetector.position, m_HighCoverDetector.forward * m_PopoutLength, m_HighCover ? m_DebugSettings.ChangeStateColor : m_DebugSettings.CenterColor);

            return false;
        }


        public override bool Move()
        {
            m_Controller.Velocity = m_Velocity;
            m_Rigidbody.velocity = Vector3.Slerp(m_Rigidbody.velocity, m_Controller.Velocity, 20 * m_DeltaTime);

            return false;
        }



        public override bool UpdateAnimator()
        {
            //  Is Moving.
            m_Animator.SetBool(HashID.Moving, Mathf.Abs(m_Controller.InputVector.x) > 0.1f);
            //m_AnimatorMonitor.SetIntDataValue((int)m_CurrentCoverID);

            m_AnimatorMonitor.SetHorizontalInputValue(m_Controller.InputVector.x);
            m_ForwardInput = 0;
            m_AnimatorMonitor.SetForwardInputValue(m_ForwardInput);

            m_Animator.SetInteger(HashID.ActionID, m_ActionID);
            //m_Animator.SetBool(HashID.Crouching, !m_HighCover);
            m_Animator.SetFloat(HashID.Height, m_HighCover ? 1 : 0.5f, 0.1f, m_DeltaTime);

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
        //        if (Physics.Raycast(m_ObjectDetector.position, direction, out m_ObjectDetectorHit, m_TakeCoverDistance, m_CoverLayer))
        //        {
        //            if (!drawAllLines) Debug.DrawRay(m_ObjectDetector.position, direction, hit ? Color.green : Color.magenta, 1);
        //            hit = true;
        //            m_TargetPosition = m_ObjectDetectorHit.point + (m_ObjectDetectorHit.normal * (m_CapsuleCollider.radius * 1.0f));
        //            m_TargetPosition.y = m_Controller.transform.position.y;
        //            return true;
        //        }
        //        if (drawAllLines) Debug.DrawRay(m_ObjectDetector.position, direction, hit ? Color.green : Color.magenta, 1);
        //    }
        //    return false;
        //}




    }

}

