using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TireRotationPivot : NetworkPositionRotation
{
    [SerializeField] private Transform axle;

    [SerializeField] float offset;

    [Networked] private float wheelAngle { get; set; }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data) && Object.HasStateAuthority)
        {
            wheelAngle = data.wheelAngle;
        }
        // transform.rotation = Quaternion.Euler(0, -data.wheelAngle / 10, 0);
        transform.position = Vector3.Lerp(
            transform.position, axle.position +
            new Vector3(Mathf.Cos(-axle.rotation.eulerAngles.y * Mathf.Deg2Rad) * offset,
                        0f,
                        Mathf.Sin(-axle.rotation.eulerAngles.y * Mathf.Deg2Rad) * offset),
                        Runner.DeltaTime);
        transform.rotation = Quaternion.Euler(0, axle.rotation.eulerAngles.y - wheelAngle / 10, 0);
    }
}
