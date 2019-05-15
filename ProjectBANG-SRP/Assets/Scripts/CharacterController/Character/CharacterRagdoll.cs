namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [DisallowMultipleComponent]
    public class CharacterRagdoll : MonoBehaviour
    {
        private const string m_RagdollLayerName = "CharacterCollider";

        private Dictionary<HumanBodyBones, JointSettings> m_JointSettings = new Dictionary<HumanBodyBones, JointSettings>()
        {
            { HumanBodyBones.LeftUpperLeg, new JointSettings(new Vector3(0, 1, 0), new Vector3(0, 0, -1), -20, 90, 30, 30) },
            { HumanBodyBones.LeftLowerLeg, new JointSettings(new Vector3(0, 0, -1), new Vector3(0, 1, 0),-140, 0, 10, 30) },

            { HumanBodyBones.RightUpperLeg, new JointSettings(new Vector3(0, 1, 0), new Vector3(0, 0, 1),-20, 90, 30, 30) },
            { HumanBodyBones.RightLowerLeg, new JointSettings(new Vector3(0, 0, -1), new Vector3(0, -1, 0),-140, 0, 10, 30) },

            { HumanBodyBones.Spine, new JointSettings(new Vector3(0, 0, 1), new Vector3(0, -1, 0),-30, 20, 20, 30) },
            { HumanBodyBones.Head, new JointSettings(new Vector3(1, 0, 0),  new Vector3(0, 0, 1),-40, 50, 30, 30) },

            { HumanBodyBones.LeftUpperArm, new JointSettings(new Vector3(0, 1, 0), new Vector3(0, 0, 1), -100, 60, 60, 50) },
            { HumanBodyBones.LeftLowerArm, new JointSettings(new Vector3(0, 1, 0), new Vector3(0, 0, 1), -90, 20, 20, 30) },
           
            { HumanBodyBones.RightUpperArm, new JointSettings(new Vector3(0, -1, 0), new Vector3(0, 0, -1), -60, 100, 60, 50) },
            { HumanBodyBones.RightLowerArm, new JointSettings(new Vector3(0, -1, 0), new Vector3(0, 0, -1), -20, 90, 20, 30) },
        };

        [SerializeField]
        private List<Collider> m_RagdollColliders = new List<Collider>();
        [SerializeField]
        private List<Rigidbody> m_RagdollRigidbody = new List<Rigidbody>();




        private CharacterLocomotion m_Controller;
        private Rigidbody m_Rigidbody;
        private Collider m_Collider;
        private Animator m_Animator;
        private GameObject m_GameObject;
        private Transform m_Transform;



        private void Awake()
        {
            m_Controller = GetComponent<CharacterLocomotion>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
            m_Animator = GetComponent<Animator>();
            m_GameObject = gameObject;
            m_Transform = transform;

            SetupRagdoll();


        }


		private void OnEnable()
		{
            EventHandler.RegisterEvent<Vector3, Vector3>(m_GameObject, EventIDs.OnRagdoll, EnableRagdoll);
		}


		private void OnDisable()
		{
            EventHandler.UnregisterEvent<Vector3, Vector3>(m_GameObject, EventIDs.OnRagdoll, EnableRagdoll);
		}




		private void SetupRagdoll()
        {
            Rigidbody[] rb = m_GameObject.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rb.Length; i++)
            {
                if (rb[i] == m_Rigidbody) continue;

                rb[i].isKinematic = true;
                rb[i].useGravity = false;
                //rb[i].mass *= 5;
                rb[i].gameObject.layer = LayerMask.NameToLayer(m_RagdollLayerName);
                m_RagdollRigidbody.Add(rb[i]);
            }

            Collider[] col = m_GameObject.GetComponentsInChildren<Collider>();
            for (int i = 0; i < col.Length; i++)
            {
                if (col[i] == m_Collider) continue;

                col[i].isTrigger = true;
                m_RagdollColliders.Add(col[i]);
            }

        }


        public void EnableRagdoll(Vector3 position, Vector3 direction)
        {
            m_Animator.enabled = false; //  this will stop ragdolls from exploding.
            m_Collider.enabled = false;
            m_Rigidbody.isKinematic = true;


            for (int i = 0; i < m_RagdollColliders.Count; i++)
            {
                m_RagdollColliders[i].enabled = true;
                m_RagdollColliders[i].isTrigger = false;
            }
            for (int i = 0; i < m_RagdollRigidbody.Count; i++)
            {
                m_RagdollRigidbody[i].isKinematic = false;
                m_RagdollRigidbody[i].useGravity = true;
            }
        }


        public void DisableRagdoll()
        {
            if(m_Animator) m_Animator.enabled = true; 
            if (m_Animator) m_Collider.enabled = true;
            if (m_Animator) m_Rigidbody.isKinematic = true;


            for (int i = 0; i < m_RagdollColliders.Count; i++)
            {
                //m_RagdollColliders[i].enabled = true;
                m_RagdollColliders[i].isTrigger = true;
            }
            for (int i = 0; i < m_RagdollRigidbody.Count; i++)
            {
                m_RagdollRigidbody[i].isKinematic = true;
                m_RagdollRigidbody[i].useGravity = false;
            }
        }






        public void UpdateCharacterJoints(){
            //  InitializeJointSettings
            foreach (var item in m_JointSettings)
            {
                m_JointSettings[item.Key].SetupCharacterJoint(GetBoneTransform(item.Key));
            }
        }


        public Transform GetBoneTransform(HumanBodyBones humanBodyBone){
            var animator = GetComponent<Animator>();
            if (animator != null)
                return animator.GetBoneTransform(humanBodyBone).transform;
            return null;
        }



        [Serializable]
        public class JointSettings
        {
            public Vector3 axis;
            public Vector3 swingAxis;
            public SoftJointLimit lowTwistLimit;
            public SoftJointLimit highTwistLimit;
            public SoftJointLimit swing1Limit;
            public SoftJointLimit swing2Limit;

            private CharacterJoint charJoint;


            public JointSettings(float lowTwistLimit, float highTwistLimit, float swing1Limit, float swing2Limit)
            {
                this.lowTwistLimit = new SoftJointLimit() { limit = lowTwistLimit };
                this.highTwistLimit = new SoftJointLimit() { limit = highTwistLimit };
                this.swing1Limit = new SoftJointLimit() { limit = swing1Limit };
                this.swing2Limit = new SoftJointLimit() { limit = swing2Limit };
            }

            public JointSettings(Vector3 axis, Vector3 swingAxis, float lowTwistLimit, float highTwistLimit, float swing1Limit, float swing2Limit)
            {
                this.axis = axis;
                this.swingAxis = swingAxis;
                this.lowTwistLimit = new SoftJointLimit() { limit = lowTwistLimit };
                this.highTwistLimit = new SoftJointLimit() { limit = highTwistLimit };
                this.swing1Limit = new SoftJointLimit() { limit = swing1Limit };
                this.swing2Limit = new SoftJointLimit() { limit = swing2Limit };
            }

            public void SetupCharacterJoint(Transform transform)
            {
                charJoint = transform.GetComponent<CharacterJoint>();
                if(charJoint != null){
                    charJoint.axis = axis;
                    charJoint.swingAxis = swingAxis;
                    charJoint.lowTwistLimit = lowTwistLimit;
                    charJoint.highTwistLimit = highTwistLimit;
                    charJoint.swing1Limit = swing1Limit;
                    charJoint.swing2Limit = swing2Limit;
                } 
                else {
                    Debug.LogFormat("{0} does not have a character joint.", transform.gameObject);
                }

            }
        }





    }
}