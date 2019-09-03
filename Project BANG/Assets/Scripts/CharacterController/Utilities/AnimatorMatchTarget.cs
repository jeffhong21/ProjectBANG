using UnityEngine;
using System;


namespace CharacterController
{
    [Serializable]
    public class AnimatorMatchTarget
    {
        private const float m_timeBuffer = 0.01f;


        [SerializeField]
        private CharacterAction m_action;
        [SerializeField]
        private bool m_matchTarget = true;
        [SerializeField]
        private Vector3 m_positionOffset = new Vector3(0, 0, 0);
        [SerializeField]
        private Vector3 m_rotationOffset = new Vector3(0, 0, 0);
        [SerializeField, Range(0f,0.99f)]
        private float m_startMatchTime = 0.1f;
        [SerializeField, Range(0.01f, 1f)]
        private float m_endMatchTime = 0.2f;
        //[MinMaxRange(0, 1)]
        //public Vector2 matchTargetRange = new Vector2(0.1f, 0.2f);
        [SerializeField]
        private AvatarTarget m_avatarTarget = AvatarTarget.Root;
        [SerializeField]
        private Vector3 m_positionXYZWeight = Vector3.one;
        [SerializeField, Range(0f, 1f)]
        private float m_rotationWeight = 1;


        private Vector2 m_matchTargetRange = new Vector2(0.1f, 0.2f);
        private MatchTargetWeightMask m_weightMask = new MatchTargetWeightMask(Vector3.one, 1);


        private Vector3 m_matchPosition;
        private Quaternion m_matchRotation;
        private Animator m_animator;


        //
        //  Properties
        //

        //public Vector3 matchPosition


        public bool matchTarget {
            get { return m_matchTarget; } set { m_matchTarget = value; }
        }

        public Vector3 matchPosition {
            get {
                if (m_positionOffset != Vector3.zero)
                    return Vector3Util.Add(m_matchPosition, m_positionOffset);
                return m_matchPosition;
            }
            set { m_matchPosition = value; }
        }

        public Quaternion matchRotation {
            get {
                if (m_rotationOffset != Vector3.zero)
                    return m_matchRotation * Quaternion.Euler(m_rotationOffset);
                return m_matchRotation;
            }
            set { m_matchRotation = value; }
        }

        public Vector2 matchTargetRange {
            get {
                m_matchTargetRange.x = Mathf.Clamp(m_startMatchTime, 0, m_endMatchTime - m_timeBuffer);
                m_matchTargetRange.y = Mathf.Clamp(m_endMatchTime, m_startMatchTime + m_timeBuffer, 1);
                return m_matchTargetRange;
            }

        }

        public MatchTargetWeightMask weightMask {
            get {
                m_weightMask.positionXYZWeight = m_positionXYZWeight;
                m_weightMask.rotationWeight = m_rotationWeight;
                return m_weightMask;
            }
        }

        public HumanBodyBones humanBodyBoneTarget
        {
            get
            {
                var bone = HumanBodyBones.Hips;
                switch (m_avatarTarget)
                {
                    case AvatarTarget.LeftHand:
                        bone = HumanBodyBones.LeftHand;
                        break;
                    case AvatarTarget.RightHand:
                        bone = HumanBodyBones.RightHand;
                        break;
                    case AvatarTarget.LeftFoot:
                        bone = HumanBodyBones.LeftFoot;
                        break;
                    case AvatarTarget.RightFoot:
                        bone = HumanBodyBones.RightFoot;
                        break;
                    case AvatarTarget.Body:
                        bone = HumanBodyBones.Hips;
                        break;
                }
                return bone;
            }
        }




        //
        //  Constructor
        //
        public AnimatorMatchTarget(Animator animator)
        {
            m_animator = animator;
            m_matchTargetRange = new Vector2(m_startMatchTime, m_endMatchTime);
            m_weightMask = new MatchTargetWeightMask(m_positionXYZWeight, m_rotationWeight);
        }




        public bool MatchTarget( Vector3 targetPosition, Quaternion targetRotation )
        {
            if (m_animator.isMatchingTarget || !m_matchTarget) return false;

            m_animator.MatchTarget(targetPosition + m_positionOffset, targetRotation, m_avatarTarget, m_weightMask, matchTargetRange.x, matchTargetRange.y);

            return true;
        }


        public void Reset(bool stopMatch = true, bool completeMatch = false)
        {
            if(stopMatch) m_animator.InterruptMatchTarget(completeMatch);

            m_matchPosition = default;
            m_matchRotation = default;
        }
    }

}
