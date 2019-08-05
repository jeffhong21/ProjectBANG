namespace CharacterController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    [RequireComponent(typeof(Item))]
    public class MeleeWeapon : MonoBehaviour, IUseableItem
    {
        [Tooltip(" ")]
        [SerializeField] protected int m_MaxCollisionCount = 30;
        [Tooltip(" ")]
        [SerializeField] protected Collider m_AttackHitbox;
        [Tooltip(" ")]
        [SerializeField] protected LayerMask m_ImpactLayers;
        [Tooltip(" ")]
        [SerializeField] protected float m_DamageAmount;
        [Tooltip(" ")]
        [SerializeField] protected float m_ImpactForce = 5;


        public bool InUse()
        { 
            throw new System.NotImplementedException();
        }

        public bool TryUse()
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
