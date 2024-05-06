using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    public Hand hand;

    [SerializeField] private InputActionReference vrLeftControllerGrip;

    private void Update()
    {
        hand.SetGrip(vrLeftControllerGrip.action.ReadValue<float>());
    }
}