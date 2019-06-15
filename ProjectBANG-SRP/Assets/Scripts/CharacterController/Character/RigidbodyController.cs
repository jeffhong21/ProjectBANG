namespace CharacterController
{
    using UnityEngine;
    using System;

    //[DisallowMultipleComponent]
    //[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(LayerManager))]
    public class RigidbodyController : MonoBehaviour
    {





        protected virtual void Awake()
        {

        }






        public void DetectCollisions()
        {

        }






        //protected void ScaleCapsule(float scaleFactor)
        //{
        //    if (m_CapsuleCollider.height != m_ColliderHeight * scaleFactor)
        //    {
        //        m_CapsuleCollider.height = Mathf.MoveTowards(m_CapsuleCollider.height, m_ColliderHeight * scaleFactor, Time.deltaTime * 4);
        //        m_CapsuleCollider.center = Vector3.MoveTowards(m_CapsuleCollider.center, m_ColliderCenter * scaleFactor, Time.deltaTime * 2);
        //    }
        //}





    }

}
