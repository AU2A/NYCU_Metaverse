using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;


// public class InputHandler : NetworkBehaviour, IBeforeUpdate
public class InputHandler : NetworkBehaviour
{
    [SerializeField] private Transform vrHeadset;
    [SerializeField] private Transform vrLeftHand;
    [SerializeField] private Transform vrRightHand;

    [SerializeField] private InputActionReference vrLeftControllerPosition;
    [SerializeField] private InputActionReference vrLeftControllerRotation;
    [SerializeField] private InputActionReference vrLeftControllerGrip;
    [SerializeField] private InputActionReference vrLeftControllerAButton;
    [SerializeField] private InputActionReference vrRightControllerPosition;
    [SerializeField] private InputActionReference vrRightControllerRotation;
    [SerializeField] private InputActionReference vrRightControllerTrigger;
    [SerializeField] private InputActionReference vrRightControllerGrip;
    [SerializeField] private InputActionReference vrRightControllerAButton;
    [SerializeField] private InputActionReference vrLeftControllerXButton;
    [SerializeField] private Transform wheelRotation;
    [SerializeField] private Transform wheelLeftPoint;
    [SerializeField] private Transform wheelRightPoint;
    [SerializeField] private Transform carPosition;
    [SerializeField] private Transform carRotation;

    private float angle = 0f;

    private float wheelAngle = 0f;

    private bool flag = false;

    public override void Spawned()
    {
        if (Runner.LocalPlayer != Object.InputAuthority) return;

        var events = Runner.GetComponent<NetworkEvents>();
        events.OnInput.AddListener(OnInput);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Runner.LocalPlayer != Object.InputAuthority) return;

        var events = Runner.GetComponent<NetworkEvents>();
        events.OnInput.RemoveListener(OnInput);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var inputData = new InputData();

        // get controller input data ------------------------------
        var LeftControllerPosition = vrLeftControllerPosition.action.ReadValue<Vector3>();
        var LeftControllerRotation = vrLeftControllerRotation.action.ReadValue<Quaternion>();
        var LeftControllerGrip = vrLeftControllerGrip.action.ReadValue<float>();
        var LeftControllerAButton = vrLeftControllerAButton.action.ReadValue<float>();
        var RightControllerPosition = vrRightControllerPosition.action.ReadValue<Vector3>();
        var RightControllerRotation = vrRightControllerRotation.action.ReadValue<Quaternion>();
        var RightControllerTrigger = vrRightControllerTrigger.action.ReadValue<float>();
        var RightControllerGrip = vrRightControllerGrip.action.ReadValue<float>();
        var RightControllerAButton = vrRightControllerAButton.action.ReadValue<float>();
        var LeftControllerXButton = vrLeftControllerXButton.action.ReadValue<float>();

        // motor velocity ----------------------------------------
        var move = RightControllerTrigger;
        // keyboard input
        if (Input.GetKey(KeyCode.W))
            move = 1f;
        else if (Input.GetKey(KeyCode.S))
            move = -1f;
        // set input data
        inputData.moveVelocity = move;

        // wheel angle -------------------------------------------
        // left controller angle
        var leftControllerAngle = 0f;
        if (LeftControllerGrip == 1)
        {
            leftControllerAngle = LeftControllerRotation.eulerAngles.z;
            if (leftControllerAngle > 180f)
                leftControllerAngle -= 360f;
        }
        // right controller angle
        var rightControllerAngle = 0f;
        if (RightControllerGrip == 1)
        {
            rightControllerAngle = RightControllerRotation.eulerAngles.z;
            if (rightControllerAngle > 180f)
                rightControllerAngle -= 360f;
        }
        // calculate vr controller angle
        var vrControllerAngle = (rightControllerAngle + leftControllerAngle);
        if (RightControllerGrip == 1 && LeftControllerGrip == 1)
            wheelAngle = vrControllerAngle / 2;
        else if (RightControllerGrip == 1 || LeftControllerGrip == 1)
            wheelAngle = vrControllerAngle;
        // keyboard input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            if (Input.GetKey(KeyCode.A))
                wheelAngle = 20;
            else if (Input.GetKey(KeyCode.D))
                wheelAngle = -20;
            else
                wheelAngle = 0;
        // set wheelAngle in the range of -90 to 90
        if (wheelAngle > 90f)
            wheelAngle = 90f;
        else if (wheelAngle < -90f)
            wheelAngle = -90f;
        // set input data
        inputData.wheelAngle = wheelAngle;

        // car angle ---------------------------------------------
        if (move > 0)
        {
            angle += wheelAngle / 20;
        }
        inputData.carAngle = -angle;


        // Game World headset and controller position and rotation
        inputData.headPosition = vrHeadset.position;
        inputData.headRotation = vrHeadset.rotation;
        inputData.leftHandPosition = vrLeftHand.position;
        inputData.leftHandRotation = vrLeftHand.rotation;
        inputData.rightHandPosition = vrRightHand.position;
        inputData.rightHandRotation = vrRightHand.rotation;

        inputData.leftControllerGrip = LeftControllerGrip;
        inputData.rightControllerGrip = RightControllerGrip;

        // // get steering wheel rotation
        // inputData.leftWheelPointPoisition = wheelLeftPoint.position;
        // inputData.rightWheelPointPoisition = wheelRightPoint.position;

        //voice
        // 
        if ((LeftControllerXButton > 0f) || Input.GetKeyDown(KeyCode.M))
        {
            inputData.Buttons.Set(InputButton.SPEAK, true);
            //flag = true;
        }
        else
        {
            inputData.Buttons.Set(InputButton.SPEAK, false);
            //flag = false;
        }
        
        input.Set(inputData);
    }
}
