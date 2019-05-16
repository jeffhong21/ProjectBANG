namespace CharacterSkins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class CharacterSkinObject : MonoBehaviour
    {
        [DisplayOnly]
        protected string _skinId;
        public string skinId{
            get{
                if(_skinId == string.IsNullOrWhiteSpace)
                    _skinId = gameObject.name;
                return skinId;
            }
            set{_skinId = value; }
        }

        protected HumanBodyBones bodyBone;






    }

}
