namespace CharacterSkins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class CharacterSkinComponent : MonoBehaviour
    {
        
        public CharacterSkinData charSkinData;
        public CharacterSkinObject eyes;

        public bool enableHat;


        private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField]
        private List<CharacterSkinObject> skinObjects = new List<CharacterSkinObject>();




		//private void OnValidate()
		//{
  //          LoadCharacterSkin();
		//}



		private bool HasRequiredData()
        {
            if (charSkinData == null){
                Debug.LogFormat("CharacterSkinData is missing.");
                return false;
            }

            if(charSkinData.skinSetData == null || charSkinData.textureSetData == null){
                Debug.LogFormat("CharacterSkinData has missing data.");
                return false;
            }

            return true;
        }


        public void LoadCharacterSkin()
        {
            if (HasRequiredData() == false)
                return;
            
            UnloadCharacterSkin();


            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null){
                skinnedMeshRenderer.sharedMesh = charSkinData.skinSetData.characterMesh;
                skinnedMeshRenderer.sharedMaterial = charSkinData.materials[0];
            }


            if (GetComponent<Animator>() == null)
                if (GetComponentInParent<Animator>() == null)
                    return;

            Material material = charSkinData.materials[0];
            if(charSkinData.skinSetData.hairMesh)
            {
                var skinObject = CreateSkinObject("Hair", charSkinData.skinSetData.hairMesh, material, GetTransform(HumanBodyBones.Head));
                skinObjects.Add(skinObject);
            }
            if (charSkinData.skinSetData.hatMesh)
            {
                var skinObject = CreateSkinObject("Hat", charSkinData.skinSetData.hatMesh, material, GetTransform(HumanBodyBones.Head));
                skinObjects.Add(skinObject);
            }
            if (charSkinData.skinSetData.hatHairMesh)
            {
                var skinObject = CreateSkinObject("HatHair", charSkinData.skinSetData.hatHairMesh, material, GetTransform(HumanBodyBones.Head));
                skinObjects.Add(skinObject);
            }
        }


        private void UnloadCharacterSkin()
        {
            for (int i = 0; i < skinObjects.Count; i++)
            {
                if(skinObjects[i] != null)
                    DestroyImmediate(skinObjects[i].gameObject, true);
            }
            skinObjects.Clear();
        }



        private void ToggleHat(bool enableHat)
        {
            if(charSkinData.skinSetData.hatMesh){
                
            }
        }



        public CharacterSkinObject CreateSkinObject(string name, Mesh mesh, Material material, Transform parent = null)
        {
            GameObject skinObject = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(CharacterSkinObject));
            skinObject.transform.parent = parent == null ? transform : parent;
            skinObject.transform.localPosition = Vector3.zero;
            skinObject.transform.localEulerAngles = Vector3.zero;

            skinObject.GetComponent<MeshFilter>().mesh = mesh;
            skinObject.GetComponent<MeshRenderer>().material = material;

            return skinObject.GetComponent<CharacterSkinObject>();
        }

        public Transform GetTransform(HumanBodyBones humanBoneId)
        {
            Animator animator = GetComponent<Animator>();
            if(GetComponent<Animator>() == null)
                animator = GetComponentInParent<Animator>();
            
            if (animator == null)
                return null;
            
            return animator.GetBoneTransform(humanBoneId);
        }

    }
}
