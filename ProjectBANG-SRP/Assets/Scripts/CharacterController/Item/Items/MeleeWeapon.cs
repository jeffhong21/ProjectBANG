namespace CharacterController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MeleeWeapon : Item, IUseableItem
    {
        protected int m_MaxCollisionCount = 30;


        protected Collider m_AttackHitbox;

        protected LayerMask m_ImpactLayers;
        protected float m_DamageAmount;
        protected float m_ImpactForce = 5;


        public bool InUse()
        {
            throw new System.NotImplementedException();
        }




        protected void OnCollisionEnter( Collision collision )
        {
            
        }




        protected void OnDrawGizmos()
        {
            
        }


    }

}
