using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    public static Client Instance { set; get; }

  
    public NetworkDriver driver;
    private NetworkConnection connection;
    private bool IsActive = false;
    public Action connectionDropped;
    public string PlayerName;
    public Characters selectedPiece;
    private int currentTeam = -1;
    public string Result;
    private FixedString128Bytes opponent;
    private Vector2Int StartMove;
    private Vector2Int EndMove;
    private bool IsTeam0Turn;
  

    private Characters Player0Queen;    //access to player 1 queens health
    private Characters Player1Queen;    //access to player 1 queens health
    List<Vector2Int> HighlightMoves = new List<Vector2Int>();
    private void Awake() {
        Instance = this;
        IsTeam0Turn = true;
        //Map.Instance.SpawnAll();
        //Map.Instance.AllPosition();
    }
    public void initialize(string ip ,ushort port,string name) {
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, port);
        connection = driver.Connect(endpoint);
        Debug.Log("Attempting to connect to server on " + endpoint.Address);
        IsActive = true;
        RegisterToEvent();
        PlayerName = name;

    }

    public void ShutDown() {
        UnRegisterToEvent();
        if (IsActive) {
    
            connection = default(NetworkConnection);
            connection.Disconnect(driver);
            driver.Dispose();
            currentTeam = -1;
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

    public bool getTurn() {
        return IsTeam0Turn;
    }

    public void setTurn(bool s) {
         IsTeam0Turn = s;
    }

    public void SetQueen0(Characters q) {
       
        Player0Queen = q;
 
    }

    public void SetQueen1(Characters q) {
       Player1Queen = q;
    }

    public Characters getQueen0() {
        return Player0Queen;
    }

    public Characters getQueen1() {
        return Player1Queen;
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
        try {

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
                    break;

                }
            }
        }
        catch (ObjectDisposedException o) {
            Debug.Log($"{o.Message}");
        }
    }

    public void SendToServer(NetMessage msg) {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    private void UnRegisterToEvent() {
        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
        NetUtility.C_CLIENT_DISCONNECT += OnDisconnectClient;
        NetUtility.C_START_GAME -= OnStartGame;
        NetUtility.C_USERNAME -= OnUserNameClient;
        NetUtility.C_MAKE_MOVE = null;// OnMakeMoveClient;
        NetUtility.C_LOSER -= OnLoserClient;
        NetUtility.C_WINNER -= OnWinnerClient;
    }
    
    public Characters getSelected() {
        return selectedPiece;
    }

    public void SelectPiece(int x, int y,Characters[,] character) {
        Map.Instance.RemoveHighlightMoves();
        Characters c = character[x, y];
        if (c != null) {
            selectedPiece = c;
            StartMove = Map.Instance.getMouseover();
            Debug.Log(selectedPiece.type);
            
        }
    }

    public void ViewUnit(Characters c) {
        if(c.team == currentTeam)
        UI.Instance.InGameUI(c);
    }

    public Vector2Int getStarMove() {
        return StartMove;
    }

    public void SetStartMove(Vector2Int s) {
         StartMove = s;
    }

    public Vector2Int getEndMove() {
        return EndMove;
    }

    public void SetEndMove(Vector2Int e) {
        EndMove = e;
    }

    public void setSelected(Characters c) {
        selectedPiece = c;
    }

    public int getCurrentTeam() {
        return currentTeam;
    }

    public FixedString128Bytes getOpponent() {
        return opponent;
    }

    public void Move(Characters c,int x, int y) {
        Map.Instance.GetCharacters(c.GetX(), c.GetY()).SetPosition(Map.Instance.GetTileCenter(x, y));
        Map.Instance.GetCharacters(x, y).SetX(x);
        Map.Instance.GetCharacters(x, y).SetY(y);
    }

    //ui display when a character is killed

    public void KilledStatus(Characters selected, characterType target) {

        if (selected.HasKilled) {
            if (currentTeam == 0) {
                if (IsTeam0Turn) {
                    UI.Instance.StatusText.text = $"{opponent}'s {target} has been killed";
                }
                else {
                    UI.Instance.StatusText.text = $"{PlayerName}'s {target} has been killed";
                }
            }
            if (currentTeam == 1) {
                if (IsTeam0Turn) {
                    UI.Instance.StatusText.text = $"{PlayerName}'s {target} has been killed";

                }
                else {

                    UI.Instance.StatusText.text = $"{opponent}'s {target} has been killed";
                }
            }

        }
    }

    public void rotate(Characters c, int x, int y) {
        if (x > c.GetX() && y == c.GetY()) {
            c.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            Debug.Log("right");

        }
        else if (x < c.GetX() && y == c.GetY()) {
            c.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            Debug.Log("left");
        }
        else if (y > c.GetY() && x == c.GetX()) {
            c.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            Debug.Log("up");
        }
        else if (y < c.GetY() && x == c.GetX()) {
            c.transform.rotation = Quaternion.Euler(0f, 360f, 0f);
            Debug.Log("down");
        }
        else if (x > c.GetX() && y > c.GetY()) {
            c.transform.rotation = Quaternion.Euler(0f, 225f, 0f);
            Debug.Log("upper right");
        }
        else if (x < c.GetX() && y > c.GetY()) {
            c.transform.rotation = Quaternion.Euler(0f, 135f, 0f);
            Debug.Log("upper left");
        }
        else if (x > c.GetX() && y < c.GetY()) {
            c.transform.rotation = Quaternion.Euler(0f, 315f, 0f);
            Debug.Log("lower right ");
        }
        else if (x < c.GetX() && y < c.GetY()) {
            c.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
            Debug.Log("lower left");
        }


    }

    public void AttemptMove(int x1, int y1, int x2, int y2) {
        Debug.Log($"1 -{IsTeam0Turn}");
        StartMove = new Vector2Int(x1, y1);
        EndMove= new Vector2Int(x2, y2);
        selectedPiece = Map.Instance.GetCharacters(x1, y1);
        characterType other = characterType.RedWarrior;

        if (Map.Instance.GetCharacters(x2,y2) != null) {
            other = Map.Instance.GetCharacters(x2, y2).type;
        }
        if (selectedPiece != null) {
           ToMakeMoveClient(x1, y1, x2, y2);

            if (StartMove == EndMove) {
                Move(selectedPiece, x1, y1);
                selectedPiece = null;
                Map.Instance.RemoveHighlightMoves();
                StartMove = Vector2Int.zero;
                return;
            }

            else {
                if (selectedPiece.ValidMove(Map.Instance.getBoard(), x1, y1, x2, y2, selectedPiece)) {
                    Debug.Log($"vallidmove -{IsTeam0Turn}");
                    if (selectedPiece.HasKilled) {
                        KilledStatus(selectedPiece, other);
                    }

                    Map.Instance.SetCharacters(x2, y2, selectedPiece);
                    rotate(selectedPiece, x2, y2);
                    Move(selectedPiece, x2, y2);
                    Map.Instance.SetCharacters(x1, y1, null);
                    selectedPiece.HasKilled = false;
                    selectedPiece.HasAttcked = false;

                    //send message to server about queens health     

                    Debug.Log($"before -{IsTeam0Turn}");
                    IsTeam0Turn = !IsTeam0Turn;
                    Debug.Log($"after -{IsTeam0Turn}");
                    selectedPiece = null;
                    Map.Instance.RemoveHighlightMoves();
                    StartMove = Vector2Int.zero;

                }

                else if (selectedPiece.HasAttcked == true || selectedPiece.HasKilled == true) {
                    Debug.Log($"attack -{IsTeam0Turn}");
                    AttackStatus(selectedPiece, other);
                    rotate(selectedPiece, x2, y2);
                    //send message to server about queens health     
                    if (currentTeam == 0) {
                        SendQueenHealth(Player1Queen.GetHealth());
                    }
                    else {
                        SendQueenHealth(Player0Queen.GetHealth());
                    }
                    selectedPiece.HasAttcked = false;
                    selectedPiece.HasKilled = false;
                    selectedPiece = null;
                    Map.Instance.RemoveHighlightMoves();
                    StartMove = Vector2Int.zero;
                    Debug.Log($"before -{IsTeam0Turn}");
                    IsTeam0Turn = !IsTeam0Turn;
                    Debug.Log($"after -{IsTeam0Turn}");
                }

                else {
                    Move(selectedPiece, x1, y1);
                    selectedPiece = null;
                    Map.Instance.RemoveHighlightMoves();
                    StartMove = Vector2Int.zero;
                    return;
                }
            }
        }
    }

    //ui display when a character is attacked
    public void AttackStatus(Characters selected, characterType target) {

        if (selected.HasAttcked && !selected.HasKilled) {
            if (currentTeam == 0) {
                if (IsTeam0Turn) {
                    UI.Instance.StatusText.text = $"{opponent}'s {target} has been attacked";
                }
                else {
                    UI.Instance.StatusText.text = $"{PlayerName}'s {target} has been attacked";
                }
            }
            if (currentTeam == 1) {
                if (IsTeam0Turn) {
                    UI.Instance.StatusText.text = $"{PlayerName}'s {target} has been attacked";

                }
                else {

                    UI.Instance.StatusText.text = $"{opponent}'s {target} has been attacked";
                }
            }
        }
    }



    //multiplayer
    private void RegisterToEvent() {
        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
        NetUtility.C_CLIENT_DISCONNECT += OnDisconnectClient;
        NetUtility.C_START_GAME += OnStartGame;
        NetUtility.C_USERNAME += OnUserNameClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
        NetUtility.C_LOSER += OnLoserClient;
        NetUtility.C_WINNER += OnWinnerClient;

    }

    private void OnWinnerClient(NetMessage msg) {
        NetWinner nw = msg as NetWinner;

        if (nw.experience == 0) {
            UI.Instance.ExperienceWinner($"You won! Rank up, Rank {nw.rank}");
        }
        else {
            UI.Instance.ExperienceWinner("You won! +50XP");
        }
        // Map.Instance.ResetMap();
        //connectionDropped?.Invoke();
        currentTeam = -1;
    }

    private void OnLoserClient(NetMessage msg) {
        Debug.Log("i lost");
        UI.Instance.LoseScreen();
        // Map.Instance.ResetMap();
        // connectionDropped?.Invoke();
        currentTeam = -1;
    }

    public void OnDisconnectClient(NetMessage msg) {
        UI.Instance.DisconnectedPlayer.SetActive(true);
    }

    public void OnMakeMoveClient(NetMessage msg) {

        

        NetMakeMove mm = msg as NetMakeMove;

        if (Map.Instance.GetCharacters(mm.currentX, mm.currentY) == null) {
            Debug.Log("it FOOKIN NULL");
        
        }

            Debug.Log($"initial check - {IsTeam0Turn}{Map.Instance.GetCharacters(mm.currentX, mm.currentY)}{mm.team}");

        if (currentTeam == 0 && IsTeam0Turn) {
            Debug.Log($"team 0 check - {IsTeam0Turn}{currentTeam}");
            IsTeam0Turn = !IsTeam0Turn;
            currentTeam = 0;
            Debug.Log($"team 0 check - {IsTeam0Turn}{currentTeam}");

        }
        else if(currentTeam == 1 && !IsTeam0Turn) {
            Debug.Log($"team 1 check - {IsTeam0Turn}{currentTeam}");
            IsTeam0Turn = !IsTeam0Turn;
            currentTeam = 1;
            Debug.Log($"team 1 check - {IsTeam0Turn}{currentTeam}");
        }

        Debug.Log($"after initial check - {IsTeam0Turn}");

        if (mm.team != currentTeam ) {

            
            characterType other = characterType.BlackWarrior;// could not make null so i made it warrior as a placeholder
            if (Map.Instance.GetCharacters(mm.TargetX, mm.TargetY) != null) {
           
                other = Map.Instance.GetCharacters(mm.TargetX, mm.TargetY).type;
            }
            Characters target = Map.Instance.GetCharacters(mm.currentX, mm.currentY);
         
            if (target.ValidMove(Map.Instance.getBoard(), mm.currentX, mm.currentY, mm.TargetX, mm.TargetY, target)) {
                Debug.Log("valid");

                if (target.HasKilled) {
                    KilledStatus(target, other);
                }
                Map.Instance.SetCharacters(mm.TargetX,mm.TargetY, target);
      
                rotate(target, mm.TargetX, mm.TargetY);
         
                Move(target, mm.TargetX, mm.TargetY);
        
                Map.Instance.SetCharacters(mm.currentX, mm.currentY, null);
                target.HasAttcked = false;
                target.HasKilled = false;

                IsTeam0Turn = !IsTeam0Turn;
    
                target = null;
                selectedPiece = null;
                Map.Instance.RemoveHighlightMoves();
                StartMove =  Vector2Int.zero;
            }
            else if (target.HasAttcked == true || target.HasKilled == true) {

                Debug.Log("attack");
                AttackStatus(target, other);
               
                rotate(target, mm.TargetX, mm.TargetY);
         
                target.HasAttcked = false;
            
                target.HasKilled = false;
              
                selectedPiece = null;
              
                target = null;
              
                Map.Instance.RemoveHighlightMoves();
              
                StartMove =  Vector2Int.zero;
     
                IsTeam0Turn = !IsTeam0Turn;
    
            
            }

        }
        else{ Debug.Log("something is wrong"); }
    }

    public void OnStartGame(NetMessage msg) {
        IsTeam0Turn = true;
        UI.Instance.Waiting.SetActive(false);
        UI.Instance.OnlineMenu.SetActive(false);
        UI.Instance.turns.SetActive(true);
        UI.Instance.status.SetActive(true);
        UI.Instance.StatusText.text = "Game Started";
        UI.Instance.InGameUIDisplay.SetActive(true);
        Map.Instance.SpawnAll();
        if (currentTeam == 0) {
            Map.Instance.c.transform.position = new Vector3(-13, 12.8f, -36);
        }
        else {
            Map.Instance.c.transform.position = new Vector3(-14, 12.8f,4);
        }
    }

    public void OnWelcomeClient(NetMessage msg) {
        NetWelcome nw = msg as NetWelcome;
        currentTeam = nw.AssignedTeam;
        Debug.Log($"my assigned team is  { nw.AssignedTeam} ");

        NetUserName un = new NetUserName();
        un.PlayerName = UI.Instance.Playername.text;
        SendToServer(un);


    }

    public void OnUserNameClient(NetMessage msg) {
        NetUserName un = msg as NetUserName;
        Debug.Log($"my opponent is {un.PlayerName}");
        opponent = un.PlayerName;
    }

    public void ToMakeMoveClient(int x1, int y1, int x2, int y2) {
        NetMakeMove mm = new NetMakeMove();
        mm.currentX = x1;
        mm.currentY = y1;
        mm.TargetX = x2;
        mm.TargetY = y2;
        mm.team = currentTeam;
       SendToServer(mm);
    }

    public void SendQueenHealth(int hp) {
        NetQueen qh = new NetQueen();
        qh.QueenHealth = hp;
        
        SendToServer(qh);
    }

    private void OnKeepAlive(NetMessage nm) {
        SendToServer(nm);

    }
   

}


