namespace Bang
{
    using System.Collections.Generic;
    using UnityEngine;


    [RequireComponent(typeof(BoxCollider))]
    public class CoverObject : MonoBehaviour
    {
        private static readonly string objectTag = "Cover";



        private BoxCollider _col;

        private Vector3[] _sides = new Vector3[4];

        private Vector3[] _corners = new Vector3[4];

        [SerializeField]
        private float _entitySize = 0.5f;
        [SerializeField]
        private List<Vector3> _coverSpots = new List<Vector3>();

        private Dictionary<Vector3, GameObject> occupants = new Dictionary<Vector3, GameObject>();

        private int density = 2;

        [SerializeField]
        private CoverDebugOptions debug;



        public BoxCollider Col{
            get{return _col;}
        }

        public Vector3 Position{
            get{
                return transform.position;
            }
        }

        public Vector3[] Sides{
            get { return _sides; }
        }

        public Vector3[] Corners{
            get { return _corners; }
        }

        public float EntitySize{
            get { return _entitySize; }
        }

        public List<Vector3> CoverSpots{
            get { return _coverSpots; }
        }

        public Dictionary<Vector3, GameObject> Occupants{
            get { return occupants; }
        }

        public CoverDebugOptions DebugOptions{
            get { return debug; }
        }





		private void Awake()
		{
            Init();
		}

		private void OnEnable()
		{
            SetupCoverLocations();
		}


		public void Init()
        {
            _col = GetComponent<BoxCollider>();
            gameObject.tag = objectTag;
            if(occupants == null) occupants = new Dictionary<Vector3, GameObject>();
        }


		public void SetupCoverLocations()
        {
            Init();
            CoverSpots.Clear();

            Sides[0] = Position + new Vector3(0, 0, Col.size.z) * 0.5f;
            Sides[1] = Position + new Vector3(-(Col.size.x), 0, 0) * 0.5f;
            Sides[2] = Position + new Vector3(0, 0, -(Col.size.z) ) * 0.5f;
            Sides[3] = Position + new Vector3(Col.size.x, 0, 0) * 0.5f;

            Corners[0] = Position + new Vector3(Col.size.x, 0, Col.size.z) * 0.5f;
            Corners[1] = Position + new Vector3(Col.size.x, 0, -Col.size.z) * 0.5f;
            Corners[2] = Position + new Vector3(-Col.size.x, 0, -Col.size.z) * 0.5f;
            Corners[3] = Position + new Vector3(-Col.size.x, 0, Col.size.z) * 0.5f;


            for (int i = 0; i < Corners.Length; i++)
            {
                //int next = i + 1 % Corners.Length == 1 ? 0 : i + 1;
                int next = i + 1;
                if (next >= Corners.Length) next = 0;

                Vector3 dir = (Corners[next] - Corners[i]).normalized;

                Vector3 start = Corners[i] + (EntitySize * dir);
                CoverSpots.Add(start);

                Vector3 end = Corners[next] - (EntitySize * dir);
                CoverSpots.Add(end);
            }





            //for (int i = 0; i < CoverSpots.Count; i++)
            //{
            //    Vector3 direction = Sides[i] - Position;
            //    CoverSpots[i] = Sides[i] + (direction.normalized * EntitySize);

            //    if(occupants.ContainsKey(CoverSpots[i]) == false){
            //        occupants.Add(CoverSpots[i], null);
            //    }
            //}
        }





        /// <summary>
        /// Returns true if entity is within range of any coverspot.
        /// </summary>
        /// <returns><c>true</c>, if in range was ised, <c>false</c> otherwise.</returns>
        /// <param name="entity">Entity.</param>
        /// <param name="rangeModifier">Range modifier.</param>
        public bool IsInRange(Transform entity, float rangeModifier = 1f)
        {
            float distance = 0f;
            float range = EntitySize * rangeModifier;

            for (int i = 0; i < CoverSpots.Count; i++){
                distance = Vector3.Distance(CoverSpots[i], entity.position);
                if (distance < range){
                    return true;
                }
            }
            return false;
        }




        public bool TakeCoverSpot(GameObject entity)
        {
            bool takingCoverSpot = false;

            if(IsInRange(entity.transform))
            {
                int coverSpotIndex = -1;
                float closest = float.MaxValue;
                float distance;

                for (int i = 0; i < CoverSpots.Count; i++)
                {
                    distance = Vector3.Distance(CoverSpots[i], entity.transform.position);
                    if (distance < closest){
                        closest = distance;
                        //  Check if position is occupied.
                        if(occupants.ContainsKey(CoverSpots[i])){
                            if (occupants[CoverSpots[i]] == null){
                                coverSpotIndex = i;
                                takingCoverSpot = true;
                            }
                        }
                    }
                }
                if (coverSpotIndex >= 0){
                    var coverPosition = CoverSpots[coverSpotIndex];
                    occupants[coverPosition] = entity;
                }
            }

            return takingCoverSpot;
        }


        public bool LeaveCoverSpot(GameObject entity)
        {
            bool leavingCover = false;
            for (int i = 0; i < occupants.Count; i++){
                var position = CoverSpots[i];
                if(occupants[position] == entity){
                    occupants[position] = null;
                    leavingCover = true;
                }
            }
            return leavingCover;
        }



		private void Reset()
		{
            _sides = new Vector3[4];
            _coverSpots = new List<Vector3>();
		}


        private void OnDrawGizmosSelected()
		{
            for (int i = 0; i < CoverSpots.Count; i++){
                if (CoverSpots[i] != Vector3.zero){
                    if(Occupants != null){
                        if(Occupants.ContainsKey(CoverSpots[i])){
                            Gizmos.color = Occupants[CoverSpots[i]] == null ? DebugOptions.EmptyColor : DebugOptions.OccupiedColor;
                        }
                    } else {
                        Gizmos.color = Color.white;
                    }
                    Gizmos.DrawSphere(CoverSpots[i], debug.Size);
                }
            }
		}



        public Vector3? GetCoverSpot(GameObject entity)
        {
            foreach (var spot in occupants)
            {
                if (spot.Value == entity)
                {
                    return spot.Key;
                }
            }

            return null;
        }


        public Vector3 GetClosestSpot(Transform entity, float rangeModifier = 1f)
        {
            float distance = 0f;
            float range = EntitySize * rangeModifier;
            float closest = float.MaxValue;
            Vector3 closestSpot = Vector3.zero;

            for (int i = 0; i < CoverSpots.Count; i++)
            {
                distance = Vector3.Distance(CoverSpots[i], entity.position);
                if (distance < closest + range)
                {
                    closest = distance;
                    closestSpot = CoverSpots[i];
                }
            }

            return closestSpot;
        }





        public class CoverPosition
        {
            public GameObject entity;
            public Vector3 location;
            public Vector3 neighborA;
            public Vector3 neighborB;
        }





		[System.Serializable]
        public class CoverDebugOptions
        {
            [SerializeField]
            private float _size = 0.5f;
            [SerializeField]
            private Color _emptyColor = Color.green;
            [SerializeField]
            private Color _occupiedColor = Color.yellow;


            private Vector3 _boxSize = new Vector3();


            public float Size
            {
                get { return _size; }
            }

            public Vector3 BoxSize
            {
                get
                {
                    _boxSize.Set(_size, _size, _size);
                    return _boxSize;
                }
            }

            public Color EmptyColor
            {
                get { return _emptyColor; }
            }

            public Color OccupiedColor
            {
                get { return _occupiedColor; }
            }

        }
	}
}