using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSkins
{
    
    public class ActorSkinComponent : MonoBehaviour
    {
        private readonly string holderName = "Accessories";

        public ActorSkin skin;
        public bool randomSkin;
        public AccessorySlots slots = new AccessorySlots();

        [SerializeField]
        private Animator anim;
        [SerializeField]
        private SkinnedMeshRenderer meshRenderer;
        private List<GameObject> instancedObj;
        private GameObject holder;




        public SkinnedMeshRenderer MeshRenderer{
            get { return meshRenderer; }
        }



        private void OnValidate(){
            LoadActorSkin();
        }


        private void Initialize()
        {
            anim = GetComponent<Animator>();

            if(holder == null){
                holder = new GameObject();
                holder.name = holderName;
                holder.transform.parent = this.transform;
                holder.transform.localPosition = Vector3.zero;
                holder.transform.localEulerAngles = Vector3.zero;
                holder.transform.localScale = Vector3.one;
            }

            if(instancedObj == null) instancedObj = new List<GameObject>();
        }


        public void LoadActorSkin()
        {
            if(meshRenderer == null || skin == null){
                //Debug.Log("** No SkinnedMeshRenderer loaded. **");
                return;
            }

            //  Use a random skin.
            if(randomSkin){
                meshRenderer.sharedMesh = skin.GetRandomSkin().sharedMesh;
                //meshRenderer.sharedMaterial.SetTexture("_MainTex", characterSkinTexture[textureType]);
                //meshRenderer.sharedMaterial.mainTexture = characterSkinTexture[textureType];
            }
            //  Use Mesh Provided from ActorSkin.
            else{
                meshRenderer.sharedMesh = skin.mesh.sharedMesh;
                //meshRenderer.sharedMaterial.SetTexture("_MainTex", characterSkinTexture[textureType]);
                //meshRenderer.sharedMaterial.mainTexture = characterSkinTexture[textureType];
            }

        }


        public void LoadAccessories()
        {
            Initialize();
            //LoadAccessories(neck, AccessoryBones.Neck, Vector3.zero);
            LoadNeckAccessories();
            LoadLeftHipAccessory();
            LoadRightHipAccessory();
        }




        private void LoadNeckAccessories()
        {
            GameObject go = LoadAccessories(slots.neck, AccessoryBones.Neck);
        }

        private void LoadLeftHipAccessory()
        {
            float distance = 0.25f;
            Vector3 position = -transform.right * distance;
            Vector3 rotation = new Vector3(0, -90, 0); ;
            GameObject go = LoadAccessories(slots.leftHip, AccessoryBones.Hips, position, rotation);
        }


        private void LoadRightHipAccessory()
        {
            float distance = 0.25f;
            Vector3 position = transform.right * distance;
            Vector3 rotation = new Vector3(0, 90, 0); ;
            GameObject go = LoadAccessories(slots.rightHip, AccessoryBones.Hips, position, rotation);
        }



        private GameObject LoadAccessories(Accessories a, AccessoryBones bone, Vector3? position = null, Vector3? rotation = null)
        {
            GameObject go = null;
            if (a != null)
            {
                go = a.Initialize();
                go.transform.parent = GetBone(bone);
                go.transform.localPosition = Vector3.zero;
                go.transform.parent = holder.transform;

                if(position != null){
                    go.transform.localPosition += (Vector3)position;
                }
                if (rotation != null){
                    go.transform.localEulerAngles = (Vector3)rotation;
                }
            }
            return go;
        }


        private void RemoveAccessories()
        {
            int childs = holder.transform.childCount;
            for (int i = childs - 1; i > 0; i--)
            {
                Destroy(holder.transform.GetChild(i).gameObject);
            }
        }


        //private void RegisterAccessory(string key, GameObject value)
        //{
        //    Debug.Log("Registering " + accessories[key]);
        //    if(accessories[key] != null){
        //        Debug.Log("Destroying " + accessories[key]);
        //        Destroy(accessories[key]);
        //    }
        //    accessories[key] = value;
        //}





        private Transform GetBone(AccessoryBones bone)
        {
            switch (bone)
            {
                case AccessoryBones.Head:
                    return anim.GetBoneTransform(HumanBodyBones.Head);

                case AccessoryBones.Neck:
                    return anim.GetBoneTransform(HumanBodyBones.Neck);

                case AccessoryBones.UpperChest:
                    return anim.GetBoneTransform(HumanBodyBones.UpperChest);

                case AccessoryBones.Chest:
                    return anim.GetBoneTransform(HumanBodyBones.Chest);

                case AccessoryBones.Hips:
                    return anim.GetBoneTransform(HumanBodyBones.Hips);

                case AccessoryBones.RightHand:
                    return anim.GetBoneTransform(HumanBodyBones.RightHand);

                case AccessoryBones.LeftHand:
                    return anim.GetBoneTransform(HumanBodyBones.LeftHand);

                default:
                    return null;
            }

        }



        private void UpdateIK(AvatarIKGoal goal, Transform target, float w)
        {
            if (target != null)
            {
                anim.SetIKPositionWeight(goal, w);
                anim.SetIKRotationWeight(goal, w);
                anim.SetIKPosition(goal, target.position);
                anim.SetIKRotation(goal, target.rotation);
            }
        }

        [System.Serializable]
        public class AccessorySlots
        {
            public Accessories head;

            public Accessories neck;

            public Accessories frontChest;

            public Accessories backChest;

            public Accessories leftHip;

            public Accessories rightHip;

        }


        private class Keys
        {
            private readonly string head = "Head";
            private readonly string neck = "Neck";
            private readonly string leftHip = "LeftHip";
            private readonly string rightHip = "RightHip";

            public string Head
            {
                get { return head; }
            }

            public string Neck
            {
                get { return neck; }
            }

            public string LeftHip
            {
                get { return leftHip; }
            }

            public string RightHip
            {
                get { return rightHip; }
            }
        }
	}




    public enum AccessoryBones
    {
        Head,
        Neck,
        UpperChest,
        Chest,
        Hips,
        RightHand,
        LeftHand
    }

    public enum AccessoryLocations
    {
        Head,
        Neck,
        FrontChest,
        BackChest,
        LeftHip,
        RightHip,
        FrontHip,
        BackHip
    }
}
