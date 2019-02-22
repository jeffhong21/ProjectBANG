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
        protected LayerMask m_SolidLayer;



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

        public LayerMask SolidLayer
        {
            get { return m_SolidLayer; }
            set { m_SolidLayer = value; }
        }



		private void Awake()
		{
			
		}
	}

}

