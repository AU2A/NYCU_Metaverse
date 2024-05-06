using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hand : MonoBehaviour
{
    Animator animator;
    private float grip_target;
    private float grip_current;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHand();
    }

    internal void SetGrip(float v)
    {
        grip_target = v;
    }

    void AnimateHand()
    {
        if(grip_current!=grip_target)
        {
            grip_current = Mathf.MoveTowards(grip_current, grip_target, Time.deltaTime * speed);
            animator.SetFloat("Grip", grip_current);
        }
    }
}
