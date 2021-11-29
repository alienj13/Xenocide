using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameInitializer : MonoBehaviour
{
    [Header("Gamemode dependent objects")]
    [SerializeField] private SingleplayerGameController singleplayerGameControllerPrefab;
    [SerializeField] private MultiplayerGameController multiplayerGameControllerPrefab;
    [SerializeField] private SingleplayerField singleplayerFieldPrefab;
    [SerializeField] private MultiplayerField multiplayerFieldPrefab;

    [Header("Scene references")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private UIManager UIManager;
    [SerializeField] private Transform fieldAnchor;
    [SerializeField] private CameraController cameraController;

    [Header("Debugging")]
    [SerializeField] private DebugButton debugButton;

    public void CreateMultiplayerField()
    {
        // TODO: Re-implement once we have Photon installed
        //if (!networkManager.IsRoomFull())
        //    PhotonNetwork.Instantiate(multiplayerFieldPrefab.name, fieldAnchor.position, fieldAnchor.rotation);
    }

    public void CreateSingleplayerField()
    {
        Instantiate(singleplayerFieldPrefab, fieldAnchor);
    }

    public void InitializeMultiplayerController()
    {
        MultiplayerField field = FindObjectOfType<MultiplayerField>();
        if (field)
        {
            MultiplayerGameController controller = Instantiate(multiplayerGameControllerPrefab);
            controller.SetDependencies(UIManager, field, cameraController);
            controller.CreatePlayers();

            controller.SetMultiplayerDependencies(networkManager);
            networkManager.SetDependencies(controller);

            field.SetDependencies(controller);
        }
    }

    public void InitializeSingleplayerController()
    {
        SingleplayerField field = FindObjectOfType<SingleplayerField>();
        if (field)
        {
            SingleplayerGameController controller = Instantiate(singleplayerGameControllerPrefab);
            controller.SetDependencies(UIManager, field, cameraController);
            controller.CreatePlayers();

            field.SetDependencies(controller);
            controller.StartNewGame();

            debugButton.SetDependencies(controller, field);
        }
    }
}
