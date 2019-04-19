namespace CharacterController
{
    using UnityEngine;



    public class LayerManager : MonoBehaviour
    {
        //
        // Fields
        //
        [SerializeField]
        protected LayerMask m_EnemyLayer;
        [SerializeField]
        protected LayerMask m_InvisibleLayer;
        [SerializeField]
        protected LayerMask m_GroundLayer;



        public LayerMask EnemyLayer
        {
            get { return m_EnemyLayer; }
            set { m_EnemyLayer = value; }
        }

        public LayerMask InvisibleLayer
        {
            get { return m_InvisibleLayer; }
            set { m_InvisibleLayer = value; }
        }

        public LayerMask GroundLayer
        {
            get { return m_GroundLayer; }
            set { m_GroundLayer = value; }
        }



		private void Awake()
		{
			
		}
	}

}

