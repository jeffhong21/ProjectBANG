namespace CharacterController
{
    using UnityEngine;


    public class ShootableWeapon : Weapon
    {
        //
        // Fields
        //
        protected Transform m_FirePoint;
        protected float m_FireRate = 2;                 //  The number of shots per second
        protected float m_FireCount = 1;
        protected int m_ClipSize = 4;

        protected bool m_AutoReload;
        protected float m_RecoilAmount = 0.1f;
        protected float m_Spread = 0.01f;

        protected string m_IdleAnimationStateName;
        protected string m_FireAnimationStateName;
        protected string m_ReloadAnimationStateName;

        protected GameObject m_Projectile;





		//
		// Methods
		//
		public override void Awake()
		{
            base.Awake();
		}

		public override void Initialize(Inventory inventory)
        {
            m_Inventory = inventory;
        }


        public override bool TryUse()
        {

            return true;
        }


        public override bool CanUse()
        {
            return true;
        }


        public override bool InUse()
        {
            return false;
        }

        public override void TryStopUse()
        {

        }

        public bool TryStartReloading()
        {
            return true;
        }


        public bool IsReloading()
        {
            return true;
        }


        public bool TryStopReloading()
        {
            return true;
        }




		protected override void ItemActivated()
		{

		}

        protected virtual void Fire()
        {

        }

        protected virtual void ProjectileFire()
        {

        }

        protected virtual void HitscanFire()
        {

        }


        protected override void OnStartAim()
        {

        }


        protected override void OnAim(bool aim)
        {

        }
	}

}