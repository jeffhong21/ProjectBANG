namespace CharacterController
{
    using UnityEngine;


    public class Projectile : MonoBehaviour
    {
        //
        // Fields
        //
        protected float m_DamageAmount = 1f;
        protected float m_ImpactForce = 5f;
        protected string m_DamageEvent;
        protected GameObject m_Explosion;
        protected GameObject m_DefaultDecal;
        protected GameObject m_DefaultDust;
        protected Transform m_Transform;
        protected Collider m_Collider;
        protected float m_InitialSpeed;
        protected float m_Speed = 5;
        protected float m_Lifespace = 10;
        protected bool m_DestroyOnCollision = true;




        //
        // Methods
        //
        protected virtual void Awake()
        {

        }


        public void Initialize(Vector3 direction, Vector3 torque, GameObject originator)
        {
            
        }

        public void WaitForInitialization()
        {
            
        }


        protected virtual void FixedUpdate()
        {
            
        }


        protected virtual void Collide(GameObject originator, Transform collisionTransform, Vector3 collisionPoint, Vector3 collisionPointNormal, bool destroy)
        {

        }
    }

}