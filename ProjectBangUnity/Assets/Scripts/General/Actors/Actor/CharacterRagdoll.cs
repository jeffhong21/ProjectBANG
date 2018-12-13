namespace Bang
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;


    public class CharacterRagdoll : MonoBehaviour
    {
        public Rigidbody rigid;
        public Animator anim;
        public GameObject activeModel;

        List<Collider> ragdollColliders = new List<Collider>();
        List<Rigidbody> ragdollRigids = new List<Rigidbody>();


		private void Awake()
		{
            rigid = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            activeModel = gameObject;

            SetupRagdoll();
		}


		private void SetupRagdoll()
        {
            Rigidbody[] rigids = activeModel.GetComponentsInChildren<Rigidbody>();

            for (int i = 0; i < rigids.Length; i++)
            {
                if (rigids[i] == rigid)
                {
                    continue;
                }

                Collider col = rigids[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                ragdollRigids.Add(rigids[i]);
                ragdollColliders.Add(col);
                rigids[i].isKinematic = true;
                //r.gameObject.layer = 10;
            }

        }

        public void EnableRagdoll(float t){
            StartCoroutine(EnableRagdoll_AfterDelay(t));
        }


        IEnumerator EnableRagdoll_AfterDelay(float t){
            yield return new WaitForSeconds(t);
            EnableRagdoll_Actual();

            yield return new WaitForEndOfFrame();
            anim.enabled = false; //  this will stop ragdolls from exploding.
        }


        void EnableRagdoll_Actual()
        {
            for (int i = 0; i < ragdollColliders.Count; i++)
            {

                ragdollColliders[i].isTrigger = false;
                ragdollRigids[i].isKinematic = false;
            }
        }
    }
}