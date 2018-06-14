namespace Bang.AbilitySystem
{
    using UnityEngine;
    using System;

    public class DefaultDash : Ability
    {
        
        [SerializeField]
        protected float _dashSpeed = 10f;
        [SerializeField]
        protected float _dashDistance = 10f;

        protected bool isDashing;
        private Vector3 dashStartPosition, cursorPosition, playerVelocity;


        public float dashSpeed
        {
            get { return _dashSpeed; }
            set { _dashSpeed = value; }
        }

        public float dashDistance
        {
            get { return _dashDistance; }
            set { _dashDistance = value; }
        }


        public override void ExecuteAbility()
        {
            isDashing = true;
            animator.SetBool("Dash", isDashing);
            dashStartPosition = actor.transform.position;
        }

    }

}

