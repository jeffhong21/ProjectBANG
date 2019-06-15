namespace CharacterController
{
    using UnityEngine;


    [DisallowMultipleComponent]
    public class LayerManager : MonoBehaviour
    {
        public const int Climbable = 24;
        public const int Vaultable = 25;
        public const int Solid = 26;
        public const int VisualEffects = 27;

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



		//private void Awake()
		//{
			
		//}



        public static void UpdateLayers()
        {

        }
	}

}

