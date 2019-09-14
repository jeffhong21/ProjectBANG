using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using DebugUI;
namespace CharacterController
{
    public class EntityObjectManager : MonoBehaviour
    {

        private Dictionary<int, IEntityObject> m_allEntityObjects;

        [SerializeField]
        private float m_deltaTime;
        [SerializeField]
        private float m_previousTime;

        private bool m_frameUpdated;


        private void Start()
        {
            m_previousTime = 0f;
        }


        private void FixedUpdate()
        {
            Tick();

            m_frameUpdated = true;


            DebugUI.DebugUI.Log(this, Time.fixedDeltaTime, "fixedDeltaTime", DebugUI.RichTextColor.Cyan);
        }


        private void Update()
        {
            if (m_frameUpdated)
            {
                m_frameUpdated = false;
                return;
            }
            Tick();

            m_frameUpdated = false;

            DebugUI.DebugUI.Log(this, Time.deltaTime, "deltaTime", DebugUI.RichTextColor.Cyan);
        }



        private void Tick()
        {
            m_deltaTime = Time.time - m_previousTime;
            m_previousTime = Time.time;





            DebugUI.DebugUI.Log(this, m_deltaTime, "custom delta time", Time.inFixedTimeStep ? DebugUI.RichTextColor.Cyan : DebugUI.RichTextColor.LightBlue);
        }



        private float GetDeltaTime()
        {
            float deltaTime = Time.time - m_previousTime;
            m_previousTime = Time.time;
            return deltaTime;
        }


    }
}
