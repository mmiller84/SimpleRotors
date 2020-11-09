//
// Copyright (c) Mike Miller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;

public abstract class AircraftBase : MonoBehaviour
{
    public abstract Rigidbody Rigidbody { get; internal set; }
}
