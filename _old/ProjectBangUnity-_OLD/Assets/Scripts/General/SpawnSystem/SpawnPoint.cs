using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bang
{
    
    public class SpawnPoint : MonoBehaviour
    {
        public int grouping;
        public SpawnShape shape;

        public float radius = 1f;
        public float xLength = 1f;
        public float zLength = 1f;
        public Color gizmoColor = new Color(1f, 0f, 0f, 0.2f);

        private Vector3 boxSize = Vector3.one;


		private void Awake()
		{
            gameObject.tag = "RespawnPoint";
		}


		private void OnEnable()
		{
            boxSize = new Vector3(xLength, 1, zLength);
		}


        public Vector3 GetSpawnPoint()
        {
            Vector3 spawnPoint = transform.position;
            switch (shape)
            {
                case SpawnShape.Point:
                    spawnPoint = transform.position;
                    break;
                case SpawnShape.Sphere:
                    spawnPoint = transform.position + (Random.insideUnitSphere * radius);
                    spawnPoint.y = 0;
                    break;
                case SpawnShape.Box:
                    float randomX = Random.Range(transform.position.x - xLength, transform.position.x + xLength);
                    float randomZ = Random.Range(transform.position.z - zLength, transform.position.z + zLength);
                    Vector3 randomRange = new Vector3(randomX, 0, randomZ);

                    spawnPoint = transform.position + randomRange;
                    break;
            }

            return spawnPoint;
        }


		private void OnDrawGizmosSelected()
		{
            Gizmos.color = gizmoColor;
            switch (shape)
            {
                case SpawnShape.Point:
                    Gizmos.DrawSphere(transform.position, 0.25f);
                    break;
                case SpawnShape.Sphere:
                    Gizmos.DrawSphere(transform.position, radius);
                    break;
                case SpawnShape.Box:
                    boxSize.Set(xLength, 1, zLength);
                    Gizmos.DrawCube(transform.position + Vector3.up * (boxSize.y * 0.5f), boxSize);
                    break;
            }    
		}

	}

    public enum SpawnShape
    {
        Point,
        Sphere,
        Box
    }

}
