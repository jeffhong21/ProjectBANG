namespace CharacterSkins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public interface ISetData
    {
        string SetId { get;}
    }


    [CreateAssetMenu(fileName = "SkinSetData", menuName = "Character Skins/Set Data/Skin Set Data")]
    public class SkinSetData : ScriptableObject, ISetData
    {
        [SerializeField]
        protected string setId;
        [Header("-- Mesh --")]
        [SerializeField]
        protected Mesh _characterMesh;
        [Header("-- Hair --")]
        [SerializeField]
        protected Mesh _hairMesh;
        [Header("-- Hat --")]
        [SerializeField]
        protected Mesh _hatMesh;
        [SerializeField]
        protected Mesh _hatHairMesh;
        [Header("-- Facial Hair --")]
        [SerializeField]
        protected Mesh _facialHairMesh;




        public string SetId{
            get { return setId; }
        }

        public Mesh characterMesh{
            get { return _characterMesh; }
        }

        public Mesh hairMesh{
            get { return _hairMesh; }
        }

        public Mesh hatMesh{
            get { return _hatMesh; }
        }

        public Mesh hatHairMesh{
            get { return _hatHairMesh; }
        }

        public Mesh facialHairMesh{
            get { return _facialHairMesh; }
        }




    }



}
