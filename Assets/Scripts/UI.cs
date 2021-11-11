using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject OnlineMenu;
    public GameObject Waiting;
    public GameObject DisconnectedPlayer;
    public static UI Instance { set; get; }

    public Server server;
    public Client client;

    [SerializeField] private TMP_InputField address;
    [SerializeField] public TMP_InputField Playername;
    [SerializeField] private Button playButton;

    private void Awake() {
        Instance = this;
        startMenu.SetActive(true);
        OnlineMenu.SetActive(false);
        Waiting.SetActive(false);
        

       
    }
    public void Update() {
        playButton.enabled = !string.IsNullOrWhiteSpace(Playername.text);
    }
    public void play() {
        startMenu.SetActive(false);
        OnlineMenu.SetActive(true);
     
       
    }


    public void back() {
        startMenu.SetActive(true);
        OnlineMenu.SetActive(false);
        client.ShutDown();
    }

    public void HostOnline() {
        server.initialize(8007);
        client.initialize("127.0.0.1", 8007,Playername.text);
        Waiting.SetActive(true);
        OnlineMenu.SetActive(false);

    }

    public void ConnectAsClient() {
        client.initialize(address.text, 8007,Playername.text);
        Debug.Log("attempt connection...");
    }

    public void backToOnlineMenu() {
        OnlineMenu.SetActive(true);
        Waiting.SetActive(false);
        server.ShutDown();
        client.ShutDown();
    }

    public void ExitFromDisconnect() {
        DisconnectedPlayer.SetActive(false);
        startMenu.SetActive(true);
        client.ShutDown();

    }

}
