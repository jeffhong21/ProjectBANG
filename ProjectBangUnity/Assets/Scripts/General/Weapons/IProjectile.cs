namespace Bang
{
    using UnityEngine;

    public interface IProjectile : IPooled
    {

        float damage { get; set; }

        //float lifeDuration { get; }

        Vector3 spawnLocation { get; }

        //float maxRange { get; }

        float velocity { get; set; }

    }

}