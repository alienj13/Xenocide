using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Scene Dependencies")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameController gameController;
    [SerializeField] private Field field;
    [SerializeField] private InGameUI inGameUI;
    [SerializeField] private AnimationUI animationUI;

    [Header("Buttons")]
    [SerializeField] private Button player1TeamButton;
    [SerializeField] private Button player2TeamButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private TextMeshProUGUI multiplayerResultText;

    [Header("Screen GameObjects")]
    [SerializeField] private GameObject gameModeSelectionScreen;
    [SerializeField] private GameObject connectScreen;
    [SerializeField] private GameObject teamSelectionScreen;
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private GameObject gameUIScreen;
    [SerializeField] private GameObject inGameUIScreen;
    [SerializeField] private GameObject animationScreen;

    [Header("Other UI")]
    [SerializeField] private TMP_Dropdown gameLevelSelection;

    [Header("Debugging")]
    [SerializeField] private DebugButton debugButton;
    [SerializeField] private Button turnEndButton;
    [SerializeField] private TextMeshProUGUI currentTurnText;
    // Switch this bool for debugButton!
    private bool debugMode = true;

    private void Awake()
    {
        gameLevelSelection.AddOptions(Enum.GetNames(typeof(PlayerLevel)).ToList());
        OnGameLaunched();

        // Temporary solution:
        inGameUI.UpdateUserDetails(UserAccountDetails.username, UserAccountDetails.userRank);
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
        if (BackgroundMusic.Instance)
        {
            BackgroundMusic.Instance.PlayGamePlay();
        }
    }

    public void OnMultiplayerModeSelected()
    {
        DisableAllScreens();
        connectScreen.SetActive(true);
        connectionStatusText.gameObject.SetActive(true);
        if (BackgroundMusic.Instance)
        {
            BackgroundMusic.Instance.PlayGamePlay();
        }
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

    public void OnGameFinished(XPlayer winner)
    {
        DisableAllScreens();
        gameoverScreen.SetActive(true);
        resultText.SetText(String.Format("{0} won!", winner.Team));

        if (gameController is MultiplayerGameController)
        {
            MultiplayerGameController mgc = (MultiplayerGameController)gameController;
            if (mgc.localPlayer == winner)
            {
                
                if (UserAccountDetails.userExist)
                {
                    multiplayerResultText.SetText("XP increase");
                    StartCoroutine(RetrieveExperience(UserAccountDetails.username));
                }
            }
        }
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
        if (debugMode)
        {
            debugButton.gameObject.SetActive(true);
            debugButton.SetDependencies(gameController, field);
        }

        //turnEndButton.gameObject.SetActive(true);
        currentTurnText.gameObject.SetActive(true);
    }

    public void Exit()
    {
        SceneManager.LoadScene(Scenes.Login.ToString());
    }

    // In-game UI
    #region In-game UI
    public void ShowUnitDetails(Unit unit)
    {
        inGameUIScreen.SetActive(true);
        inGameUI.UpdateUnitDetails(unit);
    }

    public void HideUnitDetails()
    {
        inGameUIScreen.SetActive(false);
    }

    public void UpdateUnitDetails(Unit unit)
    {
        inGameUI.UpdateUnitDetails(unit);
    }

    #endregion

    // Animation
    #region Animation
    public void ShowAnimationScreen()
    {
        animationScreen.SetActive(true);
    }

    public void ShowExecutionAnimation(Unit attacker, Unit defender)
    {
        animationUI.ShowExecutionAnimation(attacker, defender);
    }

    public void HideAnimationScreen()
    {
        animationScreen.SetActive(false);
    }
    #endregion

    // [] Temporary solution
    #region XP increase
    public IEnumerator RetrieveExperience(string user)
    {

        WWWForm form = new WWWForm();
        form.AddField("username", user);

        using (UnityWebRequest www = UnityWebRequest.Post("https://xenoregistertest.000webhostapp.com/RetrieveExperience.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
            {

                Debug.Log(www.error);
                StartCoroutine(RetrieveExperience(user));//get experience
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                int experience = int.Parse(www.downloadHandler.text);
                if (experience >= 150)
                {
                    //check if a rank up is due
                    StartCoroutine(UpdateRank(user));//rank up
                }
                else
                {
                    StartCoroutine(UpdateExperience(user));//+50XP
                }
            }
        }
    }
    public IEnumerator UpdateRank(string user)
    {

        WWWForm form = new WWWForm();
        form.AddField("username", user);

        using (UnityWebRequest www = UnityWebRequest.Post("https://xenoregistertest.000webhostapp.com/UpdateRank.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
            {
                StartCoroutine(UpdateRank(user));
            }
            else
            {
                multiplayerResultText.SetText("+50 XP. User ranked up.");
            }
        }
    }
    public IEnumerator UpdateExperience(string user)
    {

        WWWForm form = new WWWForm();
        form.AddField("username", user);

        using (UnityWebRequest www = UnityWebRequest.Post("https://xenoregistertest.000webhostapp.com/UpdateExperience.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
            {
                StartCoroutine(UpdateExperience(user));
            }
            else
            {
                multiplayerResultText.SetText("+50 XP");
            }
        }
    }
    #endregion
}
