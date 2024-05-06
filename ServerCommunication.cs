using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;


public class ServerCommunication : NetworkBehaviour
{
    private BasicSpawner spawner;

    private GameTimer game_timer;

    private void Start()
    {
        game_timer = GameObject.FindObjectOfType(typeof(GameTimer)) as GameTimer;
        spawner = GameObject.FindObjectOfType(typeof(BasicSpawner)) as BasicSpawner;
    }

    //player alert server for winning
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_WonGameMsg(string player_id, RpcInfo info = default)
    {
        //Debug.Log("RPC sent");
        RPC_AnnouceWinner(player_id);
    }


    //server alert all and write in WonGame UI
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AnnouceWinner(string winner)
    {

        foreach (var gameObj in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            //Debug.Log(gameObj.name);
            if (gameObj.name == "WonGame")
            {
                TextMeshProUGUI ui_text = gameObj.GetComponent<TextMeshProUGUI>();
                ui_text.text = winner + " has won the game.";
            }
        }

    }

    //player send this to server to update the player coin pair
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_InformCoinChg(string player_id, int coin_num, RpcInfo info = default)
    {
        //Debug.Log("coin change player: " + player_id + " " + coin_num);
        UpdatePlayerCoins(player_id, coin_num);
    }

    //server update player coin pair
    private void UpdatePlayerCoins(string player_id, int coin_num)
    {
        if (Object.HasStateAuthority)
        {
            spawner.player_coin_list[player_id] = coin_num;

        }

    }

}
