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
    [SerializeField] private Transform boardAnchor;
    [SerializeField] private CameraController cameraController;

    public void CreateMultiplayerField()
    {
        // TODO: Re-implement once we have Photon installed
        //if (!networkManager.IsRoomFull())
        //    PhotonNetwork.Instantiate(multiplayerFieldPrefab.name, boardAnchor.position, boardAnchor.rotation);
    }

    public void CreateSingleplayerField()
    {
        Instantiate(singleplayerFieldPrefab, boardAnchor);
    }

    public void InitializeMultiplayerController()
    {
        MultiplayerField board = FindObjectOfType<MultiplayerField>();
        if (board)
        {
            MultiplayerGameController controller = Instantiate(multiplayerGameControllerPrefab);
            controller.SetDependencies(UIManager, board, cameraController);
            controller.CreatePlayers();

            controller.SetMultiplayerDependencies(networkManager);
            networkManager.SetDependencies(controller);

            board.SetDependencies(controller);
        }
    }

    public void InitializeSingleplayerController()
    {
        SingleplayerField board = FindObjectOfType<SingleplayerField>();
        if (board)
        {
            SingleplayerGameController controller = Instantiate(singleplayerGameControllerPrefab);
            controller.SetDependencies(UIManager, board, cameraController);
            controller.CreatePlayers();

            board.SetDependencies(controller);
            controller.StartNewGame();
        }
    }
}
