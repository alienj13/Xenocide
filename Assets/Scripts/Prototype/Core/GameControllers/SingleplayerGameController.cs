using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerGameController : GameController
{
    protected override void SetGameState(GameState state)
    {
        this.state = state;
    }

    public override void CreateUnitsBasedOnRank()
    {
        // P1
        List<FieldLayout> fieldLayouts_P1 = getPlayerLayouts(PlayerTeam.P1, 5);
        foreach (FieldLayout layout in fieldLayouts_P1)
            CreateUnitsFromLayout(layout);
        // P2
        List<FieldLayout> fieldLayouts_P2 = getPlayerLayouts(PlayerTeam.P2, 3);
        foreach (FieldLayout layout in fieldLayouts_P2)
            CreateUnitsFromLayout(layout);
    }

    public override void TryToStartCurrentGame()
    {
        CreateUnitsBasedOnRank();
        activePlayer.OnTurnStart();

        SetGameState(GameState.Play);
        SetCameraActive(true);
        UpdateCameraOnTeamChange(activePlayer);
    }

    public override bool CanPerformAction()
    {
        if (!IsGameInProgess())
            return false;
        return true;
    }

    protected override void ChangeActiveTeam()
    {
        base.ChangeActiveTeam();
        //DelayGameOnTeamChange();
        UpdateCameraOnTeamChange(activePlayer);
    }

    public void Update()
    {
        // Space for end-turn
        if (Input.GetKeyUp(KeyCode.Space))
            EndTurn();
    }
}
