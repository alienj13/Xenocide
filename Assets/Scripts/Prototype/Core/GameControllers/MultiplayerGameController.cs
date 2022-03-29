using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

public class MultiplayerGameController : GameController, IOnEventCallback
{
    private const byte SET_GAME_STATE_EVENT_CODE = 1;
    private const byte CREATE_UNITS_EVENT_CODE = 2;

    public XPlayer localPlayer;
    private NetworkManager networkManager;

    public void SetMultiplayerDependencies(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    protected override void SetGameState(GameState state)
    {
        object[] content = new object[] { (int)state };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(SET_GAME_STATE_EVENT_CODE, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public override void CreateUnitsBasedOnRank()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            PlayerTeam team = (((int) player.CustomProperties["team"] == 1) ? PlayerTeam.P1 : PlayerTeam.P2);
            int rank = (int) player.CustomProperties["rank"];

            List<FieldLayout> fieldLayouts = getPlayerLayouts(team, rank);
            foreach (FieldLayout layout in fieldLayouts)
                CreateUnitsFromLayout(layout);
        }
    }

    private void CreateUnitsEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(CREATE_UNITS_EVENT_CODE, null, raiseEventOptions, SendOptions.SendReliable);
    }


    public override void TryToStartCurrentGame()
    {
        if (networkManager.IsRoomFull())
        {
            //CreateUnitsBasedOnRank();
            //activePlayer.OnTurnStart();

            CreateUnitsEvent();
            SetGameState(GameState.Play);
        }
    }
    public override bool CanPerformAction()
    {
        if (!IsGameInProgess() | !IsLocalPlayerTurn())
            return false;
        return true;
    }

    private bool IsLocalPlayerTurn()
    {
        return localPlayer == activePlayer;
    }

    public void SetLocalPlayer(PlayerTeam team)
    {
        localPlayer = (team == PlayerTeam.P1) ? player1 : player2;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == SET_GAME_STATE_EVENT_CODE)
        {
            object[] data = (object[])photonEvent.CustomData;
            GameState state = (GameState)data[0];

            this.state = state;
        }
        else if (eventCode == CREATE_UNITS_EVENT_CODE)
        {
            //Debug.Log("CREATE_UNITS_EVENT");
            CreateUnitsBasedOnRank();
            activePlayer.OnTurnStart();
        }
    }
}
