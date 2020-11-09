//
// Copyright (c) Mike Miller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotorAssembly : MonoBehaviour
{
    public Helicopter Heli;
    public float BladeLength = 5;
    public float MaxBladePitch = 10f;
    public float MinBladePitch = -5f;
    public float CyclicMaxPitch = 5f;

    private float _collective;
    private float _rollInput;
    private float _pitchInput;

    private Rigidbody _rb;
    private Quaternion[] _initialRotations;
    private FixedJoint[] _rotors;

    private void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = Mathf.Infinity;

        _rotors = GetComponent<HingeJoint>().connectedBody.GetComponents<FixedJoint>();
        _initialRotations = new Quaternion[_rotors.Length];

        for(int i=0; i<_rotors.Length; i++)
        {
            _rotors[i].connectedBody.maxAngularVelocity = Mathf.Infinity;
            _initialRotations[i] = _rotors[i].connectedBody.transform.localRotation;
        }
    }

    private void Update()
    {
        _collective = Heli.Collective;
        _rollInput = Input.GetAxis("Horizontal");
        _pitchInput = Input.GetAxis("Vertical") * -1f;
    }

    private void FixedUpdate()
    {
        var relwind = -Heli.Rigidbody.velocity;

        for (int i=0; i<_rotors.Length; i++)
        {
            var holder = _rotors[i].connectedBody.transform;
            var rotor = holder.GetChild(0);
            var rvel = _rb.GetPointVelocity(rotor.transform.position) + relwind;
            var angle = Vector3.SignedAngle(holder.forward, Heli.transform.forward, Heli.transform.up);
            //var dot = Vector3.Dot(relwind.normalized, rvel.normalized); //use the dot product to simulate the effects of blade flapping (+1 == retreating blade; -1 == advancing blade)

            var rollCyclic = Mathf.Sin(angle * Mathf.Deg2Rad) * _rollInput;
            var pitchCyclic = Mathf.Cos(angle * Mathf.Deg2Rad) * _pitchInput;

            var cyclic = Mathf.Clamp(rollCyclic + pitchCyclic, -1f, 1f);

            var bladePitch = Mathf.Lerp(MinBladePitch, MaxBladePitch, _collective);
            bladePitch += cyclic * CyclicMaxPitch;

            rotor.transform.localRotation = Quaternion.AngleAxis(bladePitch, Vector3.left);
        }
    }
}
