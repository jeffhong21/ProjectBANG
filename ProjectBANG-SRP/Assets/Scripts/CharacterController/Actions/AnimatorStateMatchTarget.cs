using UnityEngine;
using System;


namespace CharacterController
{
    [Serializable]
    public class AnimatorStateMatchTarget
    {
        public string stateName;

        public float threshold;

        public Vector3 matchTargetOffset = new Vector3(0.1f, 0.1f, 0);
        [Range(0.01f, 1)]
        public float startMatchTarget = 0.01f;
        [Range(0.01f, 1)]
        public float stopMatchTarget = 0.1f;

        public AvatarTarget avatarTarget = AvatarTarget.LeftHand;


    }

}
