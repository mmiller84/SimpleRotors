using UnityEngine;

public class TailRotor : MonoBehaviour
{
    public float ThrustMultiplier = 0f;
    public float MaxAntiTorqueForce = 200f;

    private Helicopter _heli;
    private Rigidbody _rb;

    private float _yawInput = 0f;

    private void Start()
    {
        _heli = GetComponentInParent<Helicopter>();
        _rb = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        _yawInput = Input.GetAxis("Yaw") * -1f;
    }

    private void FixedUpdate()
    {
        var pedalForce = MaxAntiTorqueForce * _yawInput;
        _rb.AddRelativeTorque(Vector3.up * (_heli.Rpm * ThrustMultiplier + pedalForce));
    }
}
