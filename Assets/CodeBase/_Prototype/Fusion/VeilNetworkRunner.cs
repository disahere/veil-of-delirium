using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace CodeBase._Prototype.Fusion
{
  public class VeilNetworkRunner : MonoBehaviour, INetworkRunnerCallbacks
  {
    [SerializeField] GameMode gameMode = GameMode.Shared;
    [Space]
    [SerializeField] NetworkObject playerPrefab;

    readonly string _sessionName = "VeilSession";
    NetworkRunner _runner;

    async void Start()
    {
      _runner = GetComponent<NetworkRunner>();
      _runner.ProvideInput = true;
      _runner.AddCallbacks(this);

      await _runner.StartGame(new StartGameArgs
      {
        GameMode    = gameMode,
        SessionName = _sessionName
      });

      if (_runner.IsSharedModeMasterClient)
      {
        _ = _runner.LoadScene(Constants.TestSceneName);
      }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
      Debug.Log($"OnPlayerJoined {player}, IsServer={runner.IsServer}, IsMaster={runner.IsSharedModeMasterClient}");

      if (runner.IsSharedModeMasterClient)
      {
        runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
      }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
  }
}
