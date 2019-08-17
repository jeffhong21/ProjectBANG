using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour
{
    protected enum RotateObjectUpdateMode { FixedUpdate, Update, LateUpdate }

    [SerializeField]
    protected RotateObjectUpdateMode updateMode = RotateObjectUpdateMode.FixedUpdate;

    [Tooltip("Rotate around this point in local space.")]
    public Vector3 pivot;

    public bool rotateX = false;
    public bool rotateY = true;
    public bool rotateZ = false;

    public float rotationSpeed;
    public bool isMoving;


    Vector3 rotationVector;
    Vector3 rotationEulerVector;
    protected float deltaTime;
    protected Transform mTransform;


    private void Start()
    {
        deltaTime = Time.fixedDeltaTime;
        mTransform = transform;

        var rot = mTransform.rotation.eulerAngles;

        rotationEulerVector = new Vector3(rot.x % 360f, rot.y % 360f, rot.z % 360f);
        rotationVector = new Vector3(rotateX ? 1f : 0f, rotateY ? 1f : 0f, rotateZ ? 1f : 0f);


    }


    private void FixedUpdate()
    {
        
        if (isMoving)
        {
            transform.position += (transform.rotation * pivot);

            rotationVector.x = rotateX ? 1f : 0f;
            rotationVector.y = rotateY ? 1f : 0f;
            rotationVector.z = rotateZ ? 1f : 0f;

            mTransform.rotation *= Quaternion.AngleAxis(rotationSpeed * deltaTime, rotationVector);

            mTransform.position -= (mTransform.rotation * pivot);

            rotationEulerVector.Set(
                mTransform.rotation.eulerAngles.x % 360f,
                mTransform.rotation.eulerAngles.x % 360f,
                mTransform.rotation.eulerAngles.x % 360f
                );

            mTransform.rotation = Quaternion.Euler(rotationEulerVector);
        }
    }


}
