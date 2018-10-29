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



        protected override void ExecuteUpdate(float deltaTime)
        {

        }

        protected override void ExecuteFixedUpdate(float deltaTime)
        {

        }


        protected override void OnShootWeapon(){
            weapon.Shoot();
        }

        protected override void OnTakeDamage(Vector3 hitDirection)
        {
            AnimHandler.PlayTakeDamage(hitDirection);
        }


        public void EnterCover()
        {
            CoverObject cover = FindClosestCover();
            EnterCover(cover);
        }


        private CoverObject FindClosestCover()
        {
            CoverObject closestCover = null;
            float mDist = float.MaxValue;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2, Layers.cover);

            for (int i = 0; i < colliders.Length; i++)
            {
                var col = colliders[i];
                if (col == null || col.gameObject == gameObject)
                {
                    continue;
                }

                if (col.GetComponent<CoverObject>())
                {
                    float tDist = Vector3.Distance(colliders[i].transform.position, position);
                    if (tDist < mDist)
                    {
                        mDist = tDist;
                        closestCover = colliders[i].GetComponent<CoverObject>();
                    }
                }
            }
            return closestCover;
        }


		public void CheckIfCanEmerge()
		{
            if(CanEmergeFromCover(rightHelper, true)){
                Debug.Log("Can emerge from right.");
            }
            else if(CanEmergeFromCover(leftHelper, false)){
                Debug.Log("Can emerge from left.");
            }
            else{
                Debug.Log("Cannot emerge");
            }
            //Debug.Break();
		}


		public override void OnDeath()
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



        //public void OnDrawGizmosSelected(){
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(AimOrigin, _shootTarget);
        //}

	}
}


