namespace Bang
{
    using UnityEngine;

    public interface IAgentCtrl
    {
        
        void MoveTo(Vector3 destination);


        void StopMoving();


        /// <summary>
        /// Called when the attack target changes.
        /// </summary>
        /// <param name="newAttackTarget">The new attack target.</param>
        void OnAttackTargetChanged(IHasHealth newAttackTarget);


        /// <summary>
        /// Called when the attack target dies.
        /// </summary>
        void OnAttackTargetDead();
    }

}