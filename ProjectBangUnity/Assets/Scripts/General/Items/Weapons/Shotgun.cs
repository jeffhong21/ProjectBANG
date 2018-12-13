namespace Bang
{
    using System.Collections;
    using UnityEngine;
    using SharedExtensions;

    public class Shotgun : ShootableWeapon
    {

        [Header("Shotgun Properties")]
        public int projectilesPerShot = 3;
        public float angleSpread = 8;




        public override void Shoot()
        {
            if (Time.timeSinceLevelLoad > lastShootTime)
            {
                if (currentAmmo > 0 && canShoot)
                {

                    Vector3[] dirs = transform.forward.GetDirectionsFrom(projectilesPerShot, angleSpread);
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, dirs[i]);
                        //  Spawn Projectile from the PooManager.
                        IPooled p = PoolManager.instance.Spawn(PoolTypes.Projectile, projectileSpawn.position, rotation);
                        Projectile pooledProjectile = p.gameObject.GetComponent<Projectile>();
                        //  Initialize the projectile.
                        pooledProjectile.Initialize(owner, damage, power, range);

                    }


                    //  Play Particle Shoot Effects
                    if (particles != null){
                        for (int i = 0; i < particles.Length; i++){
                            particles[i].Play();
                        }
                    }


                    //  Update Current Ammo;
                    currentAmmo--;
                    //  Stamp last shot time.
                    lastShootTime = Time.timeSinceLevelLoad + firerate;
                }
            }
        }




        protected override IEnumerator PlayReloadAnim(float time)
        {
            yield return new WaitForSeconds(time);
        }


        protected override IEnumerator PlayShootAnim(float time)
        {
            throw new System.NotImplementedException();
        }
    }
}


