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

    public void ChangeScene()
    {
        UserAccountDetails.username = loginUI.DisplayUsername.text;
        UserAccountDetails.userRank = loginUI.DisplayRank.text;
        UserAccountDetails.userEXP = loginUI.DisplayExperience.text;

        SceneManager.LoadScene(Scenes.PrototypeX.ToString());
    }
}
