using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

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
    public GameObject Lose;
    public GameObject InGameUIDisplay;
    public GameObject ExperienceWin;

    public static UI Instance { set; get; }
    public string Result;

   

    [SerializeField] public Texture[] units;
    [SerializeField] public TMP_InputField Playername;
    [SerializeField] public TMP_InputField PlayerPassword;
    [SerializeField] private Button LoginButton;
    [SerializeField] public TMP_Text Loser;
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
    [SerializeField] public TMP_Text ExperienceWinText;
    [SerializeField] public GameObject test;
    [SerializeField] public VideoClip[] vids ;
    [SerializeField]  public VideoPlayer vp;
    private void Awake() {
        Instance = this;
        front.SetActive(true);
        InGameUIDisplay.SetActive(false);
        startMenu.SetActive(false);
        Register.SetActive(false);
        OnlineMenu.SetActive(false);
        Waiting.SetActive(false);
        turns.SetActive(false);
        status.SetActive(false);
        Notification.SetActive(false);
        ExperienceWin.SetActive(false);
        Lose.SetActive(false);
       
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
        Client.Instance.ShutDown();
    }

    public void HostOnline() {
        Client.Instance.initialize("139.59.186.112", 8007,Playername.text);
        Waiting.SetActive(true);

        OnlineMenu.SetActive(false);
        InGameRank.text = DisplayRank.text;
        InGameName.text = DisplayUsername.text;
       // InGameUIDisplay.SetActive(true);
    }

    public void backToOnlineMenu() {
        OnlineMenu.SetActive(true);
        Waiting.SetActive(false);
        Client.Instance.ShutDown();
    }

    public void ExitFromDisconnect() {
        DisconnectedPlayer.SetActive(false);
        startMenu.SetActive(true);
        Client.Instance.ShutDown();
        turns.SetActive(false);
        status.SetActive(false);
        InGameUIDisplay.SetActive(false);
    }

    public void CloseNotification() {
        Notification.SetActive(false);
        
    }

    public void Exit() {
        StartCoroutine(SignUp.Instance.RetrieveRank(Playername.text));
        StartCoroutine(SignUp.Instance.RetrieveExperience(Playername.text));
        Lose.SetActive(false);
        OnlineMenu.SetActive(true);
        turns.SetActive(false);
        status.SetActive(false);
        ExperienceWin.SetActive(false);
        Client.Instance.ShutDown();
    }

    public void LoseScreen() {
        Loser.text = $"You lost!";
        Lose.SetActive(true);
        turns.SetActive(false);
        status.SetActive(false);
        InGameUIDisplay.SetActive(false);
        Client.Instance.ShutDown();
        test.GetComponent<Map>().enabled = false; //toggle this script to re-invoke it
        test.GetComponent<Map>().enabled = true;
    }

    public void ExperienceWinner(string message) {
        ExperienceWinText.text = message;
        ExperienceWin.SetActive(true);
        turns.SetActive(false);
        status.SetActive(false);
        InGameUIDisplay.SetActive(false);
        Client.Instance.ShutDown();
        test.GetComponent<Map>().enabled = false; //toggle this script to re-invoke it
        test.GetComponent<Map>().enabled = true;

    }

    public void InGameUI(Characters c) {

        if (c.type == characterType.BlackDrone || c.type == characterType.RedDrone) {
            InGameUnitType.text = "Drone";
        }
        else if (c.type == characterType.BlackWarrior || c.type == characterType.RedWarrior) {
            InGameUnitType.text = "Warrior";
        }
       else if (c.type == characterType.Queen) {
            InGameUnitType.text = "Queen";
        }

        InGameUnitHealth.text = $"{c.GetHealth()}";
        InGameUnitAttack.text = $"{c.GetAttack()}";
        InGameUnitDefense.text = $"{c.GetDefense()}";

        if (c.type == characterType.BlackDrone  && Client.Instance.getCurrentTeam() == 0) {
            //DisplayUnit.texture = units[1];
            vp.clip = vids[3];

            vp.Play();

        }

        else if (c.type == characterType.RedDrone && Client.Instance.getCurrentTeam() == 1) {
            // DisplayUnit.texture = units[0];
            vp.clip = vids[1];

            vp.Play();


        }

        else if (c.type == characterType.Queen && Client.Instance.getCurrentTeam() == 0) {
           // DisplayUnit.texture = units[2];
        }

        else if (c.type == characterType.Queen && Client.Instance.getCurrentTeam() == 1) {
           // DisplayUnit.texture = units[3];
        }

        else if (c.type == characterType.BlackWarrior && Client.Instance.getCurrentTeam() == 0) {
            // DisplayUnit.texture = units[4];
            vp.clip = vids[2];

            vp.Play();
        }

        else if (c.type == characterType.RedWarrior && Client.Instance.getCurrentTeam() == 1) {
            //DisplayUnit.texture = units[5];
            vp.clip = vids[0];

            vp.Play();
            
        }
    }

}
