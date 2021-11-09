using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject OnlineMenu;
    [SerializeField] private GameObject Waiting;
    public static UI Instance { set; get; }

    public Server server;
    public Client client;

    [SerializeField] private TMP_InputField address;
    private void Awake() {
        Instance = this;
    }

    public void play() {
        startMenu.SetActive(false);
        OnlineMenu.SetActive(true);
    }


    public void back() {
        startMenu.SetActive(true);
        OnlineMenu.SetActive(false);
    }

    public void HostOnline() {
        server.initialize(8007);
        client.initialize("127.0.0.1", 8007);
        Waiting.SetActive(true);
        OnlineMenu.SetActive(false);

    }

    public void ConnectAsClient() {
        client.initialize(address.text, 8007);
        Debug.Log("attempt connection...");
    }

    public void backToOnlineMenu() {
        OnlineMenu.SetActive(true);
        Waiting.SetActive(false);
        server.ShutDown();
        client.ShutDown();
    }

}
