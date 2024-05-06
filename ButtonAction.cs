using UnityEngine;
using Fusion;

public class ButtonAction : NetworkBehaviour
{

    public GameObject main_menu;
    public GameObject gameplay_ui;
    private GameTimer game_timer;

    private void Start()
    {
        game_timer = GameObject.FindObjectOfType(typeof(GameTimer)) as GameTimer;
        game_timer.StartGameNow(false);
    }

    // push start button
    public void StartGame()
    {
        game_timer.StartGameNow(true);
    }


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out InputData data))
        {
            // if ((data.vrRightControllerAButton == 1 || data.vrLeftControllerAButton == 1) && !game_timer.start_game)
            // {
            //     game_timer.StartGameNow(true);
            // }
        }
    }

    private void Update()
    {
        if (game_timer.start_game && Object.HasInputAuthority)
        {
            // disable and enable ui
            gameplay_ui.SetActive(true);
            main_menu.SetActive(false);

        }
    }

}
