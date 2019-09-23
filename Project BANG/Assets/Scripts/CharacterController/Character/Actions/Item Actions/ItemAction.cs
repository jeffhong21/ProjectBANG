namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    public class ItemAction : CharacterAction
    {


        [SerializeField, HideInInspector]
        protected int m_ItemStateID = -1;


        public virtual int ItemStateID { get { return m_ItemStateID; } set { m_ItemStateID = Mathf.Clamp(value, -1, int.MaxValue); } }


        public virtual bool ExecuteItemAction(int index)
        {
            if (CanStartAction()) {
                StartAction();
                return true;
            }
            return false;
        }

    }

}
