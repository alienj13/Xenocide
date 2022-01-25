using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private UIManager ui;
    private Field field;

    public void SetDependencies(GameController gameController, Field field)
    {
        this.gameController = gameController;
        this.field = field;
    }

    public void DebugTime()
    {
        Debug.LogError("DEBUG!");

        // Active player of the Game Controller
        //Debug.Log("Game Controller - active player: " + gameController.activePlayer.Team.ToString());

        // Active player of the camera and currently active camera
        //Debug.Log("Camera Controller - active player: " + cameraController.activePlayer.team.ToString());
        //Debug.Log("Camera Controller - current camera: " + cameraController.c.ToString());
        //Debug.Log("Camera Controller - multiplayer instance? " + (cameraController.gameController is MultiplayerGameController));

        // Transform position of the Field
        //Debug.Log(field.transform.position);

        // Type of the selected unit
        Unit selectedUnit = field.selectedUnit;
        if (selectedUnit != null)
        {
            Debug.Log("Selected Unit = " + selectedUnit.GetType());
        }
        else
            Debug.Log("Selected Unit = null");

        // User Account details
        //Debug.Log(UserAccountDetails.username);
        //Debug.Log(UserAccountDetails.userRank);
        //Debug.Log(UserAccountDetails.userEXP);

        // Animation test
        if (selectedUnit != null)
        {
            ui.ShowAnimationScreen();
            Unit test = new XDrone();
            test.Team = (selectedUnit.Team == PlayerTeam.P1) ? PlayerTeam.P2 : PlayerTeam.P1;
            ui.ShowExecutionAnimation(selectedUnit, test);
        }
    }
}
