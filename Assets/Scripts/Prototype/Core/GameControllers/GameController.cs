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
    private UnitCreator unitCreator;

    public enum GameState { Init, Play, Pause, Finished }
    [SerializeField] public GameState state;

    protected XPlayer player1;
    protected XPlayer player2;
    public XPlayer activePlayer;

    protected abstract void SetGameState(GameState state);
    public abstract void TryToStartCurrentGame();
    public abstract bool CanPerformMove();

    private void Awake()
    {
        //unitCreator = GetComponent<UnitCreator>();
    }

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

        CreatePiecesFromLayout(startingFieldLayout);
        activePlayer = player1;
        GenerateAllPossiblePlayerMoves(activePlayer);

        TryToStartCurrentGame();
    }

    public void SetupCamera(PlayerTeam team)
    {
        //cameraController.SetupCamera(team);
    }

    public void RestartGame()
    {
        Debug.Log("Game restarting.");

        DestroyPieces();
        field.OnGameRestarted();
        player1.OnGameRestarted();
        player2.OnGameRestarted();

        StartNewGame();
    }

    private void DestroyPieces()
    {
        player1.activeUnits.ForEach(p => Destroy(p.gameObject));
        player2.activeUnits.ForEach(p => Destroy(p.gameObject));
    }

    public bool IsGameInProgess()
    {
        return state == GameState.Play;
    }

    private void CreatePiecesFromLayout(FieldLayout layout)
    {
        for (int i = 0; i < layout.GetUnitsCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            string typeName = layout.GetSquareUnitNameAtIndex(i);
            PlayerTeam team = layout.GetSquarePlayerNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    public void CreatePieceAndInitialize(Vector2Int squareCoords, PlayerTeam team, Type type)
    {
        Unit newUnit = unitCreator.CreateUnit(type).GetComponent<Unit>();
        newUnit.SetData(squareCoords, team, field);

        Material teamMaterial = unitCreator.GetTeamMaterial(team);
        newUnit.SetMaterial(teamMaterial);

        field.SetUnitOnField(squareCoords, newUnit);

        XPlayer currentPlayer = (team == PlayerTeam.P1) ? player1 : player2;
        currentPlayer.AddUnit(newUnit);
    }

    private void GenerateAllPossiblePlayerMoves(XPlayer player)
    {
        player.GenerateAllPosibleMoves();
    }

    public bool IsTeamTurnActive(PlayerTeam team)
    {
        return activePlayer.team == team;
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));

        if (CheckIfGameIsFinished())
            EndGame();
        else
            ChangeActiveTeam();
    }

    private bool CheckIfGameIsFinished()
    {
        // TODO: Implement this by checking the active player Queen IsAlive()
        return false;
    }

    public void OnUnitRemoved(Unit unit)
    {
        XPlayer unitOwner = (unit.team == PlayerTeam.P1) ? player1 : player2;
        unitOwner.RemoveUnit(unit);
        Destroy(unit.gameObject);
    }

    private void EndGame()
    {
        Debug.Log("Game Ended.");

        UIManager.OnGameFinished(activePlayer.team.ToString());
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

    #region CameraControls
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
    #endregion
}
