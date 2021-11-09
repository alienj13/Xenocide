using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Instance { set; get; }

    private void Awake() {
        Instance = this;
    }


    public NetworkDriver driver;
    private NetworkConnection connection;
    private bool IsActive = false;
    public Action connectionDropped;


    public void initialize(string ip ,ushort port) {
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, port);

        connection = driver.Connect(endpoint);

        Debug.Log("Attempting to connect to server on " + endpoint.Address);
        IsActive = true;
        RegisterToEvent();
    }

    public void ShutDown() {
        UnRegisterToEvent();
        if (IsActive) {
            driver.Dispose();
            connection = default(NetworkConnection);
            IsActive = false;
        }
    }

    public void OnDestroy() {
        ShutDown();
    }


    public void Update() {
        if (!IsActive) {
            return;
        }
        
        driver.ScheduleUpdate().Complete();
        CheckAlive();
        
        UpdateMessagePump();
    }

    private void CheckAlive() {
        if (!connection.IsCreated && IsActive) {
            Debug.Log("Lost connection to server");
            connectionDropped?.Invoke();
            ShutDown();
        }
    }

    private void UpdateMessagePump() {

        DataStreamReader stream;
        NetworkEvent.Type cmd;
            while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty) {

                if (cmd == NetworkEvent.Type.Connect) {
                SendToServer(new NetWelcome());
                Debug.Log("we're connected");
                }
                else if (cmd == NetworkEvent.Type.Data) {
                    NetUtility.OnData(stream, default(NetworkConnection));
                }
                else if (cmd == NetworkEvent.Type.Disconnect) {

                    Debug.Log("Client got disconnected from server");
                    connection = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    ShutDown();
                
                }
            }
    }

    public void SendToServer(NetMessage msg) {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    private void RegisterToEvent() { 
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
    }
    private void UnRegisterToEvent() {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage nm) {
        SendToServer(nm);
    }
}
