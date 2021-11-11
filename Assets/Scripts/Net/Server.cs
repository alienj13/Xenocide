
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
    
    public static Server Instance { set; get; }
    
    private void Awake() {
        Instance = this;
        RegisterEvents();
    }

    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;
    private NativeList<NetworkConnection> room;
    private bool IsActive = false;
    private const float KeepAliveTickRate = 20.0f;
    private float LastKeepAlive;
    private int playerCount = -1;
    public Action connectionDropped;
    private string player0;
    private FixedString128Bytes player1;
   

    public void initialize(ushort port) {
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = port;
        if (driver.Bind(endpoint) != 0) {
            Debug.Log("Unable to bind on port " + endpoint.Port);
            return;
        }
        else {
            driver.Listen();
            Debug.Log("currently listening on port "+ endpoint.Port);

        }
        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        room = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        IsActive = true;
  
    }

    public void ShutDown() {
        if (IsActive) {
            driver.Dispose();
            connections.Dispose();
            IsActive = false;
        }
       // UnRegisterEvents();
    }

    public void OnDestroy() {
        ShutDown();
    }

    public void Update() {
        if (!IsActive) {
            return;
        }
        KeepAlive();
        driver.ScheduleUpdate().Complete();
        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }

    private void KeepAlive() {
        if (Time.time - LastKeepAlive> KeepAliveTickRate) {
            LastKeepAlive = Time.time;
            Broadcast(new NetKeepAlive());

        }
    }

    private void UpdateMessagePump() {
        DataStreamReader stream;
        for (int i = 0; i < connections.Length; i++) {
            NetworkEvent.Type cmd;
            while((cmd = driver.PopEventForConnection(connections[i],out stream)) != NetworkEvent.Type.Empty){
                if (cmd == NetworkEvent.Type.Data) {
                    NetUtility.OnData(stream, connections[i], this);
                }
                else if (cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                   
                        Broadcast(new NetDisconnect());
                    
                    //ShutDown();
                    break;
                }
            }
        }
    }

    private void AcceptNewConnections() {
        NetworkConnection c;
        while((c = driver.Accept()) != default(NetworkConnection)) {
            connections.Add(c);
        }
    }

    private void CleanupConnections() {
        for (int i = 0; i < connections.Length; i++) {
            if (!connections[i].IsCreated) {
                connections.RemoveAtSwapBack(i);
                i--;
            }
        }
    }

    public void SendToClient(NetworkConnection Connection, NetMessage msg) {
        DataStreamWriter writer;
        driver.BeginSend(Connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    public void Broadcast(NetMessage msg) {
        for (int i = 0; i < connections.Length; i++) {
            if (connections[i].IsCreated) {
                Debug.Log($"sending {msg.Code} tp : {connections[i].InternalId}");
                SendToClient(connections[i], msg);
            }
        }
    }

    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn) {
        NetMakeMove mm = msg as NetMakeMove;

       Broadcast(mm);
    }

    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn) {
        NetWelcome nw = msg as NetWelcome;
        room.Add(cnn);
       // nw.name = "k";
        //player1 = nw.name;
       // Debug.Log(player1);
        for (int i = 0; i< room.Length;i++) { 
            if(cnn == room[i]) {
                nw.AssignedTeam = i;
                
            }
        }

        ++playerCount;
        SendToClient(cnn, nw);
        if (playerCount == 1) {
            if (room[0] !=null && room[1] != null ) {
            NetStartGame ng = new NetStartGame();
            SendToClient(room[0],ng);
            SendToClient(room[1], ng);
            }
        }
    }
    private void RegisterEvents() {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.S_USERNAME += OnUserNameServer;
    }

    private void OnUserNameServer(NetMessage msg, NetworkConnection cnn) {
        Debug.Log("username");
    }

    private void UnRegisterEvents() {
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
        NetUtility.S_WELCOME -= OnWelcomeServer;
    }

    }
