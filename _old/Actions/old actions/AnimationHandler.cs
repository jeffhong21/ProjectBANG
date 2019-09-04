namespace CharacterController
{
    using UnityEngine;
    using UnityEngine.Animations;
    using System;
    using System.Collections.Generic;


    public class AnimationHandler : MonoBehaviour
    {
        Dictionary<int, string> AnimStateHashes;



        //[SerializeField]
        private bool m_DebugStateChanges;
        //[SerializeField]
        private float m_HorizontalInputDampTime = 0.1f;
        //[SerializeField]
        private float m_ForwardInputDampTime = 0.1f;

        [Header("--  Agent Context --")]
        [SerializeField]
        private float normalizeTime;
        [SerializeField]
        private int shortNameHash;
        [SerializeField]
        private string state;
        [SerializeField]
        private int PistolAimHash;
        [SerializeField]
        private int RifleEquipHash;


        private int ItemLayerIndex = 1;

        private Animator m_Animator;
        private RuntimeAnimatorController m_RuntimeAnimator;



        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_RuntimeAnimator = m_Animator.runtimeAnimatorController;
        }


        private void Start()
        {
            AnimStateHashes = new Dictionary<int, string>()
            {
                { Animator.StringToHash("Pickup_Item.Pickup_Item_Ground"), "Pickup_Item.Pickup_Item_Ground"},
                { Animator.StringToHash("Jump.Jump"), "Jump.Jump"},
                { Animator.StringToHash("Pistol.Aim"), "Pistol.Aim"},
                { Animator.StringToHash("Rifle.Equip"), "Rifle.Equip"}
            };
        }


		private void Update()
		{
            normalizeTime = m_Animator.GetCurrentAnimatorStateInfo(ItemLayerIndex).normalizedTime % 1;
            shortNameHash = m_Animator.GetCurrentAnimatorStateInfo(ItemLayerIndex).shortNameHash;
            if(AnimStateHashes.ContainsKey(shortNameHash)){
                state = AnimStateHashes[shortNameHash];

                Debug.Break();
            }else{
                state = "";
            }
		}



		public void SetForwardInputValue(float value)
        {
            //m_Animator.SetFloat(HashID.ForwardInput, value, m_ForwardInputDampTime, Time.deltaTime);
        }


        public void SetHorizontalInputValue(float value)
        {
            //m_Animator.SetFloat(HashID.HorizontalInput, value, m_HorizontalInputDampTime, Time.deltaTime);
        }

        public void ExecuteEvent(string eventName)
        {
            //Debug.Log(eventName);
            //EventHandler.ExecuteEvent(gameObject, eventName);
        }




    }

}