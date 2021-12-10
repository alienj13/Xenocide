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

    [Header("Other UI")]
    [SerializeField] private TMP_Dropdown gameLevelSelection;

    [Header("Debugging")]
    [SerializeField] private DebugButton debugButton;
    [SerializeField] private Button turnEndButton;

    private void Awake()
    {
        gameLevelSelection.AddOptions(Enum.GetNames(typeof(PlayerLevel)).ToList());
        OnGameLaunched();
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

        // Debug:
        Debugging();
    }

    public void DisableAllScreens()
    {
        gameModeSelectionScreen.SetActive(false);
        connectScreen.SetActive(false);
        teamSelectionScreen.SetActive(false);
        gameoverScreen.SetActive(false);
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
        gameoverScreen.SetActive(true);
        resultText.SetText(String.Format("{0} won!", winner));
    }

    public void RestartGame()
    {
        gameController.RestartGame();
    }

    public void EndTurn()
    {
        gameController.EndTurn();
    }

    public void Debugging()
    {
        // TODO: remove this when done testing
        debugButton.gameObject.SetActive(true);
        debugButton.SetDependencies(gameController, field);

        turnEndButton.gameObject.SetActive(true);
    }
}
