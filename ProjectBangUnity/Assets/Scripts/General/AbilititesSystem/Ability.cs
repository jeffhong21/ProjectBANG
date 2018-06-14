namespace Bang.AbilitySystem
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections;
    using System.Collections.Generic;


    public abstract class Ability : ScriptableObject, IAbility
    {

        [SerializeField]
        protected string _displayName;

        [SerializeField]
        protected float _coolDownDuration;

        [SerializeField]
        protected Animator _animator;


        private float _coolDownTimeLeft;

        private GameObject _actor;



        public string displayName{
            get{
                return _displayName;
                }
            set{
                _displayName = value;
            }
        }

        public float coolDownDuration{
            get{
                return _coolDownDuration;
            }
            set{
                _coolDownDuration = value;
            }
        }

        public Animator animator{
            get { 
                return _animator; 
            }
        }

        public float coolDownTimeLeft{
            get{
                return _coolDownTimeLeft;
            }
            protected set{
                _coolDownTimeLeft = Mathf.Clamp(0, coolDownDuration, value);
            }
        }


        protected GameObject actor{
            get { return _actor; }
            set { _actor = value; }
        }



        public void RegisterToHUD(){
            
        }



        public virtual void ExecuteAbility()
        {
            if(coolDownTimeLeft == 0){
                coolDownTimeLeft = coolDownDuration;
            }
            else{
                return;
            }
        }


        protected virtual void Update()
        {
            if(coolDownTimeLeft > 0)
            {
                coolDownTimeLeft -= Time.deltaTime;
                // float roundedCd = Mathf.Round (coolDownTimeLeft);
                // coolDownTextDisplay.text = roundedCd.ToString ();
                // darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
            }
        }






        public static void Create(GameObject actor, string name)
        {
            Ability ability = ScriptableObject.CreateInstance<Ability>();
            ability.displayName = name;
            ability.actor = actor;


            string assetDir = AssetDatabase.GenerateUniqueAssetPath(name + ".asset");


            AssetDatabase.CreateAsset(ability, assetDir);
            AssetDatabase.SaveAssets();

        }

    }


}

