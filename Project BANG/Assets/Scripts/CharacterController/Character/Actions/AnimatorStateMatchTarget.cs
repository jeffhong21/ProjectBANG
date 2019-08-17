using UnityEngine;
using System;


namespace CharacterController
{
    [Serializable]
    public class AnimatorStateMatchTarget
    {
        public bool enableMatchTarget = true;

        public string stateName;

        public Vector3 matchTargetOffset = new Vector3(0.1f, 0, 0);
        [Range(0.01f,0.99f)]
        public float startMatchTarget = 0.1f;
        [Range(0.01f, 0.99f)]
        public float endMatchTarget = 0.2f;
        //[MinMaxRange(0, 1)]
        //public Vector2 matchTargetRange = new Vector2(0.1f, 0.2f);

        public AvatarTarget avatarTarget = AvatarTarget.Root;
        public Vector3 positionXYZWeight = Vector3.one;
        [Range(0f, 1f)]
        public float rotationWeight = 1;

        public MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 1);

        //
        //  Properties
        //

        public HumanBodyBones HumanBodyBone
        {
            get
            {
                var bone = HumanBodyBones.Hips;
                switch (avatarTarget)
                {
                    case AvatarTarget.LeftHand:
                        bone = HumanBodyBones.LeftHand;
                        break;
                    case AvatarTarget.RightHand:
                        bone = HumanBodyBones.RightHand;
                        break;
                    case AvatarTarget.LeftFoot:
                        bone = HumanBodyBones.LeftFoot;
                        break;
                    case AvatarTarget.RightFoot:
                        bone = HumanBodyBones.RightFoot;
                        break;
                    case AvatarTarget.Body:
                        bone = HumanBodyBones.Hips;
                        break;
                }
                return bone;
            }
        }





    }

}
