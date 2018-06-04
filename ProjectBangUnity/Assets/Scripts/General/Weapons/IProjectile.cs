namespace Bang
{
    using UnityEngine;

    public interface IProjectile : IPooled
    {

        float Damage { get; }

        float LifeDuration { get; }

        Vector3 SpawnLocation { get; }

        float MaxRange { get; }

        float Velocity { get; }

    }

}