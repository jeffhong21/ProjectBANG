namespace Bang
{
    using UnityEngine;
    using System;


    public class PlayerInput1 : MonoBehaviour
    {
        [SerializeField]
        protected bool useRawInput = true;
        [SerializeField, Range(0.1f, 1)]
        protected float sensitivity = 1f;
        [SerializeField]
        protected Vector2 playerInput;
        [SerializeField]
        protected bool rolling;

        protected PlayerController playerCtrl;



        public Vector2 PlayerInput{
            get{
                return playerInput;
            }
        }


        const float attackInputDuration = 0.03f;

        WaitForSeconds attackInputWait;
        Coroutine attackWaitCoroutine;


        void Awake()
        {
            playerCtrl = GetComponent<PlayerController>();

            //attackInputWait = new WaitForSeconds(attackInputDuration);
        }


        void Update()
        {
            if (useRawInput)
            {
                playerInput.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
            else
            {
                playerInput.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }


            if (InputManager.Space)
            {
                Debug.Log("Rolling");
            }
            else if (InputManager.LMB)
            {
                playerCtrl.FireWeapon(playerCtrl.CursorPosition);
                //Debug.Log("Shooting");
            }
            else if (InputManager.RMB)
            {

            }

            else if (InputManager.R)
            {
                Debug.LogFormat("<color=#800080ff>{0}</color>.  Current ammo is <color=#800080ff>{1}</color>", "Reloading weapon", playerCtrl.weapon.currentAmmo);  // purple
            }

            //  Check if player is moving.
            //isMoving = Math.Abs(playerInput.x) >= 0.1f || Math.Abs(playerInput.y) >= 0.1f ? true : false;
        }



    }
}


