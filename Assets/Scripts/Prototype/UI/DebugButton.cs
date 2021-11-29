using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour
{
    private GameController gameController;
    private Field field;

    public void SetDependencies(GameController gameController, Field field)
    {
        this.gameController = gameController;
        this.field = field;
    }

    public void DebugTime()
    {
        Debug.Log("DEBUG!");
        Debug.Log(gameController.activePlayer.team.ToString());
    }
}
