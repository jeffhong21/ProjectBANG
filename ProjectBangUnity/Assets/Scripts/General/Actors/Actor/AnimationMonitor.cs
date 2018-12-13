namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;




    public class AnimationMonitor : MonoBehaviour
    {
        [SerializeField]
        protected bool m_DebugStateChanges;
        [SerializeField]
        protected AnimatorStateData m_BaseState;
        [SerializeField]
        protected AnimatorStateData m_UpperBodyState;


        protected Animator m_Animator;





        public void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_BaseState = new AnimatorStateData("Movement");
            m_UpperBodyState = new AnimatorStateData("Idle");

        }



        public void SetHorizontalInputValue(float value)
        {
            m_Animator.SetFloat(HashID.InputX, value);
        }

        public void SetForwardInputValue(float value)
        {
            m_Animator.SetFloat(HashID.InputY, value);
        }

        public void SetIntDataValue(int value)
        {
            m_Animator.SetInteger(HashID.AnimationIndex, value);
        }

        public void ExecuteEvent(string eventName)
        {
            m_Animator.SetTrigger(eventName);
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

