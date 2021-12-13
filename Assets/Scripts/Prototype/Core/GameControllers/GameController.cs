using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameController : MonoBehaviour
{
    [SerializeField] private FieldLayout startingFieldLayout;
    private Field field;
    private UIManager UIManager;
    private CameraController cameraController;
    public UnitCreator unitCreator;

    public enum GameState { Init, Play, Pause, Finished }
    [SerializeField] public GameState state;

    protected XPlayer player1;
    protected XPlayer player2;
    public XPlayer activePlayer;

    protected abstract void SetGameState(GameState state);
    public abstract void TryToStartCurrentGame();
    public abstract bool CanPerformAction();

    public void SetDependencies(UIManager UIManager, Field field, CameraController cameraController, UnitCreator unitCreator)
    {
        this.UIManager = UIManager;
        this.field = field;
        this.cameraController = cameraController;
        this.unitCreator = unitCreator;
    }

    public void CreatePlayers()
    {
        player1 = new XPlayer(PlayerTeam.P1, field);
        player2 = new XPlayer(PlayerTeam.P2, field);
    }

    public void StartNewGame()
    {
        SetGameState(GameState.Init);

        UIManager.OnGameStarted();

        CreateUnitsFromLayout(startingFieldLayout);
        activePlayer = player1;
        activePlayer.OnTurnStart();

        TryToStartCurrentGame();
    }

    public void RestartGame()
    {
        Debug.Log("Game restarting.");

        DestroyUnits();
        field.OnGameRestarted();
        player1.OnGameRestarted();
        player2.OnGameRestarted();

        StartNewGame();
    }

    private void DestroyUnits()
    {
        player1.ActiveUnits.ForEach(p => Destroy(p.gameObject));
        player2.ActiveUnits.ForEach(p => Destroy(p.gameObject));
    }

    public bool IsGameInProgess()
    {
        return state == GameState.Play;
    }

    private void CreateUnitsFromLayout(FieldLayout layout)
    {
        for (int i = 0; i < layout.GetUnitsCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            string typeName = layout.GetSquareUnitNameAtIndex(i);
            PlayerTeam team = layout.GetSquarePlayerNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreateUnitAndInitialize(squareCoords, team, type);
        }
    }

    public void CreateUnitAndInitialize(Vector2Int squareCoords, PlayerTeam team, Type type)
    {
        Unit newUnit = unitCreator.CreateUnit(type).GetComponent<Unit>();
        newUnit.SetData(squareCoords, team, field);

        Material teamMaterial = unitCreator.GetTeamMaterial(team);
        if (teamMaterial)
            newUnit.SetMaterial(teamMaterial);

        field.SetUnitOnField(squareCoords, newUnit);

        XPlayer currentPlayer = (team == PlayerTeam.P1) ? player1 : player2;
        currentPlayer.AddUnit(newUnit);
    }

    public bool IsTeamTurnActive(PlayerTeam team)
    {
        return activePlayer.Team == team;
    }

    public void StartTurn()
    {
        activePlayer.OnTurnStart();
    }

    public void EndAction()
    {
        activePlayer.GenerateAllPossibleActions();

        if (CheckIfGameIsFinished())
            EndGame();
    }

    public void EndTurn()
    {
        // In-game UI:
        UIManager.HideUnitDetails();

        activePlayer.OnTurnEnd();

        if (CheckIfGameIsFinished())
            EndGame();
        else
            ChangeActiveTeam();

        activePlayer.OnTurnStart();
    }

    private bool CheckIfGameIsFinished()
    {
        XPlayer opponentPlayer = GetOpponentToPlayer(activePlayer);
        Unit opponentQueen = opponentPlayer.GetUnitsOfType<XQueen>().FirstOrDefault();
        if (!opponentQueen.IsAlive())
            return true;
        return false;
    }

    public void OnUnitRemoved(Unit unit)
    {
        XPlayer unitOwner = GetPlayerOfTeam(unit.Team);
        unitOwner.RemoveUnit(unit);
        Destroy(unit.gameObject);
    }

    private void EndGame()
    {
        Debug.Log("Game Ended.");

        UIManager.OnGameFinished(activePlayer);
        SetGameState(GameState.Finished);
    }

    protected virtual void ChangeActiveTeam()
    {
        activePlayer = (activePlayer == player1) ? player2 : player1;
    }

    private XPlayer GetOpponentToPlayer(XPlayer player)
    {
        return (player == player1) ? player2 : player1;
    }

    public XPlayer GetPlayerOfTeam(PlayerTeam team)
    {
        switch (team)
        {
            case PlayerTeam.P1:
                return player1;
            case PlayerTeam.P2:
                return player2;
            default:
                return null;
        }
    }

    #region In-game UI
    public void ShowUnitDetails(Unit unit)
    {
        UIManager.ShowUnitDetails(unit);
    }

    public void HideUnitDetails()
    {
        UIManager.HideUnitDetails();
    }
    #endregion

    #region CameraControls
    // TODO: FIX THIS
    public void DelayGameOnTeamChange()
    {
        SetGameState(GameState.Pause);
        Invoke("UpdateCameraTransform", 2);
    }

    public void UpdateCameraTransform()
    {
        // TODO: Implement this
        //cameraController.UpdateCameraOnTeamChange();
        SetGameState(GameState.Play);
    }

    public void UpdateCameraOnTeamChange(XPlayer activePlayer)
    {
        cameraController.SetActivePlayerCamera(activePlayer);
    }

    public void SetupCamera(PlayerTeam team)
    {
        // TODO: Implement this when doing multiplayer
        cameraController.SetLocalPlayerCamera(team);
    }

    public void SetCameraActive(bool activation)
    {
        cameraController.setActive(activation);
    }
    #endregion
}
