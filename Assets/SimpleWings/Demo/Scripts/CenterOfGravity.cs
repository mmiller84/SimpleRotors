using UnityEngine;

public class CenterOfGravity : MonoBehaviour
{
    void Start()
    {
        var rb = GetComponentInParent<Rigidbody>();
        rb.centerOfMass = rb.transform.InverseTransformPoint(transform.position);
    }
}
