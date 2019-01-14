namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;

    public static class HashID
    {
        public static readonly int HorizontalInput = Animator.StringToHash("HorizontalInput");
        public static readonly int ForwardInput = Animator.StringToHash("ForwardInput");
        public static readonly int Yaw = Animator.StringToHash("Yaw");

        public static readonly int Moving = Animator.StringToHash("Moving");
        public static readonly int Crouching = Animator.StringToHash("Crouching");

        public static readonly int AnimationIndex = Animator.StringToHash("AnimationIndex");
        public static readonly int FloatValue = Animator.StringToHash("FloatValue");
        public static readonly int Speed = Animator.StringToHash("Speed");


    }


    public class AnimationMonitor : MonoBehaviour
    {
        //
        // Fields
        //
        [SerializeField]
        protected bool m_DebugStateChanges;
        protected float m_HorizontalInputDampTime = 0.1f;
        protected float m_ForwardInputDampTime = 0.1f;
        [SerializeField]
        protected AnimatorStateData m_BaseState;

        protected Animator m_Animator;

        protected Rigidbody m_Rigidbody;

        //
        // Properties
        //
        public float HorizontalInputValue {
            get;
        }

        public float ForwardInputValue
        {
            get;
        }

        public int BaseLayerIndex
        {
            get;
        }



        //
        // Methods
        //
        public void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_BaseState = new AnimatorStateData("Movement");
        }

        public void PlayDefaultState()
        {
            
        }


        public void SetHorizontalInputValue(float value)
        {
            m_Animator.SetFloat(HashID.HorizontalInput, value, m_HorizontalInputDampTime, Time.deltaTime);
        }


        public void SetForwardInputValue(float value)
        {
            m_Animator.SetFloat(HashID.ForwardInput, value, m_ForwardInputDampTime, Time.deltaTime);
        }


        public void SetIntDataValue(int value)
        {
            m_Animator.SetInteger(HashID.AnimationIndex, value);
        }


        protected virtual void OnAnimatorMove()
        {
            m_Rigidbody.velocity = (m_Animator.deltaPosition * 1) / Time.deltaTime;
            //Debug.Log(m_Animator.deltaPosition);
        }










        [Serializable]
        public class AnimatorStateData
        {
            //
            // Fields
            //  
            string m_Name = "Movement";
            float m_TransitionDuration = 0.2f;
            float m_SpeedMultiplier = 1f;


            //
            // Properties
            //  
            public string Name
            {
                get { return m_Name; }
            }

            public float TransitionDuration
            {
                get { return m_TransitionDuration; }
            }

            public float SpeedMultiplier
            {
                get { return m_SpeedMultiplier; }
            }


            //
            // Constructor
            //  
            public AnimatorStateData(string name){
                m_Name = name;
            }

        }
    }



}

