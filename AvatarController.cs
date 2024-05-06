using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[System.Serializable]
public class MapTransforms
{
    public Transform ik_target;
    public Vector3 tracking_pos_offset;
    public Vector3 tracking_rot_offset;
}

public class AvatarController : NetworkBehaviour
{
    [SerializeField] private MapTransforms head;
    [SerializeField] private MapTransforms left_hand;
    [SerializeField] private MapTransforms right_hand;

    [SerializeField] private float turn_smooth;
    [SerializeField] Transform ik_head;
    [SerializeField] Vector3 head_body_offset;
    [SerializeField] Vector3 steeringWheeloffset;

    public override void FixedUpdateNetwork()
    {
        // update user prefab head, hands poisitions and rotations
        if (GetInput(out InputData data) && Object.HasStateAuthority)
        {
            // head.ik_target.position = data.headPosition + head.tracking_pos_offset;
            // head.ik_target.rotation = data.headRotation * Quaternion.Euler(head.tracking_rot_offset);

            // float carRotationSin = Mathf.Sin(data.carRotation.eulerAngles.y * Mathf.Deg2Rad);
            // float carRotationCos = Mathf.Cos(data.carRotation.eulerAngles.y * Mathf.Deg2Rad);

            // if (data.vrRightControllerGrip == 1)
            // {
            //     right_hand.ik_target.position = data.rightWheelPointPoisition;
            // }
            // else
            // {
            //     right_hand.ik_target.position = new Vector3(
            //         data.rightHandPosition.x + carRotationSin * (right_hand.tracking_pos_offset.x + right_hand.tracking_pos_offset.z),
            //         data.rightHandPosition.y + right_hand.tracking_pos_offset.y,
            //         data.rightHandPosition.z + carRotationCos * (right_hand.tracking_pos_offset.x + right_hand.tracking_pos_offset.z));
            // }
            // right_hand.ik_target.rotation = data.rightHandRotation * Quaternion.Euler(right_hand.tracking_rot_offset);

            // if (data.vrLeftControllerGrip == 1)
            // {
            //     left_hand.ik_target.position = data.leftWheelPointPoisition;
            // }
            // else
            // {
            //     left_hand.ik_target.position = new Vector3(
            //         data.leftHandPosition.x + carRotationSin * (left_hand.tracking_pos_offset.x + left_hand.tracking_pos_offset.z),
            //         data.leftHandPosition.y + left_hand.tracking_pos_offset.y,
            //         data.leftHandPosition.z + carRotationCos * (left_hand.tracking_pos_offset.x + left_hand.tracking_pos_offset.z));
            // }

            // left_hand.ik_target.rotation = data.leftHandRotation * Quaternion.Euler(left_hand.tracking_rot_offset);

        }
    }

}
