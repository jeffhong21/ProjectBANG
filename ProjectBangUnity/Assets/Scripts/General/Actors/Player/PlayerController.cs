namespace Bang
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;


    public class PlayerController : ActorController
    {
        //
        //  Fields
        //
        public PlayerStats stats;

        public PlayerCrosshairs crosshairs;

        public Camera playerCamera;

        private PlayerInputHandler input;



        //
        //  Methods
        //
        protected override void Awake()
        {
            base.Awake();

            input = GetComponent<PlayerInputHandler>();

            playerCamera = GameManager.instance.PlayerCamera.GetComponent<Camera>();
            GameManager.instance.PlayerCamera.target = this.transform;
            //playerCamera.GetComponent<CameraController>().target = this.transform;

            crosshairs = Instantiate(crosshairs, this.transform.position, crosshairs.transform.rotation, this.transform);
        }


        protected override void OnEnable()
		{
            base.OnEnable();
		}


        protected override void OnDisable()
		{
            base.OnDisable();
		}


        public void ShootWeapon(Vector3 target)
        {
            weapon.Shoot(target);
        }


        public void Reload()
        {
            // TODO    
        }


        public void InitiateCover(CoverObject target)
        {
            
        }


        public void EnterCover()
        {
            bool inCover = false;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3, Layers.cover);
            float mDist = float.MaxValue;
            CoverObject closestCover = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                var col = colliders[i];
                if (col == null || col.gameObject == gameObject) {
                    continue;
                }

                if (col.GetComponent<CoverObject>()){
                    float tDist = Vector3.Distance(colliders[i].transform.position, position);
                    if (tDist < mDist){
                        mDist = tDist;
                        closestCover = colliders[i].GetComponent<CoverObject>();
                    }
                }
            }

            if(closestCover != null){
                inCover = closestCover.TakeCoverSpot(this.gameObject);
                if(inCover) Debug.Log("In Cover");
            }

        }


        public override void Death()
        {
            AnimHandler.Death();
            DisableControls();
        }


        public override void EnableControls()
        {
            input.enabled = true;
        }

        public override void DisableControls()
        {
            input.enabled = false;
        }

    }
}


