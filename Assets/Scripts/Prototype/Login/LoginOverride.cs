using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UI))]
public class LoginOverride : MonoBehaviour
{
    // Override Junaid's UI

    private UI loginUI;

    [Header("Scenes")]
    [SerializeField] GameObject frontMenu;
    [SerializeField] GameObject howToPlayMenu;

    private void Awake()
    {
        loginUI = GetComponent<UI>();
    }

    public void StartGameWithUser()
    {
        UserAccountDetails.username = loginUI.DisplayUsername.text;
        UserAccountDetails.userRank = loginUI.DisplayRank.text;
        UserAccountDetails.userEXP = loginUI.DisplayExperience.text;
        UserAccountDetails.userExist = true;

        SceneManager.LoadScene(Scenes.PrototypeX.ToString());
    }

    public void StartGameAsGuest()
    {
        UserAccountDetails.username = "Guest";
        UserAccountDetails.userRank = "1";
        UserAccountDetails.userEXP = "0";
        UserAccountDetails.userExist = false;

        SceneManager.LoadScene(Scenes.PrototypeX.ToString());
    }

    // [] Temporary solution
    public void GoToHowToPlay()
    {
        frontMenu.SetActive(false);
        howToPlayMenu.SetActive(true);
    }

    public void GoToFront()
    {
        frontMenu.SetActive(true);
        howToPlayMenu.SetActive(false);
    }

    public void OpenRegisterPage()
    {
        Application.OpenURL("https://xenocide.me/register");
    }
}
