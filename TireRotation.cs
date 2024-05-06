using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TireRotation : NetworkPositionRotation
{
    [Networked] private float moveVelocity { get; set; }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data) && Object.HasStateAuthority)
        {
            moveVelocity = data.moveVelocity;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(moveVelocity * 10, 0, 0), 0.1f);
        // transform.Rotate(moveVelocity * 5, 0, 0);
    }
}
