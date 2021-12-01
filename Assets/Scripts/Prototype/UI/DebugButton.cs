using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private CameraController cameraController;
    private Field field;

    public void SetDependencies(GameController gameController, Field field)
    {
        this.gameController = gameController;
        this.field = field;
    }

    public void DebugTime()
    {
        Debug.Log("DEBUG!");

        Debug.Log("Game Controller - active player: " + gameController.activePlayer.team.ToString());

        //Debug.Log("Camera Controller - active player: " + cameraController.activePlayer.team.ToString());
        //Debug.Log("Camera Controller - current camera: " + cameraController.c.ToString());
        //Debug.Log("Camera Controller - multiplayer instance? " + (cameraController.gameController is MultiplayerGameController));

        Debug.Log(field.transform.position);
    }
}
