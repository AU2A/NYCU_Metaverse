using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SteeringWheel : NetworkPositionRotation
{
    [SerializeField] private Transform car;

    [Networked] private float wheelAngle { get; set; }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data) && HasStateAuthority)
        {
            wheelAngle = data.wheelAngle;
        }

        transform.position = Vector3.Lerp(
            transform.position, car.position +
            new Vector3(Mathf.Sin(car.rotation.eulerAngles.y * Mathf.Deg2Rad) * 0.23f,
                        0.34f,
                        Mathf.Cos(car.rotation.eulerAngles.y * Mathf.Deg2Rad) * 0.23f)
                        , Runner.DeltaTime);
        transform.rotation = Quaternion.Euler(0, car.rotation.eulerAngles.y, wheelAngle);
    }
}
