using UnityEngine;
using RootMotion.FinalIK;
using System.Collections.Generic;

namespace CharacterController
{
    public class FinalIKController : CharacterIKBase
    {
        [Header("Full Body IK")]
        public FullBodyBipedIK m_fullbodyIK;
        [SerializeField]
        protected Transform rightHandTarget, leftHandTarget, bodyEffectorTarget, m_lookTarget;
        [Header("GrounderIK")]
        public GrounderFBBIK grounderIK;

        [Header("LookAtIK")]
        public LookAtIK m_lookAtIK;

        [Header("AimIK")]
        public AimIK aimIK;

        [Header("Debug")]
        [SerializeField] private bool hideInInspector;


        private bool updateFrame;





        public Transform LookTarget {
            get { return m_lookTarget;
        }
            set {
                m_lookTarget = value;
                //if(m_lookAtIK != null) m_lookAtIK.solver.target = m_lookTarget;
            }
        }






        float lookWeight = 1;
        float lookWeightVelocity;
        float weightSmoothTime = 0.3f;





        protected override void Initialize()
        {
            if (m_fullbodyIK == null) m_fullbodyIK = GetComponent<FullBodyBipedIK>();
            if (m_fullbodyIK != null) {
                if (rightHandTarget == null) {
                    rightHandTarget = CreateIKEffectors("RightHand Effector Target", m_animator.GetBoneTransform(HumanBodyBones.RightHand).position, m_animator.GetBoneTransform(HumanBodyBones.RightHand).rotation);
                    m_fullbodyIK.solver.rightHandEffector.target = rightHandTarget;
                }
                if (leftHandTarget == null) {
                    leftHandTarget = CreateIKEffectors("LeftHand Effector Target", m_animator.GetBoneTransform(HumanBodyBones.LeftHand).position, m_animator.GetBoneTransform(HumanBodyBones.LeftHand).rotation);
                    m_fullbodyIK.solver.leftHandEffector.target = leftHandTarget;

                }
                if (bodyEffectorTarget == null) {
                    bodyEffectorTarget = CreateIKEffectors("Body Effector Target", m_animator.GetBoneTransform(HumanBodyBones.Hips).position, m_animator.GetBoneTransform(HumanBodyBones.Hips).rotation);
                    m_fullbodyIK.solver.bodyEffector.target = bodyEffectorTarget;

                }
            }


            if (m_lookAtIK == null) m_lookAtIK = GetComponent<LookAtIK>();
            if (m_lookAtIK != null) {
                if (m_lookTarget == null) {
                    m_lookTarget = CreateIKEffectors("LookAt Target", m_animator.GetBoneTransform(HumanBodyBones.Head).position + transform.forward, Quaternion.identity);
                    m_lookTarget.position = m_animator.GetBoneTransform(HumanBodyBones.Neck).position + transform.forward * 10;
                    m_lookAtIK.solver.target = m_lookTarget;
                }
                m_lookAtIK.solver.head = new IKSolverLookAt.LookAtBone(m_animator.GetBoneTransform(HumanBodyBones.Head));

                IKSolverLookAt.LookAtBone[] spineBones =
    {
                new IKSolverLookAt.LookAtBone(m_animator.GetBoneTransform(HumanBodyBones.Spine)),
                new IKSolverLookAt.LookAtBone(m_animator.GetBoneTransform(HumanBodyBones.Chest)),
                new IKSolverLookAt.LookAtBone(m_animator.GetBoneTransform(HumanBodyBones.UpperChest)),
                new IKSolverLookAt.LookAtBone(m_animator.GetBoneTransform(HumanBodyBones.Neck))
            };
                m_lookAtIK.solver.spine = spineBones;
            }









            //  ---------------
            //  GrounderIK
            //  ---------------
            //GrounderFBBIK.SpineEffector[] spineEffectors = {
            //    new GrounderFBBIK.SpineEffector(FullBodyBipedEffector.LeftShoulder, 1, 0.3f)
            //};

            //grounderIK.spine = spineEffectors;
            grounderIK.solver.maxStep = m_controller.Collider.radius;
            grounderIK.solver.quality = Grounding.Quality.Simple;

        }





