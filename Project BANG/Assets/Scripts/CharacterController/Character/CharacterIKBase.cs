using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    [DisallowMultipleComponent]
    public abstract class CharacterIKBase : MonoBehaviour
    {


        protected Animator animator;
        protected Transform mTransform;
        protected GameObject mGameObject;


        //  ---  Abstract methods    --------------------

        /// <summary>
        /// This method is called in the Start method.  Use it to initialize any character parameters.
        /// </summary>
        protected abstract void Initialize();






        protected virtual void Start()
        {
            if(animator == null) animator = GetComponent<Animator>();
            mTransform = transform;
            mGameObject = gameObject;

            Initialize();
        }






        protected Vector3 GetFootPosition(AvatarIKGoal foot, bool worldPos = false)
        {
            if (foot == AvatarIKGoal.LeftFoot || foot == AvatarIKGoal.RightFoot)
            {
                Vector3 footPos = animator.GetIKPosition(foot);
                Quaternion footRot = animator.GetIKRotation(foot);
                float botFootHeight = foot == AvatarIKGoal.LeftFoot ? animator.leftFeetBottomHeight : animator.rightFeetBottomHeight;
                Vector3 footHeight = new Vector3(0, -botFootHeight, 0);


                footPos += footRot * footHeight;

                return !worldPos ? footPos : mTransform.InverseTransformPoint(footPos);
            }

            return Vector3.zero;
        }



        /// <summary>
        /// Create Transforms to be used as targets for IK.
        /// </summary>
        /// <param name="effectorName"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="hideFlag"></param>
        /// <returns></returns>
        protected Transform CreateIKEffectors(string effectorName, Vector3 position, Quaternion rotation, bool hideFlag = true)
        {
            Transform effector = new GameObject(effectorName).transform;
            effector.position = position;
            effector.rotation = rotation;
            effector.parent = transform;

            //if(hideFlag) effector.hideFlags = HideFlags.HideInHierarchy;
            return effector;
        }
    }

}
