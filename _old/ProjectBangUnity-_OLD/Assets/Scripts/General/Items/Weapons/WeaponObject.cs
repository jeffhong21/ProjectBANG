using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bang
{
    [CreateAssetMenu(fileName = "ShootableWeapon", menuName = "Items/Shootable Weapons")]
    public class WeaponObject : ScriptableObject
    {
        public string nameID;

        public ShootableWeapon prefab;

        public WeaponTypes weaponType;

        public bool autoReload;

        public int maxAmmo = 6;

        public float damage = 1f;

        public float firerate = 0.5f;

        public float power = 0.5f;

        public float range = 20f;

        public float spread;

        public Projectile projectile;

        public IKPositions mainHandIK;

    }




}




