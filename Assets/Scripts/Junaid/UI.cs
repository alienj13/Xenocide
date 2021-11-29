using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject OnlineMenu;
    public GameObject Waiting;
    public GameObject DisconnectedPlayer;
    public GameObject turns;
    public GameObject status;
    public GameObject front;
    public GameObject Register;
    public GameObject Notification;
    public GameObject Victory;
    public static UI Instance { set; get; }
    public string Result;

    public Client client;
   // public SignUp s = new SignUp();

   // [SerializeField] private TMP_InputField address;
    [SerializeField] public TMP_InputField Playername;
    [SerializeField] public TMP_InputField PlayerPassword;
    [SerializeField] private Button LoginButton;
    [SerializeField] public TMP_Text WinnerLoser;
    [SerializeField] public TMP_Text PlayersTurn;
    [SerializeField] public TMP_Text StatusText;
    [SerializeField] public TMP_Text NotificationText;
    [SerializeField] public TMP_InputField RegisterName;
    [SerializeField] public TMP_InputField RegisterEmail;
    [SerializeField] public TMP_InputField RegisterPassword;
    [SerializeField] private Button RegisterButton;

    private void Awake() {
        Instance = this;
        front.SetActive(true);
        startMenu.SetActive(false);
        Register.SetActive(false);
        OnlineMenu.SetActive(false);
        Waiting.SetActive(false);
        turns.SetActive(false);
        status.SetActive(false);
        Notification.SetActive(false);
    }
    public void Update() {
        LoginButton.enabled = !string.IsNullOrWhiteSpace(Playername.text)
         && !string.IsNullOrWhiteSpace(PlayerPassword.text);

        RegisterButton.enabled =
            !string.IsNullOrWhiteSpace(RegisterName.text) &&
            !string.IsNullOrWhiteSpace(RegisterPassword.text) &&
            !string.IsNullOrWhiteSpace(RegisterEmail.text);

    }
    public void play() {
       // Debug.Log(Playername.text);
        // Debug.Log(PlayerPassword.text);
        StartCoroutine(SignUp.Instance.LogIn(Playername.text,  PlayerPassword.text));
       // startMenu.SetActive(false);
        //OnlineMenu.SetActive(true);
    }

    public void GoToLogin() {
        startMenu.SetActive(true);
        front.SetActive(false);
        Register.SetActive(false);
    }

    public void GotoRegister() {
        startMenu.SetActive(false);
        Register.SetActive(true);
    }

    public void CreateNewAccount() {

        StartCoroutine(SignUp.Instance.CreateAccount(RegisterName.text,RegisterEmail.text,RegisterPassword.text));
       // Debug.Log(SignUp.Instance.get());
    }

    public void back() {
        startMenu.SetActive(true);
        OnlineMenu.SetActive(false);
        client.ShutDown();
    }

    public void HostOnline() {
        Client.Instance.initialize("127.0.0.1", 8007,Playername.text);
        Waiting.SetActive(true);
        OnlineMenu.SetActive(false);
    }

    public void backToOnlineMenu() {
        OnlineMenu.SetActive(true);
        Waiting.SetActive(false);
        client.ShutDown();
    }

    public void ExitFromDisconnect() {
        DisconnectedPlayer.SetActive(false);
        startMenu.SetActive(true);
        client.ShutDown();
        turns.SetActive(false);
        status.SetActive(false);
    }

    public void CloseNotification() {
        Notification.SetActive(false);
    }

}