        private void OnValidate()
        {
            var ikComponents = new List<IK>();
            if (m_fullbodyIK != null) ikComponents.Add(m_fullbodyIK);
            //if (grounderIK != null) ikComponents.Add(grounderIK);
            if (m_lookAtIK != null) ikComponents.Add(m_lookAtIK);
            if (aimIK != null) ikComponents.Add(aimIK);


            for (int i = 0; i < ikComponents.Count; i++)
            {
                ikComponents[i].hideFlags = hideInInspector ? HideFlags.HideInInspector : HideFlags.None;
            }
            if (grounderIK != null) grounderIK.hideFlags = hideInInspector ? HideFlags.HideInInspector : HideFlags.None;


            //iks = GetComponent<IK>();
        }



        //private void FixedUpdate()
        //{
        //    updateFrame = true;
        //}


        private void LateUpdate()
        {
            //if (!updateFrame) return;
            //updateFrame = false;

            //LookAtIKSolver();

            //CheckFeetGrounded();
        }








        private void EquipUnequip(Item item, int slotID)
        {


        }


        bool planted;
        bool initalize;
        [SerializeField ]LegInfo[] legInfo;
        private void CheckFeetGrounded()
        {
           
            if(initalize == false) {
                if (legInfo.Length == 0 || legInfo == null) {
                    legInfo = new LegInfo[grounderIK.solver.legs.Length];
                    for (int i = 0; i < grounderIK.solver.legs.Length; i++) {
                        legInfo[i] = new LegInfo(grounderIK.solver.legs[i].transform);
                    }
                }
                initalize = true;
            }

            if (grounderIK == null || m_controller == null || legInfo == null) return;


            if (m_controller.Moving)
            {
                for (int i = 0; i < grounderIK.solver.legs.Length; i++)
                {
                    Grounding.Leg leg = grounderIK.solver.legs[i];
                    LegInfo info = legInfo[i];
                    if (leg.initiated)
                    {
                        info.isGrounded = leg.isGrounded;
                        info.velocity = leg.velocity;
                        info.IKPosition = leg.IKPosition;
                        info.heightFromGround = leg.heightFromGround;
                        info.hit = leg.GetHitPoint;
                        //if(leg.IKPosition.y < 1 && leg.velocity.x < 1 && m_animator.pivotWeight <= 0.25f)

                        //if(i == 0 && info.isGrounded && m_animator.velocity.sqrMagnitude > 4) {
                        //    Debug.LogFormat("<b><color=yellow>[FOOT PLANTED]</color></b> {0} is grounded <color=blue>{3}</color>.  IK = <color=blue>{1}</color>, V = <color=blue>{2}</color>",
                        //        info.transform.name, info.IKPosition, info.velocity, m_animator.pivotWeight);
                        //    Debug.Break();
                        //}
                    }

                }
            }


        }








        private void LookAtIKSolver()
        {
            //// Do nothing if FixedUpdate has not been called since the last LateUpdate
            //if (!updateFrame) return;
            //updateFrame = false;

            //// Updating the IK solvers in a specific order. 
            //foreach (IK component in components) component.GetIKSolver().Update();

            var lookTargetDir = m_lookTarget.position - transform.position;
            var angleDif = Vector3.Angle(transform.forward, lookTargetDir);


            m_lookAtIK.solver.IKPositionWeight = Mathf.SmoothDamp(m_lookAtIK.solver.IKPositionWeight, (angleDif > 75) ? 0 : lookWeight, ref lookWeightVelocity, weightSmoothTime);
            if (m_lookAtIK.solver.IKPositionWeight >= 0.999f) m_lookAtIK.solver.IKPositionWeight = 1f;
            if (m_lookAtIK.solver.IKPositionWeight <= 0.001f) m_lookAtIK.solver.IKPositionWeight = 0f;

            //CharacterDebug.Log("Look Target Angle difference", angleDif);
        }






        [System.Serializable]
        private struct LegInfo
        {
            public Transform transform;
            public bool isGrounded;
            public Vector3 velocity;
            public Vector3 IKPosition;
            public float heightFromGround;
            public RaycastHit hit;

            public LegInfo(Transform t)
            {
                transform = t;
                isGrounded = false;
                velocity = default;
                IKPosition = default;
                heightFromGround = 0;
                hit = new RaycastHit();
            }

            public void OnPreGround()
            {

            }

            public void OnPostGround()
            {

            }

        }

    }
}














