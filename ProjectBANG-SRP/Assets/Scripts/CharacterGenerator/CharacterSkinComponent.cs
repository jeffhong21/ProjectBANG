namespace CharacterSkins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class SkinIDs
    {
        public static string Hair = "Hair";
        public static string HatHair = "HatHair";
    }

    public class CharacterSkinComponent : MonoBehaviour
    {


        [SerializeField]
        private SkinSetData skinData;
        [SerializeField]
        private Material skinOverrideMaterial;
        [SerializeField]
        private CharacterSkinObject eyes;
        [SerializeField]
        private bool enableHat;
        [SerializeField]
        private CharacterSkinObject hat;

        private CharacterSkinObject hair;
        private CharacterSkinObject hatHair;



        private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField]
        private List<CharacterSkinObject> skinObjects = new List<CharacterSkinObject>();
        private Dictionary<string, CharacterSkinObject> skinObjectsLookup ;




		private bool HasRequiredData()
        {
            if (skinData == null){
                Debug.LogFormat("SkinData is missing.");
                return false;
            }

            if(skinData.CharacterMesh == null || skinData.SkinTextureSet == null){
                Debug.LogFormat("CharacterSkinData has missing data.");
                return false;
            }

            return true;
        }



        private void Reload()
        {
            if(skinObjectsLookup == null)
                skinObjectsLookup = new Dictionary<string, CharacterSkinObject>();

            for(int i = 0; i < skinObjects.Count; i ++){
                if(skinObjects[i] == null){
                    skinObjects.RemoveAt(i);
                    continue;
                }
                if(skinObjectsLookup.ContainsKey(skinObjects[i].skinId) == false){
                    skinObjectsLookup.Add(skinObjects[i].skinId, skinObjects[i]);
                }
            }
        }


        private void Initialize()
        {
            if (GetComponent<Animator>() == null)
                if (GetComponentInParent<Animator>() == null)
                    return;

            if(skinData.HairMesh)
                hair = CreateSkinObject(SkinIDs.Hair, GetTransform(HumanBodyBones.Head));
            if(skinData.HatHairMesh)
                hatHair = CreateSkinObject(SkinIDs.HatHair, GetTransform(HumanBodyBones.Head));
        }


        public void LoadCharacterSkin()
        {
            if (HasRequiredData() == false)
                return;
            
            // UnloadCharacterSkin();

            Material skinMaterial = skinOverrideMaterial != null ? skinOverrideMaterial : skinData.SkinMaterial;

            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null){
                skinnedMeshRenderer.sharedMesh = skinData.CharacterMesh;
                skinnedMeshRenderer.sharedMaterial = skinMaterial;
            }


            // //  Create hair.
            // if(hair && skinData.HairMesh)
            //     UpdateMesh(SkinIDs.Hair, skinData.HairMesh, skinMaterial);

            
            
            //  Create eyes.

        }


        private void UnloadCharacterSkin()
        {
            for (int i = 0; i < skinObjects.Count; i++){
                if(skinObjects[i] != null)
                    DestroyImmediate(skinObjects[i].gameObject, true);
            }
            skinObjects.Clear();
            skinObjectsLookup.Clear();
        }




        private void UpdateMesh(CharacterSkinObject skinObject, Mesh mesh, Material material)
        {
            MeshFilter meshFilter = skinObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = skinObject.GetComponent<MeshRenderer>();
            meshFilter.mesh = mesh;
            meshRenderer.material = material;
        }


        private CharacterSkinObject CreateSkinObject(string name, Transform parent = null)
        {
            GameObject skinObject = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(CharacterSkinObject));
            skinObject.transform.parent = parent == null ? transform : parent;
            skinObject.transform.localPosition = Vector3.zero;
            skinObject.transform.localEulerAngles = Vector3.zero;

            skinObjects.Add(skinObject);
            return skinObject.GetComponent<CharacterSkinObject>();
        }


        private CharacterSkinObject CreateSkinObject(string name, Mesh mesh, Material material, Transform parent = null)
        {
            GameObject skinObject = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(CharacterSkinObject));
            skinObject.transform.parent = parent == null ? transform : parent;
            skinObject.transform.localPosition = Vector3.zero;
            skinObject.transform.localEulerAngles = Vector3.zero;

            skinObject.GetComponent<MeshFilter>().mesh = mesh;
            skinObject.GetComponent<MeshRenderer>().material = material;

            skinObjects.Add(skinObject);
            return skinObject.GetComponent<CharacterSkinObject>();
        }

        private Transform GetTransform(HumanBodyBones humanBoneId)
        {
            Animator animator = GetComponent<Animator>();
            if(GetComponent<Animator>() == null)
                animator = GetComponentInParent<Animator>();
            
            if (animator == null)
                return null;
            
            return animator.GetBoneTransform(humanBoneId);
        }



        private void ToggleHat(bool enableHat)
        {
            if(charSkinData.skinSetData.hatMesh){
                
            }
        }
    }
}
