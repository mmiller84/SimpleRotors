//
// Copyright (c) Brian Hernandez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;
using System;

public class Helicopter : AircraftBase
{
    public bool EngineOn = false;

	public WeaponDropper[] weapons;

	public override Rigidbody Rigidbody { get; internal set; }

    private Rigidbody _hub;
    private HingeJoint _hinge;

    public int Rpm
    {
        get
        {
            var angVel = _hub.transform.InverseTransformVector(_hub.angularVelocity);
            return Mathf.Abs(Mathf.FloorToInt(angVel.y * 9.5493f));
        }
    }

    private float _collective = 0f;
    public float Collective => _collective;

	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
        _hinge = GetComponent<HingeJoint>();
        _hub = _hinge.connectedBody;
    }

	private void Start()
	{
		try
		{
			Input.GetAxis("Yaw");
		}
		catch (ArgumentException e)
		{
			Debug.LogWarning(e);
			Debug.LogWarning(name + ": \"Yaw\" axis not defined in Input Manager. Tail rotor will not work correctly!");
		}
    }

	void Update()
	{
        if(Input.GetButtonDown("Jump"))
        {
            EngineOn = !EngineOn;
        }

		_collective = (Input.GetAxis("Collective") + 1f) / 2f;

		if (weapons.Length > 0)
		{
			if (Input.GetButtonDown("Fire3"))
			{
				foreach (WeaponDropper dropper in weapons)
				{
					dropper.Fire(Rigidbody.GetPointVelocity(dropper.transform.position));
				}
			}
		}

        _hinge.useMotor = EngineOn;
    }

    private float CalculatePitchG()
	{
		// Angular velocity is in radians per second.
		Vector3 localVelocity = transform.InverseTransformDirection(Rigidbody.velocity);
		Vector3 localAngularVel = transform.InverseTransformDirection(Rigidbody.angularVelocity);

		// Local pitch velocity (X) is positive when pitching down.

		// Radius of turn = velocity / angular velocity
		float radius = (Mathf.Approximately(localAngularVel.x, 0.0f)) ? float.MaxValue : localVelocity.z / localAngularVel.x;

		// The radius of the turn will be negative when in a pitching down turn.

		// Force is mass * radius * angular velocity^2
		float verticalForce = (Mathf.Approximately(radius, 0.0f)) ? 0.0f : (localVelocity.z * localVelocity.z) / radius;

		// Express in G (Always relative to Earth G)
		float verticalG = verticalForce / -9.81f;

		// Add the planet's gravity in. When the up is facing directly up, then the full
		// force of gravity will be felt in the vertical.
		verticalG += transform.up.y * (Physics.gravity.y / -9.81f);

		return verticalG;
	}

    private void OnGUI()
	{
		const float msToKnots = 1.94384f;
		GUI.Label(new Rect(10, 40, 300, 20), string.Format("Speed: {0:0.0} knots", Rigidbody.velocity.magnitude * msToKnots));
		GUI.Label(new Rect(10, 60, 300, 20), string.Format("Collective: {0:0.0}%", _collective * 100.0f));
		GUI.Label(new Rect(10, 80, 300, 20), string.Format("G Load: {0:0.0} G", CalculatePitchG()));
        GUI.Label(new Rect(10, 100, 300, 20), string.Format("Rotor RPM: {0}", Rpm));
    }
}
