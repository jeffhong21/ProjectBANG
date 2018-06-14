namespace Bang.AbilitySystem
{
    using UnityEngine;
    using System;


    public interface IAbility
    {
        string displayName{
            get;
            set;
        }

        float coolDownDuration{
            get;
            set;
        }

        float coolDownTimeLeft{
            get;
        }

        
        void ExecuteAbility();

    }

}

