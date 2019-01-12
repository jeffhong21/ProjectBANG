using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Bang
{
    [RequireComponent(typeof(Animator))]
    public class CharacterIdles : MonoBehaviour
    {
        public static readonly int InputAnim = Animator.StringToHash("Animation");

        public Animator anim;



        void Awake()
        {
            if (anim == null) GetComponent<Animator>();
        }



        void Update()
        {
            
            if(Input.GetKeyDown(KeyCode.Alpha0))
            {
                anim.SetInteger(InputAnim, 0);
            }
            else if(InputManager.Alpha1)
            {
                anim.SetInteger(InputAnim, 1);
            }
            else if (InputManager.Alpha2)
            {
                anim.SetInteger(InputAnim, 2);
            }
            else if (InputManager.Alpha3)
            {
                anim.SetInteger(InputAnim, 3);
            }
            else if (InputManager.Alpha4)
            {
                anim.SetInteger(InputAnim, 4);
            }



        }




    }
}

