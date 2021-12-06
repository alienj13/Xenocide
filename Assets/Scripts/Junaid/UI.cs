using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
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
    public GameObject InGameUIDisplay;

    public static UI Instance { set; get; }
    public string Result;

    public Client client;
    // public SignUp s = new SignUp();

    // [SerializeField] private TMP_InputField address;
    [SerializeField] public Texture[] units;
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
    [SerializeField] public TMP_Text DisplayUsername;
    [SerializeField] public TMP_Text DisplayRank;
    [SerializeField] public TMP_Text DisplayExperience;
    [SerializeField] public RawImage DisplayUnit;
    [SerializeField] public Text InGameName;
    [SerializeField] public Text InGameRank;
    [SerializeField] public Text InGameUnitType;
    [SerializeField] public Text InGameUnitHealth;
    [SerializeField] public Text InGameUnitAttack;
    [SerializeField] public Text InGameUnitDefense;
    private void Awake() {
        Instance = this;
        front.SetActive(false);
        InGameUIDisplay.SetActive(true);
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

        //InGameUI();

    }
    public void play() {
     
        StartCoroutine(SignUp.Instance.LogIn(Playername.text,  PlayerPassword.text));
      
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
    
    }

    public void back() {
        startMenu.SetActive(true);
        OnlineMenu.SetActive(false);
        client.ShutDown();
    }

    public void HostOnline() {
        Client.Instance.initialize("127.0.0.1", 8007,Playername.text);
        Waiting.SetActive(false);

        OnlineMenu.SetActive(false);
        InGameRank.text = DisplayRank.text;
        InGameName.text = DisplayUsername.text;
        InGameUIDisplay.SetActive(true);
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


    public void VictoryScreen(FixedString128Bytes winner, FixedString128Bytes looser) {
        UI.Instance.WinnerLoser.text = $"{winner} has won, {looser} has lost";
        UI.Instance.Victory.SetActive(true);
        UI.Instance.turns.SetActive(false);
        UI.Instance.status.SetActive(false);

        
    }

    public void InGameUI(Characters c) {
       

        InGameUnitType.text = $"{c.type}";
        InGameUnitHealth.text = $"{c.GetHealth()}";
        InGameUnitAttack.text = $"{c.GetAttack()}";
        InGameUnitDefense.text = $"{c.GetDefense()}";

        if (c.type == characterType.Drone  && Client.Instance.getCurrentTeam() == 0) {
            DisplayUnit.texture = units[1];
            
        }

        else if (c.type == characterType.Drone && Client.Instance.getCurrentTeam() == 1) {
            DisplayUnit.texture = units[0];
        }

        else if (c.type == characterType.Queen && Client.Instance.getCurrentTeam() == 0) {
            DisplayUnit.texture = units[2];
        }

        else if (c.type == characterType.Queen && Client.Instance.getCurrentTeam() == 1) {
            DisplayUnit.texture = units[3];
        }

        else if (c.type == characterType.Warrior && Client.Instance.getCurrentTeam() == 0) {
            DisplayUnit.texture = units[4];
        }

        else if (c.type == characterType.Warrior && Client.Instance.getCurrentTeam() == 1) {
            DisplayUnit.texture = units[5];
        }
    }

}
