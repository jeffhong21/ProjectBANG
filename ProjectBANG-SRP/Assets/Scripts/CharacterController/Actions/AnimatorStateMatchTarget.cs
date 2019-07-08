using UnityEngine;
using System;


namespace CharacterController
{
    [Serializable]
    public class AnimatorStateMatchTarget
    {
        public string stateName;

        public Vector3 matchTargetOffset = new Vector3(0.1f, 0, 0);
        [Range(0.01f,0.99f)]
        public float startMatchTarget = 0.1f;
        [Range(0.01f, 0.99f)]
        public float endMatchTarget = 0.2f;
        //[MinMaxRange(0, 1)]
        //public Vector2 matchTargetRange = new Vector2(0.1f, 0.2f);

        public AvatarTarget avatarTarget = AvatarTarget.Root;


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




        //
        //  Methods
        //

        public static HumanBodyBones GetHumanBodyBone(AvatarTarget target)
        {
            switch (target)
            {
                case AvatarTarget.LeftHand:
                    return HumanBodyBones.LeftHand;
                case AvatarTarget.RightHand:
                    return HumanBodyBones.RightHand;
                case AvatarTarget.LeftFoot:
                    return HumanBodyBones.LeftFoot;
                case AvatarTarget.RightFoot:
                    return HumanBodyBones.RightFoot;
                case AvatarTarget.Body:
                    return HumanBodyBones.Hips;
                default:
                    return HumanBodyBones.Hips;
            }
        }
    }

}
