using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Teleport : MonoBehaviour
{

    public Transform destination;

    private Color baseColor = new Color(0, 0.75f, 0, 0.75f);
    private Color destinationColor = new Color(0.75f, 0, 0, 0.75f);

    private BoxCollider triggerCollider;



    private void Start()
    {
        triggerCollider = GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destination != null)
        {
            other.transform.position = destination.position + Vector3.up * 0.2f;
            if(other.attachedRigidbody != null)
                other.attachedRigidbody.velocity = Vector3.zero;
        }
    }



    private void OnDrawGizmos()
    {
        if(triggerCollider == null) triggerCollider = GetComponent<BoxCollider>();

        Gizmos.color = baseColor;
        Gizmos.DrawCube(transform.TransformPoint(triggerCollider.center), triggerCollider.size);
        if(destination != null)
        {
            Gizmos.color = destinationColor;
            Gizmos.DrawCube(destination.TransformPoint(triggerCollider.center), triggerCollider.size);
            UnityEditor.Handles.DrawDottedLine(transform.position, destination.position, 4);
        }

    }
}
