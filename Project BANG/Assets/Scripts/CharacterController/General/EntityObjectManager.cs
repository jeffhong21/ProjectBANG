using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    public class EntityObjectManager : MonoBehaviour
    {
        [SerializeField]
        private float m_deltaTime;
        [SerializeField]
        private float m_time;
        private float m_previousTime;

        private bool m_frameUpdated;


        private void Start()
        {
            m_time = Time.time;
            m_previousTime = 0f;
        }


        private void FixedUpdate()
        {
            m_deltaTime = GetDeltaTime(Time.time);

            m_frameUpdated = true;
        }


        private void Update()
        {
            if (m_frameUpdated) return;

            m_deltaTime = GetDeltaTime(Time.time);


            m_frameUpdated = true;
        }



        private float GetDeltaTime(float time)
        {
            return time - m_previousTime;
        }


    }
}
