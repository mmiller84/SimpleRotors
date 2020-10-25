using UnityEngine;

public abstract class AircraftBase : MonoBehaviour
{
    public abstract Rigidbody Rigidbody { get; internal set; }
}
