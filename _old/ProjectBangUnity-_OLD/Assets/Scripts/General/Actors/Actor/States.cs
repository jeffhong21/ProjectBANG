namespace Bang
{
    using UnityEngine;
    using System;
    using System.Collections;


    public class States
    {
        public bool m_Condition;
        public float m_Cooldown;


        public virtual void OnUpdate(float time)
        {
            if (m_Cooldown > 0){
                m_Cooldown -= time;
                if (m_Cooldown <= 0){
                    OnStateChange();
                }
            }
        }

        protected virtual void OnStateChange(){
            
        }
    }



    public class IsDamageState
    {
        public bool m_Condition;
        public float m_Cooldown;
        public float m_LastDamageTaken;


        public virtual void OnUpdate(float time)
        {
            if (m_Cooldown > 0)
            {
                m_Cooldown -= time;
                if (m_Cooldown <= 0)
                {
                    //OnStateChange();
                }
            }
        }
    }


}