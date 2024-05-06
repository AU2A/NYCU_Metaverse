using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner _networkRunner = null;

    [SerializeField]
    private NetworkPrefabRef _playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> _playerList = new Dictionary<PlayerRef, NetworkObject>();

    //variable for coin
    public GameObject coin;

    public Dictionary<string, int> player_coin_list = new Dictionary<string, int>();

    private NetworkObject network_coin;
    //

    //variable for timer
    public GameTimer game_timer;
    //

    //variable for banana
    public GameObject banana;

    private NetworkObject network_banana;
    //

    private void Start()
    {
        StartGame();
    }

    private async void StartGame()
    {
        _networkRunner.ProvideInput = true;

        await _networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "Game Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()

        });

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        ////////////// chong testing for collision //////////////////
        ////////////// need to change before submit ////////////////
        // Vector3 spawnPoint;
        // if (_playerList.Count == 0)
        // {
        //     spawnPoint = new Vector3(0, 0, 0);
        // }
        // else
        // {
        //     spawnPoint = new Vector3(0, 0, -5);
        // }
        ////////////////////////////////////////////////////////

        ///spawn at lobby first///
        Vector3 spawnPoint;
        if (_playerList.Count == 0)
        {
            spawnPoint = new Vector3(-352, -0.18f, -30);
        }
        else
        {
            spawnPoint = new Vector3(-352 + (_playerList.Count * 3), -0.18f, -30);
        }
        ////

        // Vector3 spawnPoint = new Vector3(UnityEngine.Random.Range(-5, 5), 2f, UnityEngine.Random.Range(-5, 5));
        //Vector3 spawnPoint = new Vector3(0f, 0.55f, 0f);
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPoint, Quaternion.identity, player);

        _playerList.Add(player, networkPlayerObject);

        //record all player's coin number
        player_coin_list.Add(player.PlayerId.ToString(), 0);

        // for coin spawn when enough player
        ////////////// need to change to 3 before submit ////////////////
        //if (_playerList.Count == 2)
        //{
        //    Vector3 rand_pos = new Vector3(UnityEngine.Random.Range(-10, 10), 1f, UnityEngine.Random.Range(-10, 10));
        //    network_coin = runner.Spawn(coin, rand_pos, Quaternion.identity);

        //    game_timer.StartTimer(60.0f);
        //    //Debug.Log("start game");
        //}
        //

        // host call all to update their player number
        game_timer.UpdateTotalPlayer(player_coin_list.Count.ToString());
        ///
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_playerList.TryGetValue(player, out var networkObject))
        {
            runner.Despawn(networkObject);
            _playerList.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    //run this function when player collide with coin
    public void CollideWithCoin(Vector3 coin_pos)
    {
        //destroy old coin and spawn new coin
        _networkRunner.Despawn(network_coin);
        network_coin = _networkRunner.Spawn(coin, coin_pos, Quaternion.identity);
    }

    //spawn coin when start game
    public void SpawnCoin(Vector3 coin_pos)
    {
        network_coin = _networkRunner.Spawn(coin, coin_pos, Quaternion.identity);
    }

    //run this function when player collide with coin
    public void CollideWithBanana(Vector3 banana_pos)
    {
        //destroy old coin and spawn new coin
        _networkRunner.Despawn(network_banana);
        network_banana = _networkRunner.Spawn(banana, banana_pos, Quaternion.identity);
    }

    //spawn coin when start game
    public void SpawnBanana(Vector3 banana_pos)
    {
        network_banana = _networkRunner.Spawn(banana, banana_pos, Quaternion.identity);
    }
}