using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController.DebugUI
{
    public class DebugCharacterUI : MonoBehaviour
    {
        private static DebugCharacterUI _instance;
        public static DebugCharacterUI Instance
        {
            get; private set;
        }
        //public static DebugCharacterUI Instance
        //{
        //    get
        //    {
        //        if(_instance == null){
        //            var prefab = Resources.Load<DebugCharacterUI>("DebugCharacterUI");
        //            _instance = Instantiate(prefab);
        //        }
        //        return _instance;
        //    }
        //    private set
        //    {
        //        _instance = value;
        //    }
        //}


    }

}