using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class VrHeadController : NetworkPositionRotation
{
    [SerializeField] private Vector3 tracking_pos_offset;
    [SerializeField] private Vector3 tracking_rot_offset;

    [Networked] private Vector3 target_pos { get; set; }
    [Networked] private Quaternion target_rot { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data) && HasStateAuthority)
        {
            target_pos = data.headPosition;
            target_rot = data.headRotation;
        }
        transform.position = Vector3.Lerp(transform.position, target_pos + tracking_pos_offset, 0.05f);
        transform.rotation = Quaternion.Lerp(transform.rotation, target_rot * Quaternion.Euler(tracking_rot_offset), 0.05f);
    }
}
