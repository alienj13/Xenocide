using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UI))]
public class LoginOverride : MonoBehaviour
{
    // Override Junaid's UI

    private UI loginUI;

    private enum Scenes
    {
        Login, PrototypeX
    }

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
}
