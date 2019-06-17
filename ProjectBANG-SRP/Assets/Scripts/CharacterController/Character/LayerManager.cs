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
        protected LayerMask m_SolidLayers;



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

        public LayerMask SolidLayers
        {
            get { return m_SolidLayers; }
            set { m_SolidLayers = value; }
        }







        public static void UpdateLayers()
        {

        }
	}

}

