namespace Bang
{
    using UnityEngine;
    using UtilityAI;


    /// <summary>
    /// Switches the attack target to the attacker that is attacking the agent.
    /// </summary>
    public class CheckForTargetSwitch : ActionBase
    {

        private GameObject _previous;
        private GameObject _next;
        private GameObject _firstAttacker;
        [SerializeField]
        private GameObject _attackTarget;

        public GameObject attackTarget{
            get{
                return _attackTarget;
            }
        }


        public override void Execute(IAIContext context)
        {


            if(_attackTarget != null)
            {
                if(_previous != null)
                {
                    //_previous._next = _next;
                }
                else
                {
                    //_attackTarget._firstAttacker = _next;
                }
                if(_next != null)
                {
                    //_next._previous = _previous;
                }

                _previous = _next = null;
            }

            _attackTarget = attackTarget;

            if(_attackTarget != null)
            {
                //_next = _attackTarget._firstAttacker;
                if (_next != null)
                {
                    //_next._previous = this;
                }

                //_attackTarget._firstAttacker = this;
            }



            //  Compare shared game object


            //  Add target to gameobject set.


            //  Get Position.  Get the position of the attacking target.


            //  Set bool.  Start searching for the attacking target.


            //  Reset the target.
        }
    }
}


