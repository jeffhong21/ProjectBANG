using UnityEngine;
using System.Collections;
using System.Linq;

public class RaycastHelper : MonoBehaviour
{

    public float _Radius = 1;
    public float _Height = 0;
    public LayerMask _LayerMask = -1;


    public bool _OptimizeCast = true;
    public bool _ShowOptimizedStart = false;

    public bool _ShowCastStart = true;
    public bool _ShowCastEnd = true;

    public bool _ShowDistances = true;
    public bool _ShowHitCentre = false;
    public bool _ShowHitCapsule = false;

    public bool _ShowHitPoints = true;
    public bool _ShowHitNormals = true;


    void OnDrawGizmos()
    {
        Transform handleA = transform.GetChild(0);
        Transform handleB = transform.GetChild(1);

        if (handleA == null || handleB == null)
            return;

        float distance = Vector3.Distance(handleB.position, handleA.position);
        Vector3 direction = Vector3.Normalize(handleB.position - handleA.position);
        Vector3 capA = handleA.position;
        Vector3 capB = handleA.position + Vector3.up * _Height;

        RaycastHit[] hits = Physics.CapsuleCastAll(capA, capB, _Radius, direction, distance, _LayerMask);
        hits = hits.Where(x => x.transform != handleA && x.transform != handleB).ToArray();

        if (_OptimizeCast)
        {
            hits = OptimizedCast.CapsuleCastAll(capA, capB, _Radius, direction, distance, _LayerMask);
            hits = hits.Where(x => x.transform != handleA && x.transform != handleB).ToArray();
        }

        foreach (RaycastHit hit in hits)
        {
            if (_ShowHitPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(hit.point, 0.1f);
            }

            if (_ShowHitNormals)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(hit.point, hit.point + hit.normal);
            }

            if (_ShowDistances)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(capA, capA + direction * hit.distance);

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(capB, capB + direction * hit.distance);
            }

            if (_ShowHitCentre)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(capA + direction * hit.distance, capB + direction * hit.distance);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(capA + direction * hit.distance, 0.1f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(capB + direction * hit.distance, 0.1f);
            }

            if (_ShowHitCapsule)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(capA + direction * hit.distance, _Radius);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(capB + direction * hit.distance, _Radius);
            }
        }

        if (_ShowCastStart)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(handleA.position, _Radius);
            Gizmos.DrawWireSphere(handleA.position + Vector3.up * _Height, _Radius);
        }

        if (_ShowCastEnd)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(handleB.position, _Radius);
            Gizmos.DrawWireSphere(handleB.position + Vector3.up * _Height, _Radius);
        }

        if (_OptimizeCast && _ShowOptimizedStart)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(capA - direction * _Radius, _Radius);
            Gizmos.DrawWireSphere(capB - direction * _Radius, _Radius);
            Gizmos.DrawLine(capA - direction * _Radius, capA);
            Gizmos.DrawLine(capB - direction * _Radius, capB);
        }
    }
}
