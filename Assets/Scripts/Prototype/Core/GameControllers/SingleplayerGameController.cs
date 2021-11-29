using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerGameController : GameController
{
    protected override void SetGameState(GameState state)
    {
        this.state = state;
    }

    public override void TryToStartCurrentGame()
    {
        SetGameState(GameState.Play);
    }

    public override bool CanPerformMove()
    {
        if (!IsGameInProgess())
            return false;
        return true;
    }

    protected override void ChangeActiveTeam()
    {
        base.ChangeActiveTeam();
        //DelayGameOnTeamChange();
    }
}
