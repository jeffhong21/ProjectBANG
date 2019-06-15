namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    public static class EventIDs
    {
        //  Can be used to disable input.
        //EventHandler.ExecuteEvent(m_Character, "OnEnableGameplayInput", false);
        public static readonly string OnEnableGameplayInput = "OnEnableGameplayInput";


        public static readonly string OnCharacterActionActive = "OnCharacterActionActive";
        public static readonly string OnAimActionStart = "OnAimActionStart";

        public static readonly string OnInventoryEquip = "OnInventoryEquip";
        public static readonly string OnItemEquip = "OnItemEquip";

        public static readonly string OnAnimatorStartJump = "OnAnimatorStartJump";
        public static readonly string OnAnimatorEndJump = "OnAnimatorEndJump";



        public static readonly string OnTakeDamage = "OnTakeDamage";
        public static readonly string OnHeal = "OnHeal";
        public static readonly string OnDeath = "OnDeath";
        public static readonly string OnRagdoll = "OnRagdoll";







    }
}