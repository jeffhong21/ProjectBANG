namespace Bang
{
    using UnityEngine;


    public class CharacterSkin : MonoBehaviour
    {
        
        public CharacterMeshTypes skinType;
        public CharacterTextureTypes textureType;
        [Range(0,3)]
        public int textureIndex;
        public SkinnedMeshRenderer mesh;
        [SerializeField]
        protected Material material;
        public CharacterSkinManager skinManager;


		private void OnEnable()
		{
            if(mesh != null) material = mesh.sharedMaterial;
		}


		private void OnValidate()
		{
            if (skinManager != null && mesh != null)
            {
                skinManager.LoadCharacter(mesh, skinType, textureType, textureIndex);
            }
		}


		//public void LoadCharacter()
        //{
        //    if(skinManager != null && mesh != null)
        //    {
        //        skinManager.LoadCharacter(mesh, skinType, textureType, textureIndex);
        //    }
        //    else
        //    {
        //        Debug.Log(" ** Did not load any character skin. **");
        //    }
        //}



    }
}


