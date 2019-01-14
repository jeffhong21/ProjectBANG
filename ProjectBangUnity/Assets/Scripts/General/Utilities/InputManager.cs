namespace Bang
{
    using System;
    using UnityEngine;

    /// <summary>
    ///  This script manages the mouse input.
    ///  
    /// </summary>
    public static class InputManager
    {
        
        public static bool LMB
        {
            get{ return Input.GetKeyUp(KeyCode.Mouse0); }
        }

        public static bool RMB
        {
            get { return Input.GetKeyUp(KeyCode.Mouse1); }
        }

        public static bool Space
        {
            get { return Input.GetKeyUp(KeyCode.Space); }
        }

        public static bool SpaceHold
        {
            get { return Input.GetKeyDown(KeyCode.Space); }
        }

        public static bool ShiftDown
        {
            get { return Input.GetKey(KeyCode.LeftShift); }
        }

        public static bool E
        {
            get { return Input.GetKeyDown(KeyCode.E); }
        }

        public static bool Q
        {
            get { return Input.GetKeyDown(KeyCode.Q); }
        }

        public static bool R
        {
            get { return Input.GetKeyDown(KeyCode.R); }
        }

        public static bool F
        {
            get { return Input.GetKeyDown(KeyCode.F); }
        }

        public static bool Alpha1
        {
            get { return Input.GetKeyDown(KeyCode.Alpha1); }
        }

        public static bool Alpha2
        {
            get { return Input.GetKeyDown(KeyCode.Alpha2); }
        }

        public static bool Alpha3
        {
            get { return Input.GetKeyDown(KeyCode.Alpha3); }
        }

        public static bool Alpha4
        {
            get { return Input.GetKeyDown(KeyCode.Alpha4); }
        }

        public static bool Alpha5
        {
            get { return Input.GetKeyDown(KeyCode.Alpha5); }
        }

        public static bool ESC
        {
            get { return Input.GetKeyUp(KeyCode.Escape); }
        }


        //  All the Keycode values.
        static int[] values = (int[])System.Enum.GetValues(typeof(KeyCode));
        //  Keys that are pressed.
        static bool[] keys = new bool[values.Length];


        public static int? ShiftIndexInput
        {
            get
            {
                bool keyInput = false;
                int? index = null;
                for (int i = 0; i < values.Length; i++)
                {
                    keys[i] = Input.GetKeyUp((KeyCode)values[i]);
                    if (keys[i])
                    {
                        keyInput = keys[i];
                        break;
                    }
                }
                if(keyInput && Input.GetKeyDown(KeyCode.LeftShift)){
                    return index;
                }
                else{
                    return null;
                }
            }
        }

    }
}