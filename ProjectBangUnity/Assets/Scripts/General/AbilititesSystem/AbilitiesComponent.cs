namespace Bang.AbilitySystem
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class Abilities
    {
        [SerializeField]
        protected Ability[] abilitiesSlot = new Ability[Enum.GetNames(typeof(AbilitySlot)).Length];

        [SerializeField]
        protected Dictionary<AbilitySlot, Ability> _abilities;

        public Dictionary<AbilitySlot, Ability> abilities
        {
            get
            {
                if(_abilities == null)
                {
                    foreach(AbilitySlot ability in Enum.GetValues(typeof(AbilitySlot)))
                    {
                        _abilities.Add(ability, abilitiesSlot[Convert.ToInt32(ability)]);
                    }
                }

                foreach(AbilitySlot ability in Enum.GetValues(typeof(AbilitySlot)))
                {
                    _abilities[ability] = abilitiesSlot[Convert.ToInt32(ability) ] ;
                }
                return _abilities;
            }
        }

    }


    
}

