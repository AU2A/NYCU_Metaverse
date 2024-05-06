using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class GameTimer : NetworkBehaviour
{
    // variable for timer countdown
    [Networked] TickTimer timer { get; set; }

    [Networked] bool start_countdown { get; set; }

    [Header("Timer")]
    public BasicSpawner spawner;

    public string local_timer;
    //

    // variable for update player number
    [Header("Player Number")]
    public string total_player = "0";

    [Networked] int total_player_networked { get; set; }
    //

    // variable when host push start button
    [Networked] public bool start_game { get; set; }
    //

    public void StartTimer(float seconds)
    {
        timer = TickTimer.CreateFromSeconds(Runner, seconds);
        local_timer = seconds.ToString();
        start_countdown = true;
    }

    public void StopTimer()
    {
        start_countdown = false;
        timer = TickTimer.None;
    }

    public bool IsGameRunning()
    {
        return start_countdown;
    }

    public override void FixedUpdateNetwork()
    {

        if (start_countdown)
        {
            //check if time's up
            if (timer.Expired(Runner))
            {
                local_timer = "Time's Up";
                start_countdown = false;

                //find winner
                if (Object.HasStateAuthority)
                {
                    string player_with_maxcoin = "-1";
                    int max_coin = -999;
                    bool draw = false;
                    foreach (var pairs in spawner.player_coin_list)
                    {
                        //Debug.Log("winning pairs: " + pairs.Key + " " + pairs.Value);
                        if (pairs.Value > max_coin)
                        {
                            max_coin = pairs.Value;
                            player_with_maxcoin = pairs.Key;
                            draw = false;
                        }
                        else if (pairs.Value == max_coin)
                        {
                            draw = true;
                        }
                    }
                    if (draw) player_with_maxcoin = "-1";
                    RPC_AnnouceWinnerForTimeUp(player_with_maxcoin);
                }
            }
            else
            {
                //write remaining time
                int timer_int = (int)timer.RemainingTime(Runner);
                local_timer = "Remaining Time: " + timer_int.ToString();
            }
        }
    }

    //server tell all about winner with highest coin number when time's up
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AnnouceWinnerForTimeUp(string winner)
    {
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            //Debug.Log(gameObj.name);
            if (gameObj.name == "WonGame")
            {
                TextMeshProUGUI ui_text = gameObj.GetComponent<TextMeshProUGUI>();
                if (winner == "-1")
                    ui_text.text = "Draw.";
                else
                    ui_text.text = winner + " has won the game.";
            }
        }
    }

    // host will update total player
    public void UpdateTotalPlayer(string player_num)
    {

        if (Object.HasStateAuthority)
        {
            //Debug.Log("update: " + player_num);
            total_player_networked = int.Parse(player_num);
            //Debug.Log("network: " + total_player_networked);
            RPC_UpdateTotalPlayer(player_num);


        }
        else if (Object.HasInputAuthority)
        {
            ClientUpdateTotalPlayer(total_player_networked.ToString());
        }
    }

    // host send player number to all
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateTotalPlayer(string player_num)
    {
        //Debug.Log("player num rpc: " + player_num);
        ClientUpdateTotalPlayer(player_num);


    }

    // client update total player number
    private void ClientUpdateTotalPlayer(string player_num)
    {
        total_player = player_num;
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (gameObj.name == "PlayerNumber")
            {
                TextMeshProUGUI ui_text = gameObj.GetComponent<TextMeshProUGUI>();
                ui_text.text = "Total Player: " + player_num;
            }
        }
    }

    // new client update total player number
    public void GetNetworkedVariable()
    {
        total_player = total_player_networked.ToString();
        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            //Debug.Log(gameObj.name);
            if (gameObj.name == "PlayerNumber")
            {
                TextMeshProUGUI ui_text = gameObj.GetComponent<TextMeshProUGUI>();
                ui_text.text = "Total Player: " + total_player;
            }
        }
    }

    // check if player number is enough
    public bool CheckStartCondition()
    {
        if (total_player_networked >= 1)
        {
            return true;
        }
        return false;
    }

    // set networked variable when host push start button
    public void StartGameNow(bool can_start)
    {
        start_game = can_start;
    }
}
