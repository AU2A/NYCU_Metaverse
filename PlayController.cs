using Fusion;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Photon.Voice.Unity;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private Camera _camera;
    [SerializeField] private MeshRenderer[] _visuals;




    [Networked] private NetworkButtons _previousButton { get; set; }

    [Networked] private float wheelAngle { get; set; }
    [Networked] private Vector3 wheelPosition { get; set; }

    // variable for coin
    [Networked(OnChanged = nameof(OnCoinChg))]
    public int coins_num { get; set; }
    public TextMeshProUGUI coinUI;
    public TextMeshProUGUI current_playerUI;
    public GameObject other_canvas;

    public float airborne = 0f;

    private int init_coin = 0;
    private string id;
    private ServerCommunication server_comm;
    //

    // variable for timer
    private GameTimer game_timer;
    //

    // invisible for camera
    public GameObject[] invisible_part;
    //

    // variable for main menu ui
    public GameObject main_menu;
    private bool can_start = false;
    private BasicSpawner spawner;
    private bool flag = false;
    //

    //voice
    private Recorder recorder;
    public Image speaking_indicator;
    private bool prev_speak = false;

    public Image player_voice_indicator;
    //

    private void Start()
    {
        server_comm = GameObject.FindObjectOfType(typeof(ServerCommunication)) as ServerCommunication;
        game_timer = GameObject.FindObjectOfType(typeof(GameTimer)) as GameTimer;
        spawner = GameObject.FindObjectOfType(typeof(BasicSpawner)) as BasicSpawner;

        //disable game canvas
        other_canvas.SetActive(false);

        // check if player number is enough and enable start button
        if (Object.HasStateAuthority)
        {
            can_start = game_timer.CheckStartCondition();
            Debug.Log(can_start);
            if (can_start)
            {
                foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
                {
                    //Debug.Log(gameObj.name);
                    if (gameObj.name == "Start")
                    {
                        Button start = gameObj.GetComponent<Button>();
                        start.interactable = true;
                    }
                }

            }

        }
        //

        //voice
        if (Object.HasInputAuthority)
        {
            recorder = GameObject.FindObjectOfType(typeof(Recorder)) as Recorder;

        }
        //

    }

    public override void Spawned()
    {
        game_timer = GameObject.FindObjectOfType(typeof(GameTimer)) as GameTimer;

        if (Object.HasInputAuthority)
        {
            _camera.enabled = true;
            _camera.GetComponent<AudioListener>().enabled = true;

            foreach (var visual in _visuals)
            {
                visual.enabled = false;
            }

            //get current player id
            id = Runner.LocalPlayer.PlayerId.ToString();
            current_playerUI.text = "Current player: " + id;

            // update total player number for newly spawned client
            game_timer.GetNetworkedVariable();
        }
        else
        {
            _camera.enabled = false;
            _camera.GetComponent<AudioListener>().enabled = false;

            //deactive canvas that is not belongs to current player
            other_canvas.SetActive(false);
            main_menu.SetActive(false);

            //show other player head to camera
            foreach (var part in invisible_part)
            {
                part.layer = LayerMask.NameToLayer("Default");
            }
        }

        //initialize coin number
        if (Object.HasStateAuthority)
        {
            coins_num = init_coin;
        }
    }

    public override void FixedUpdateNetwork()
    {
        //
        // if (GetInput(out InputData data) && game_timer.IsGameRunning())
        if (GetInput(out InputData data))
        {
            NetworkButtons buttons = data.Buttons;
            var buttonPressed = buttons.GetPressed(_previousButton);
            _previousButton = buttons;

            // car movement
            var moveInput = new Vector3(0, 0, data.moveVelocity * 3);
            transform.position += transform.rotation * moveInput * Runner.DeltaTime;
            transform.rotation = Quaternion.Euler(0, data.carAngle, 0);


            // if (buttonPressed.IsSet(InputButton.JUMP) || buttonPressed.IsSet(InputButton.JUMP_VR))
            // {
            //     _characterController.Jump();
            // }

            // enable speaking
            if ((prev_speak == false) && buttons.IsSet(InputButton.SPEAK) && Object.HasInputAuthority)
            {
                //StartCoroutine(TriggerSpeak(buttons));
                if (recorder.TransmitEnabled)
                {
                    //Debug.Log("not transmit");
                    recorder.TransmitEnabled = false;
                    RPC_ChangeSpeaking(false);
                    player_voice_indicator.enabled = false;
                }
                else
                {
                    //Debug.Log("transmit");
                    recorder.TransmitEnabled = true;
                    prev_speak = true;
                    RPC_ChangeSpeaking(true);
                    player_voice_indicator.enabled = true;

                }

            }
            else
            {
                prev_speak = false;
            }

        }

        //teleport player from lobby to arena when start button is pushed
        if (Object.HasStateAuthority && Object.HasInputAuthority)
        {
            if (game_timer.start_game && !flag)
            {
                flag = true;
                foreach (var gameObj in GameObject.FindGameObjectsWithTag("Player") as GameObject[])
                {

                    if (gameObj.name == "Network Player(Clone)")
                    {
                        Vector3 spawnPoint = new Vector3(UnityEngine.Random.Range(-5, 5), 0.55f, UnityEngine.Random.Range(-5, 5));
                        gameObj.transform.position = spawnPoint;
                    }
                }

                StartCoroutine("Teleport");
            }
        }

    }

    // teleport player, spawn coin and start timer countdown
    IEnumerator Teleport()
    {
        Debug.Log("teleport");
        yield return new WaitForSeconds(0.01f);
        game_timer.StartGameNow(false);

        Vector3 rand_pos = new Vector3(UnityEngine.Random.Range(-10, 10), 1f, UnityEngine.Random.Range(-10, 10));
        spawner.SpawnCoin(rand_pos);
        Vector3 rand_pos2 = new Vector3(UnityEngine.Random.Range(-10, 10), 1f, UnityEngine.Random.Range(-10, 10));
        spawner.SpawnBanana(rand_pos2);

        game_timer.StartTimer(60.0f);
        Debug.Log("start game");


    }

    //player get coin
    public void GainCoin(int n)
    {
        if (Object.HasStateAuthority)
        {
            coins_num += n;
        }
    }

    //player loss coin
    public void LossCoin(int n)
    {
        if (Object.HasStateAuthority && coins_num > 0)
        {
            coins_num -= n;
        }
    }

    //change text if coin number is changed
    private static void OnCoinChg(Changed<PlayerController> changed)
    {
        changed.Behaviour.coinUI.text = "Coin: " + changed.Behaviour.coins_num.ToString();

        if (changed.Behaviour.coins_num >= 1 && changed.Behaviour.Object.HasInputAuthority)
        {
            changed.Behaviour.server_comm.RPC_InformCoinChg(changed.Behaviour.id, changed.Behaviour.coins_num);

            //check winning condition
            if (changed.Behaviour.coins_num >= 10)
            {
                changed.Behaviour.server_comm.RPC_WonGameMsg(changed.Behaviour.id);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BackCollider")
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();

            //only decrease other player's coin
            if (player.id != id)
                player.LossCoin(1);
        }

    }

    //show speaking image when talk
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_ChangeSpeaking(bool flag)
    {
        speaking_indicator.enabled = flag;
    }

}