namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class MyDemoObject : MonoBehaviour
    {

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        public void Awake()
        {
            EventHandler.RegisterEvent<CharacterAction, bool>(gameObject, "OnCharacterActionActive", OnAbilityActive);
        }

		private void OnEnable()
		{
			
		}

		/// <summary>
		/// The specified action has started or stopped.
		/// </summary>
		/// <param name="action">The action that has been started or stopped.</param>
		/// <param name="activated">Was the action activated?</param>
		private void OnAbilityActive(CharacterAction action, bool activated)
        {
            Debug.Log(action + " activated: " + activated);
        }

        /// <summary>
        /// The GameObject has been destroyed.
        /// </summary>
        public void OnDestroy()
        {
            EventHandler.UnregisterEvent<CharacterAction, bool>(gameObject, "OnCharacterActionActive", OnAbilityActive);
        }
    }

}

