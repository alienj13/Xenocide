using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private UIManager UIManager;
    [SerializeField] private GameInitializer gameInitializer;
    private MultiplayerGameController gameController;

    private const string LEVEL = "level";
    private const string TEAM = "team";
    private const int MAX_PLAYERS = 2;

    private PlayerLevel playerLevel;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void SetDependencies(MultiplayerGameController GameController)
    {
        this.gameController = GameController;
    }

    private void Update()
    {
        UIManager.SetConnectionStatus(PhotonNetwork.NetworkClientState.ToString());
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            // Debug:
            Debug.LogError($"Connected to server. Looking for random room with level [ {playerLevel} ].");

            PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
        }
        else
            PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        // Debug:
        Debug.LogError($"Connected to server. Looking for random room with level [ {playerLevel} ].");

        PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Debug:
        Debug.LogError($"Joining random room failed because of: {message}.");
        Debug.LogError($"Creating new room with player level [ {playerLevel} ].");

        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            CustomRoomPropertiesForLobby = new string[] { LEVEL },
            MaxPlayers = MAX_PLAYERS,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } },
        });
    }

    public override void OnJoinedRoom()
    {
        // Debug:
        Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} has joined the room with level [ {(PlayerLevel)PhotonNetwork.CurrentRoom.CustomProperties[LEVEL]} ].");

        gameInitializer.CreateMultiplayerField();

        PrepareTeamSelectionOption();
        UIManager.ShowTeamSelectionScreen();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Debug:
        Debug.LogError($"Player {newPlayer.ActorNumber} has joined the room.");
    }
    #endregion

    public void SetPlayerLevel(PlayerLevel level)
    {
        playerLevel = level;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } });
    }

    public void SelectTeam(int intTeam)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { TEAM, intTeam } });
        gameInitializer.InitializeMultiplayerController();
        gameController.SetLocalPlayer((PlayerTeam)intTeam);

        gameController.StartNewGame();
        gameController.SetupCamera((PlayerTeam)intTeam);
        gameController.SetCameraActive(true);
        // Debug:
        //Debug.LogError("Team Selected: " + intTeam);
    }

    private void PrepareTeamSelectionOption()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            var firstPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1);
            if (firstPlayer.CustomProperties.ContainsKey(TEAM))
            {
                var occupiedTeam = firstPlayer.CustomProperties[TEAM];
                UIManager.RestrictTeamChoice((PlayerTeam)occupiedTeam);
            }
        }
    }

    public bool IsRoomFull()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
