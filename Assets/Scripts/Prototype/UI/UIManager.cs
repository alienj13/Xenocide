using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("Scene Dependencies")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameController gameController;
    [SerializeField] private Field field;
    [SerializeField] private InGameUI inGameUI;

    [Header("Buttons")]
    [SerializeField] private Button player1TeamButton;
    [SerializeField] private Button player2TeamButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI connectionStatusText;

    [Header("Screen GameObjects")]
    [SerializeField] private GameObject gameModeSelectionScreen;
    [SerializeField] private GameObject connectScreen;
    [SerializeField] private GameObject teamSelectionScreen;
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private GameObject gameUIScreen;

    [Header("Other UI")]
    [SerializeField] private TMP_Dropdown gameLevelSelection;

    [Header("Debugging")]
    [SerializeField] private DebugButton debugButton;
    [SerializeField] private Button turnEndButton;
    [SerializeField] private TextMeshProUGUI currentTurnText;

    private void Awake()
    {
        gameLevelSelection.AddOptions(Enum.GetNames(typeof(PlayerLevel)).ToList());
        OnGameLaunched();
    }

    private void Update()
    {
        if (gameController)
        {
            if (gameController.IsGameInProgess())
                currentTurnText.SetText(gameController.activePlayer.Team.ToString());
        }
    }

    public void SetDependencies(GameController gameController, Field field)
    {
        this.gameController = gameController;
        this.field = field;
    }

    private void OnGameLaunched()
    {
        DisableAllScreens();
        gameModeSelectionScreen.SetActive(true);

    }

    public void OnSingleplayerModeSelected()
    {
        DisableAllScreens();
    }

    public void OnMultiplayerModeSelected()
    {
        DisableAllScreens();
        connectScreen.SetActive(true);
        connectionStatusText.gameObject.SetActive(true);
    }

    public void OnConnect()
    {
        networkManager.SetPlayerLevel((PlayerLevel)gameLevelSelection.value);
        networkManager.Connect();
    }

    public void OnGameStarted()
    {
        DisableAllScreens();
        connectionStatusText.gameObject.SetActive(false);
        gameUIScreen.gameObject.SetActive(true);

        // Debug:
        Debugging();
    }

    public void DisableAllScreens()
    {
        gameModeSelectionScreen.SetActive(false);
        connectScreen.SetActive(false);
        teamSelectionScreen.SetActive(false);
        gameoverScreen.SetActive(false);
        gameUIScreen.SetActive(false);
    }

    public void SetConnectionStatus(string statusText)
    {
        connectionStatusText.SetText(statusText);
    }

    public void ShowTeamSelectionScreen()
    {
        DisableAllScreens();
        teamSelectionScreen.SetActive(true);
    }

    public void SelectTeam(int intTeam)
    {
        networkManager.SelectTeam(intTeam);
    }

    public void RestrictTeamChoice(PlayerTeam occupiedTeam)
    {
        var buttonToDeactivate = (occupiedTeam == PlayerTeam.P1) ? player1TeamButton : player2TeamButton;
        buttonToDeactivate.interactable = false;
    }

    public void OnGameFinished(string winner)
    {
        DisableAllScreens();
        gameoverScreen.SetActive(true);
        resultText.SetText(String.Format("{0} won!", winner));
    }

    public void RestartGame()
    {
        gameController.RestartGame();
    }

    public void EndTurn()
    {
        //Debug.Log("End turn");
        field.EndTurn();
    }

    public void Debugging()
    {
        // TODO: remove this when done testing
        debugButton.gameObject.SetActive(true);
        debugButton.SetDependencies(gameController, field);

        turnEndButton.gameObject.SetActive(true);
        currentTurnText.gameObject.SetActive(true);
    }
}
