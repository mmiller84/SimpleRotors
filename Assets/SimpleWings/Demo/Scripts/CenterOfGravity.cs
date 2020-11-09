//
// Copyright (c) Mike Miller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;

public class CenterOfGravity : MonoBehaviour
{
    void Start()
    {
        var rb = GetComponentInParent<Rigidbody>();
        rb.centerOfMass = rb.transform.InverseTransformPoint(transform.position);
    }
}
