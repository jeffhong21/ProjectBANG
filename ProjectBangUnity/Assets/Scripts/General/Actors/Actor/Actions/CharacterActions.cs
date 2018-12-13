namespace Bang.CharacterActions
{
    using UnityEngine;
    using System.Collections;

    public abstract class CharacterActions : MonoBehaviour
    {
        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected Animator m_Animator;
        protected ActorController m_Controller;
        protected AnimationHandler m_AnimationHandler;



        protected virtual void Awake()
        {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_Animator = GetComponent<Animator>();
            m_Controller = GetComponent<ActorController>();
        }


        protected virtual void Start()
        {
        }


        public void StartAction()
        {
            if (CanStartAction())
            {
                ActionStarted();
            }
        }


        public void StopAction()
        {
            if (CanStopAction())
            {
                ActionStopped();
            }
        }


        public virtual bool CanStartAction()
        {
            throw new System.NotImplementedException();
        }

        public virtual bool CanStopAction()
        {
            throw new System.NotImplementedException();
        }


        public virtual float GetTransitionDuration()
        {
            throw new System.NotImplementedException();
        }


        public virtual float GetNormalizedTime()
        {
            throw new System.NotImplementedException();
        }


        public virtual bool Move()
        {
            throw new System.NotImplementedException();
        }


        public virtual bool UpdateRotation()
        {
            throw new System.NotImplementedException();
        }


        public virtual bool UpdateMovement()
        {
            throw new System.NotImplementedException();
        }

        public virtual bool UpdateAnimator()
        {
            throw new System.NotImplementedException();
        }



        protected abstract void ActionStarted();

        protected abstract void ActionStopped();
    }
}