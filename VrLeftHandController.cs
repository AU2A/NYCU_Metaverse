using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class VrLeftHandController : NetworkPositionRotation
{
    [SerializeField] private Transform car;
    [SerializeField] private Transform lefttWheelPoint;
    [SerializeField] private Vector3 tracking_pos_offset;
    [SerializeField] private Vector3 tracking_rot_offset;

    [SerializeField] private Animator animator;
    private float grip_target;
    private float grip_current;
    private float speed = 10;
    private string animator_para = "Grip";

    [Networked] private Vector3 target_pos { get; set; }
    [Networked] private Quaternion target_rot { get; set; }


    public override void FixedUpdateNetwork()
    {
        var carRotationSin = Mathf.Sin(car.rotation.eulerAngles.y * Mathf.Deg2Rad);
        var carRotationCos = Mathf.Cos(car.rotation.eulerAngles.y * Mathf.Deg2Rad);

        if (GetInput(out InputData data) && HasStateAuthority)
        {
            if (data.leftControllerGrip == 1)
            {
                target_pos = lefttWheelPoint.position;

                //
                //thumbs.transform.rotation = Quaternion.AngleAxis(thumbs.transform.rotation.z * -80.0f * data.leftControllerGrip, Vector3.left);
                //animator.SetBool("Grip", true);
                //animator.Play("Base Layer");
            }
            else
            {
                target_pos = data.leftHandPosition + new Vector3(
                    carRotationSin * (tracking_pos_offset.x + tracking_pos_offset.z),
                    tracking_pos_offset.y,
                    carRotationCos * (tracking_pos_offset.x + tracking_pos_offset.z));

                //
                //animator.SetBool(animator_para, false);
                //animator.SetBool("Grip", false);
            }
            
            target_rot = data.leftHandRotation;
        }

        if (GetInput(out InputData data2) && HasInputAuthority)
        {
            if (data2.leftControllerGrip == 1)
            {

                //
                //thumbs.transform.rotation = Quaternion.AngleAxis(thumbs.transform.rotation.z * -80.0f * data.leftControllerGrip, Vector3.left);
                animator.SetBool("Grip", true);
                //animator.Play("Base Layer");
            }
            else
            {
                animator.SetBool("Grip", false);
            }
        }

            transform.position = Vector3.Lerp(transform.position, target_pos, 0.05f);
        transform.rotation = Quaternion.Lerp(transform.rotation, target_rot * Quaternion.Euler(tracking_rot_offset), 0.05f);
    }

    void AnimateHand()
    {
        if (grip_current != grip_target)
        {
            grip_current = Mathf.MoveTowards(grip_current, grip_target, Time.deltaTime * speed);
            //animator.SetFloat(animator_para, grip_target);
            animator.SetBool("Grip", true);
            Debug.Log("grip");
            //animator.Play("Grip");
        }
    }
}


