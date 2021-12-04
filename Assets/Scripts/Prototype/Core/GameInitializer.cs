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
    [SerializeField] private UnitCreator unitCreator;

    [Header("Debugging")]
    [SerializeField] private DebugButton debugButton;

    public void Awake()
    {
        // TODO: remove this when done testing
        debugButton.gameObject.SetActive(true);
    }

    public void CreateMultiplayerField()
    {
        if (!networkManager.IsRoomFull())
            PhotonNetwork.Instantiate(multiplayerFieldPrefab.name, fieldAnchor.position, fieldAnchor.rotation);
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
            controller.SetDependencies(UIManager, field, cameraController, unitCreator);
            controller.CreatePlayers();

            controller.SetMultiplayerDependencies(networkManager);
            networkManager.SetDependencies(controller);

            field.SetDependencies(controller);
            cameraController.SetDependencies(controller);

            // Debug:
            debugButton.SetDependencies(controller, field);
        }
    }

    public void InitializeSingleplayerController()
    {
        SingleplayerField field = FindObjectOfType<SingleplayerField>();
        if (field)
        {
            SingleplayerGameController controller = Instantiate(singleplayerGameControllerPrefab);
            controller.SetDependencies(UIManager, field, cameraController, unitCreator);
            controller.CreatePlayers();

            field.SetDependencies(controller);
            cameraController.SetDependencies(controller);

            controller.StartNewGame();

            // Debug:
            debugButton.SetDependencies(controller, field);
        }
    }
}
