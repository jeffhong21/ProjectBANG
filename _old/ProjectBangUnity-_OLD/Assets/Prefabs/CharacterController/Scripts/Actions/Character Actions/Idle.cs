namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Idle : CharacterAction
    {

        protected float m_IdleTimeout = 3f;
        protected float m_StartIdleTime;
        protected float m_NextIdleTime;



        //
        // Methods
        //
        protected virtual void Reset()
        {
            m_StartType = ActionStartType.Automatic;
            m_StopType = ActionStopType.Automatic;
        }


        //public override bool CanStartAction()
        //{
        //    if(!m_Controller.Moving){
        //        if(Time.time > m_NextIdleTime){
        //            m_NextIdleTime = Time.time + m_IdleTimeout;
        //            return true;
        //        }
        //    }
        //    return false;
        //}




        //public override bool CanStopAction()
        //{
        //    return false;
        //}



        //protected override void ActionStarted()
        //{

        //}



        //protected override void ActionStopped()
        //{
        //    //m_NextIdleTime = 0;
        //}

        //public override void UpdateAction()
        //{

        //}


        //public override bool UpdateAnimator()
        //{
        //    return true;
        //}


		public override bool IsConcurrentAction()
		{
            return true;
		}

		////  Returns the state the given layer should be on.
		//public override string GetDestinationState(int layer)
		//{
		//    return "Idle";
		//}





	}

}

