namespace CharacterSkins
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class SkinIDs
    {
        public static string Hair = "HairObject";
        public static string HatHair = "HatHairObject";
    }

    public class CharacterSkinComponent : MonoBehaviour
    {
        [Header("-- Character Skin Data --")]
        [SerializeField]
        private CharacterSkinData skinData;
        [SerializeField]
        private Material skinOverrideMaterial;
        //[SerializeField]
        private CharacterSkinObject eyes;
        [SerializeField]
        private bool enableHat;
        [SerializeField]
        private CharacterSkinObject hat;

        [Header("-- SkinObject Reference --")]
        [SerializeField]
        private CharacterSkinObject hairObject;
        [SerializeField]
        private CharacterSkinObject hatHairObject;
        private CharacterSkinObject eyesObject;


        private SkinnedMeshRenderer skinnedMeshRenderer;
        //[SerializeField]
        //private List<CharacterSkinObject> skinObjects = new List<CharacterSkinObject>();
        //private Dictionary<string, CharacterSkinObject> skinObjectsLookup ;





        public CharacterSkinData SkinData{
            get { return skinData; }
        }





        private void Initialize()
        {
            if (GetComponent<Animator>() == null)
                if (GetComponentInParent<Animator>() == null)
                    return;

            if(hairObject == null)
                hairObject = CreateSkinObject(SkinIDs.Hair, GetTransform(HumanBodyBones.Head));
            if(hatHairObject == null)
                hatHairObject = CreateSkinObject(SkinIDs.HatHair, GetTransform(HumanBodyBones.Head));
        }


        private bool HasRequiredData()
        {
            if (skinData == null){
                Debug.LogFormat("SkinData is missing.");
                return false;
            }

            if (skinData.CharacterMesh == null || skinData.SkinTextureSet == null){
                Debug.LogFormat("CharacterSkinData has missing data.");
                return false;
            }

            return true;
        }


        public void LoadCharacterSkin()
        {
            Initialize();

            if (HasRequiredData() == false)
                return;
            
            Material skinMaterial = skinOverrideMaterial != null ? skinOverrideMaterial : skinData.SkinMaterial;

            //  Setup skin mesh
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null){
                skinnedMeshRenderer.sharedMesh = skinData.CharacterMesh;
                skinnedMeshRenderer.sharedMaterial = skinMaterial;
            }

            //  Update hair
            UpdateMesh(hairObject, skinData.HairMesh, skinMaterial);
            //  Update hathair
            UpdateMesh(hatHairObject, skinData.HatHairMesh, skinMaterial);
            //  Update facialhair

            hairObject.gameObject.SetActive(!enableHat);
            hatHairObject.gameObject.SetActive(enableHat);
        }


        public void LoadCharacter()
        {
            LoadCharacterSkin();

            //  Create eyes.
            if (eyes != null)
                eyesObject = Instantiate(eyes, GetTransform(HumanBodyBones.Head)) as CharacterSkinObject;
        }




        private void UpdateMesh(CharacterSkinObject skinObject, Mesh mesh, Material material)
        {
            MeshFilter meshFilter = skinObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = skinObject.GetComponent<MeshRenderer>();

            if(mesh != null){
                meshFilter.mesh = mesh;
                skinObject.gameObject.SetActive(true);
                meshRenderer.material = material;
            }
            else{
                meshFilter.mesh = null;
                skinObject.gameObject.SetActive(false);
                meshRenderer.material = null;
            }


        }











        private CharacterSkinObject CreateSkinObject(string skinObjectName, Transform parent = null)
        {
            GameObject go = new GameObject(skinObjectName, typeof(MeshFilter), typeof(MeshRenderer), typeof(CharacterSkinObject));
            go.transform.parent = parent == null ? transform : parent;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;

            CharacterSkinObject skinObject = go.GetComponent<CharacterSkinObject>();
            //skinObjects.Add(skinObject);
            return skinObject;
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



        //private void UnloadCharacterSkin()
        //{
        //    for (int i = 0; i < skinObjects.Count; i++)
        //    {
        //        if (skinObjects[i] != null)
        //            DestroyImmediate(skinObjects[i].gameObject, true);
        //    }
        //    skinObjects.Clear();
        //    skinObjectsLookup.Clear();
        //}

        //private void Reload()
        //{
        //    if(skinObjectsLookup == null)
        //        skinObjectsLookup = new Dictionary<string, CharacterSkinObject>();

        //    for(int i = 0; i < skinObjects.Count; i ++){
        //        if(skinObjects[i] == null){
        //            skinObjects.RemoveAt(i);
        //            continue;
        //        }
        //        if(skinObjectsLookup.ContainsKey(skinObjects[i].skinId) == false){
        //            skinObjectsLookup.Add(skinObjects[i].skinId, skinObjects[i]);
        //        }
        //    }
        //}
    }
}
