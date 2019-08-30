﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    [DisallowMultipleComponent]
    public abstract class CharacterIKBase : MonoBehaviour
    {
        protected const float tinyOffset = .0001f;

        protected Animator animator;
        protected Transform m_transform;
        protected GameObject m_gameObject;


        protected Vector3 m_pivotPosition;
        protected Vector3 m_leftFootPosition;
        protected Vector3 m_rightFootPosition;

        //  ---  Abstract methods    --------------------

        /// <summary>
        /// This method is called in the Start method.  Use it to initialize any character parameters.
        /// </summary>
        protected abstract void Initialize();






        protected virtual void Start()
        {
            if(animator == null) animator = GetComponent<Animator>();
            m_transform = transform;
            m_gameObject = gameObject;

            Initialize();
        }



        //protected virtual void OnAnimatorIK(int layerIndex)
        //{
        //    m_pivotPosition = GetPivotPosition();
        //}



        protected Vector3 GetPivotPosition()
        {
            animator.stabilizeFeet = true;

            Vector3 pivotPosition = animator.pivotPosition;

            m_leftFootPosition = GetFootPosition(AvatarIKGoal.LeftFoot);
            m_rightFootPosition = GetFootPosition(AvatarIKGoal.RightFoot);
            float leftHeight = MathUtil.Round(Mathf.Abs(m_leftFootPosition.y));
            float rightHeight = MathUtil.Round(Mathf.Abs(m_rightFootPosition.y));

            float threshold = 0.1f;
            float pivotDifference = Mathf.Abs(0.5f - animator.pivotWeight);
            float t = Mathf.Clamp(pivotDifference, 0, 0.5f) / 0.5f;
            //  1 means feet are not pivot.
            float feetPivotActive = 1;

            //  Both feet are grouned.
            if ((leftHeight < threshold && rightHeight < threshold) || (leftHeight > threshold && rightHeight > threshold) && pivotDifference < 5f)
            {
                t = Time.deltaTime;
                feetPivotActive = Mathf.Clamp01(feetPivotActive + t);
                pivotPosition = m_transform.position;
            }
            //  If one leg is raised and one is planted.
            else if ((leftHeight < tinyOffset && rightHeight > 0) || rightHeight < tinyOffset && leftHeight > 0)
            {
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                feetPivotActive = Mathf.Lerp(0f, 1f, t);

                animator.feetPivotActive = feetPivotActive;
                pivotPosition = animator.pivotPosition;
            }


            pivotPosition.y = m_transform.position.y;


            CharacterDebug.Log("<color=blue>* FeetPivotWeight *</color>", MathUtil.Round(feetPivotActive));
            return pivotPosition;
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

                return !worldPos ? footPos : m_transform.InverseTransformPoint(footPos);
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
