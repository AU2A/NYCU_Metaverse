using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class VrRightHandController : NetworkPositionRotation
{
    [SerializeField] private Transform car;
    [SerializeField] private Transform rightWheelPoint;
    [SerializeField] private Vector3 tracking_pos_offset;
    [SerializeField] private Vector3 tracking_rot_offset;

    [Networked] private Vector3 target_pos { get; set; }
    [Networked] private Quaternion target_rot { get; set; }

    public override void FixedUpdateNetwork()
    {
        var carRotationSin = Mathf.Sin(car.rotation.eulerAngles.y * Mathf.Deg2Rad);
        var carRotationCos = Mathf.Cos(car.rotation.eulerAngles.y * Mathf.Deg2Rad);

        if (GetInput(out InputData data) && HasStateAuthority)
        {
            if (data.rightControllerGrip == 1)
            {
                target_pos = rightWheelPoint.position;
            }
            else
            {
                target_pos = data.rightHandPosition + new Vector3(
                    carRotationSin * (tracking_pos_offset.x + tracking_pos_offset.z),
                    tracking_pos_offset.y,
                    carRotationCos * (tracking_pos_offset.x + tracking_pos_offset.z));
            }
            target_rot = data.rightHandRotation;
        }
        transform.position = Vector3.Lerp(transform.position, target_pos, 0.05f);
        transform.rotation = Quaternion.Lerp(transform.rotation, target_rot * Quaternion.Euler(tracking_rot_offset), 0.05f);
    }
}
