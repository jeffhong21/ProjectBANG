namespace CharacterController
{
    using UnityEngine;


    public abstract class CharacterAction : MonoBehaviour
    {
        //
        // Fields
        //
        protected int m_Index;
        protected string m_InputName;
        protected GameObject m_GameObject;
        protected Transform m_Transform;
        protected Animator m_Animator;
        protected CharacterLocomotion m_Controller;


        //
        // Properties
        //
        public int Index{
            get { return m_Index; }
            set { m_Index = value; }
        }

        public string InputName
        {
            get { return m_InputName; }
            set { m_InputName = value; }
        }



        //
        // Methods
        //
        protected virtual void Awake()
        {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_Animator = GetComponent<Animator>();
            m_Controller = GetComponent<CharacterLocomotion>();
        }


        protected virtual void Start()
        {
        }


        // Executed on every action to allow the action to update.
        // The action may need to update if it needs to do something when inactive or show a GUI icon when the ability can be started.
        public void UpdateAction()
        {
            
        }


        public void StartAction()
        {
            ActionStarted();
        }


        public void StopAction()
        {
            ActionStopped();
        }


        /// <summary>
        /// Checks if action can be started.
        /// </summary>
        /// <returns><c>true</c>, if start action was caned, <c>false</c> otherwise.</returns>
        public virtual bool CanStartAction()
        {
            return true;
        }


        public virtual bool CanStopAction()
        {
            return true;
        }


        public virtual string GetDestinationState(int layer)
        {
            return "";
        }


        public virtual float GetTransitionDuration()
        {
            return 0f;
        }


        public virtual float GetNormalizedTime()
        {
            float normalizedTime = m_Animator.GetCurrentAnimatorStateInfo(m_Index).normalizedTime;
            return normalizedTime;
        }


        /// <summary>
        /// Updates the animator.  If true is returned, controller can continue with its animation.  
        /// If false is returned, controller stops the current animation
        /// </summary>
        /// <returns><c>true</c>, if animator was updated, <c>false</c> otherwise.</returns>
        public virtual bool UpdateAnimator()
        {
            return true;
        }



        protected abstract void ActionStarted();

        protected abstract void ActionStopped();
    }
}