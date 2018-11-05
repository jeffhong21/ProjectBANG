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

        private PlayerInputHandler input;



        public event Action<int, int> CurrentAmmoEvent;

        public event Action<float> DamageEvent;

        public event Action<string> EquipWeaponEvent;

        //
        //  Methods
        //
        protected override void Awake()
        {
            base.Awake();
            input = GetComponent<PlayerInputHandler>();

            //crosshairs = Instantiate(crosshairs, this.transform.position, crosshairs.transform.rotation, this.transform);
        }


        protected override void ExecuteUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Break();
            }
        }

        protected override void ExecuteFixedUpdate(float deltaTime)
        {

        }

        protected override void OnEquipWeapon(Gun weapon){
            
            CurrentAmmoEvent(weapon.CurrentAmmo, weapon.MaxAmmo);
            EquipWeaponEvent(weapon.NameID);
        }


        protected override void OnShootWeapon(){
            weapon.Shoot();
            CurrentAmmoEvent(weapon.CurrentAmmo, weapon.MaxAmmo);
        }


        protected override void OnReload(){
            CurrentAmmoEvent(weapon.CurrentAmmo, weapon.MaxAmmo);
        }


        protected override void OnTakeDamage(Vector3 hitDirection)
        {
            AnimHandler.PlayTakeDamage(hitDirection);
            DamageEvent(Health.CurrentHealth);
        }


        protected override void OnDeath()
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





        public void EnterCover()
        {
            CoverObject cover = FindClosestCover();
            //EnterCover(cover);
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

        public bool CanEmergeFromCover(Transform helper, bool right)
        {
            float entitySize = 0.5f;
            float distOffset = entitySize * 0.5f;
            Vector3 origin = transform.position;
            Vector3 side = (right == true) ? transform.right : -transform.right;
            //side.y = origin.y;
            Vector3 direction = side - origin;
            Vector3 helpPosition = side + (direction.normalized * 0.025f);
            helpPosition.y = 1f;
            helper.localPosition = helpPosition;
            Vector3 outDir = (-helper.transform.forward) + helper.position;


            float scanDistance = (outDir - helper.position).magnitude;
            RaycastHit hit;

            if (Physics.Raycast(helper.position, outDir, out hit, scanDistance, Layers.cover))
            {
                Debug.DrawLine(helper.position, outDir, Color.red, 1f);
                Debug.Log(helper.name + " hit " + hit.transform.name);
                return false;
            }

            Debug.DrawLine(helper.position, outDir, Color.green, 1f);
            return true;
        }





        //public void OnDrawGizmosSelected(){
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(AimOrigin, _shootTarget);
        //}

	}
}


