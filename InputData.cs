using Fusion;
using UnityEngine;

public enum InputButton
{
    FORWARD,
    BACKWARD,
    LEFT,
    RIGHT,
    SPEAK
    // JUMP
}

public struct InputData : INetworkInput
{
    public NetworkButtons Buttons;

    public float moveVelocity;
    public float wheelAngle;
    public float carAngle;

    public Vector3 headPosition;
    public Quaternion headRotation;
    public Vector3 leftHandPosition;
    public Quaternion leftHandRotation;
    public Vector3 rightHandPosition;
    public Quaternion rightHandRotation;

    public float leftControllerGrip;
    public float rightControllerGrip;

}