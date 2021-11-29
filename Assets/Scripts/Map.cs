using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Map : MonoBehaviour {

    public static Map Instance { set; get; }

    [SerializeField] private Material m;               //material of the tiles 
    [SerializeField] private GameObject[] prefabs;     //array of 3D models
    [SerializeField] private Material[] teamMaterial;  //the colour of player 1 or 2;
    [SerializeField] private float tileSize = 1.0f;    
    [SerializeField] private float yoffset = 0.2f;     
    [SerializeField] private Vector3 center = Vector3.zero; 
   // [SerializeField] private GameObject victory; //victory ui
    [SerializeField] private Camera cam0;  //camera for player assigned to team 0
    [SerializeField] private Camera cam1;  //camera for player assigned to team 0

    List<Vector2Int> HighlightMoves = new List<Vector2Int>();//list to hold all the possible moves 
    private Characters[,] character;    //array to hold all characters on the board
    private Characters Player0Queen;    //access to player 1 queens health
    private Characters Player1Queen;    //access to player 1 queens health
    private const int XCount = 10;      // X size of the tilemap
    private const int YCount = 10;      // Y size of the tilemap
    private GameObject[,] tiles;        // array of all tiles
    private Camera c;                   //a variable to control the camera 
    private Vector2Int hover;           //co ordinates for where the mouse is relative to the camera
    private Vector3 bounds;             // positioning for tiles
    private Vector2Int mouseOver;       //mouse position
    private Vector2Int StartMove;       //startingco-ordinates for player movemnet 
    private Vector2Int EndMove;         //movement destination
    private bool IsTeam0Turn;           //player turns
    private int currentTeam = -1;       //the players team
    private FixedString128Bytes opponent;  //players opponent
    private float speed = 20;
    

    //calls our functions 
    private void Awake() {
        IsTeam0Turn = true;
        GenerateTiles(tileSize, XCount, YCount);
        RegisterEvents();
       
        //SpawnAll();
        //AllPosition();

    }

    //calls function every frame
    private void Update() {
        if (currentTeam == 0) {
            cam0.enabled = true;
            cam1.enabled = false;
            c = cam0;
        }
        else if (currentTeam == 1) {
            cam0.enabled = false;
            cam1.enabled = true;
            c = cam1;
        }
        else {
            c = Camera.main;
        }

        if (Input.GetKey(KeyCode.D)) {
            c.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A)) {
            c.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.S)) {
            c.transform.Translate(new Vector3(0,-0.15f, -speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.W)) {
            c.transform.Translate(new Vector3(0,0.15f, speed * Time.deltaTime));
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) { 
            c.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime*5));
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            c.transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime * 5));
        }

        //changes tiles depending on where mouse is positioned to highlight tiles
        RaycastHit info;
        Ray ray = c.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover","Highlight","EnemyTile"))) {
            Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);
            if (hover == -Vector2Int.one) {
                hover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            if (hover != hitPosition) {
                
                   if (MouseHighlight()) {
                    if (character[hover.x, hover.y] != null && character[hover.x, hover.y].team != currentTeam) {
                        tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("EnemyTile");
                    }
                    else {
                        tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("Highlight");
                    }
                }
                else {
                    tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("Tile");
                }
                
                hover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            

            mouseOver.x = (int)hitPosition.x;
            mouseOver.y = (int)hitPosition.y;
            int x = mouseOver.x;
            int y = mouseOver.y;

          //press left mouse button down 
            if (Input.GetMouseButtonDown(0)) {
                if (character[hitPosition.x, hitPosition.y] != null ) {
                    if ((character[hitPosition.x, hitPosition.y].team == 0 && IsTeam0Turn && currentTeam==0) ||
                        (character[hitPosition.x, hitPosition.y].team == 1 && !IsTeam0Turn && currentTeam == 1)) {
                        SelectPiece(x, y);
                        HighlightMoves = Client.Instance.getSelected().setMoves(Client.Instance.getSelected().GetX(), Client.Instance.getSelected().GetY());
                        HighlightMovesMethod();
                    }
                }
            }
            //release left mouse button 
            if (Input.GetMouseButtonUp(0)) {
                RemoveHighlightMoves();
                AttemptMove((int)StartMove.x, (int)StartMove.y, x, y);     
            }
        }
        else { 
            if (hover != -Vector2Int.one) {
                if (MouseHighlight()) {
                    if (character[hover.x, hover.y] != null && character[hover.x, hover.y].team != currentTeam) {
                        tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("EnemyTile");
                    }
                    else {
                        tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("Highlight");
                    }
                }
                else {
                    tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("Tile");
                }
                hover = -Vector2Int.one;
            }
            mouseOver.x = -1;
            mouseOver.y = -1;
        }

        Turn();

    }


    //try to move, checks if the move is valid, turn changes here 
    public void AttemptMove(int x1, int y1, int x2, int y2) {
        StartMove = new Vector2Int(x1, y1);
        EndMove = new Vector2Int(x2, y2);
        Client.Instance.selectedPiece = character[x1, y1];
        characterType other = characterType.Warrior;

        if (character[x2,y2] != null) { 
         other = character[x2, y2].type;
    }
        if (Client.Instance.getSelected() != null) {

            ToMakeMoveClient(x1, y1, x2, y2);

            if (StartMove == EndMove) {
                Debug.Log("1");
                Move(Client.Instance.getSelected(), x1, y1);
                Client.Instance.setSelected(null);
                RemoveHighlightMoves();
                StartMove = Vector2Int.zero;
                return;
            }

            else if (Client.Instance.getSelected().ValidMove(character, x1, y1, x2, y2, Client.Instance.getSelected())) {
                if (Client.Instance.getSelected().HasKilled) {
                    KilledStatus(Client.Instance.getSelected(), other);
                }
                character[x2, y2] = Client.Instance.getSelected();
                Debug.Log("2");
                Move(Client.Instance.getSelected(), x2, y2);
                
                character[x1, y1] = null;
                Client.Instance.getSelected().HasKilled = false;
                Client.Instance.getSelected().HasAttcked = false;
                if (Player0Queen.GetHealth() < 1) {

                    if (currentTeam == 0) {
                        Victory(opponent, Client.Instance.PlayerName);
                    }

                    else {
                        Victory(Client.Instance.PlayerName, opponent);
                    }
                }

                if (Player1Queen.GetHealth() < 1) {

                    if (currentTeam == 1) {
                
                        Victory(opponent, Client.Instance.PlayerName);
                    }

                    else {
                        Victory(Client.Instance.PlayerName, opponent);
                    }

                }
                IsTeam0Turn = !IsTeam0Turn;
                Client.Instance.setSelected(null);
                RemoveHighlightMoves();
                StartMove = Vector2Int.zero;
                
            }

            else if (Client.Instance.getSelected().HasAttcked == true || Client.Instance.getSelected().HasKilled == true) {
                AttackStatus(Client.Instance.getSelected(), other);
                Client.Instance.getSelected().HasAttcked = false;
                Client.Instance.getSelected().HasKilled = false;
                Client.Instance.setSelected(null);
                RemoveHighlightMoves();
                StartMove = Vector2Int.zero;
                IsTeam0Turn = !IsTeam0Turn;
            }
            
            else {
                Debug.Log("3");
                Move(Client.Instance.getSelected(), x1, y1);
                Client.Instance.setSelected(null);
                RemoveHighlightMoves();
                StartMove = Vector2Int.zero;
                return;
            }

        }
    }

    //ui display when a character is attacked
    public void AttackStatus(Characters selected, characterType target) {
       
        if (selected.HasAttcked && !selected.HasKilled) {
            if (currentTeam == 0) {
                if (IsTeam0Turn) {
                   UI.Instance.StatusText.text= $"{opponent}'s {target} has been attacked";
                }
                else {
                    UI.Instance.StatusText.text = $"{Client.Instance.PlayerName}'s {target} has been attacked";
                }
            }
            if (currentTeam == 1) {
                if (IsTeam0Turn) {
                    UI.Instance.StatusText.text = $"{Client.Instance.PlayerName}'s {target} has been attacked";

                }
                else {

                    UI.Instance.StatusText.text = $"{opponent}'s {target} has been attacked";
                }
            }
        } 
    }

    //ui display when a character is killed
 
    public void KilledStatus(Characters selected, characterType target) {
        
        if (selected.HasKilled) {
            if (currentTeam == 0) {
                if (IsTeam0Turn) {
                    UI.Instance.StatusText.text = $"{opponent}'s {target} has been killed";
                }
                else {
                    UI.Instance.StatusText.text = $"{Client.Instance.PlayerName}'s {target} has been killed";
                }
            }
            if (currentTeam == 1) {
                if (IsTeam0Turn) {
                    UI.Instance.StatusText.text = $"{Client.Instance.PlayerName}'s {target} has been killed";

                }
                else {

                    UI.Instance.StatusText.text = $"{opponent}'s {target} has been killed";
                }
            }

        }
    }


    //use single tile creator to create a grid of tiles
    public void GenerateTiles(float size, int xcount, int ycount) {//creates a grid of 20x20 tiles
        yoffset += transform.position.y;
        bounds = new Vector3((xcount / 2) * tileSize, 0, (xcount / 2) * tileSize) + center;
        tiles = new GameObject[xcount, ycount];
        for (int x = 0; x < xcount; x++) {
            for (int y = 0; y < ycount; y++) {
                tiles[x, y] = CreateSingleTile(size, x, y);
            }
        }
    }


    //create a single tile using 2 triangles
    public GameObject CreateSingleTile(float size, int x, int y) {//creates a single tile
        GameObject obj = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        obj.transform.parent = transform;
        Mesh mesh = new Mesh();
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>().material = m;
        Vector3[] vert = new Vector3[4];
        vert[0] = new Vector3(x * size, yoffset, y * size) - bounds;
        vert[1] = new Vector3(x * size, yoffset, (y + 1) * size) - bounds;
        vert[2] = new Vector3((x + 1) * size, yoffset, y * size) - bounds;
        vert[3] = new Vector3((x + 1) * size, yoffset, (y + 1) * size) - bounds;
        int[] t = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.vertices = vert;
        mesh.triangles = t;
        mesh.RecalculateNormals();
        obj.layer = LayerMask.NameToLayer("Tile");
        obj.AddComponent<BoxCollider>();
        return obj;
    }


    //Get co ords of a particular tile
    public Vector2Int GetTileIndex(GameObject hitInfo) {
        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {
                if (tiles[x, y] == hitInfo) {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one;    //out of bounds 
    }


    //uses the single character spawner to
    //spawns all the characters and places them in positions of the array
    public void SpawnAll() {
        character = new Characters[XCount, YCount];
        character[5, 0] = SpawnCharacter(characterType.Queen, 0);
        Player0Queen = character[5, 0];
        character[6, 0] = SpawnCharacter(characterType.Warrior, 0);
        character[4, 0] = SpawnCharacter(characterType.Warrior, 0);
        character[5, 1] = SpawnCharacter(characterType.Warrior, 0);
        character[5, 3] = SpawnCharacter(characterType.Drone, 0);
        character[3, 3] = SpawnCharacter(characterType.Drone, 0);
        character[7, 3] = SpawnCharacter(characterType.Drone, 0);

        character[5, 9] = SpawnCharacter(characterType.Queen, 1);
        Player1Queen = character[5, 9];
        character[6, 9] = SpawnCharacter(characterType.Warrior, 1);
        character[4, 9] = SpawnCharacter(characterType.Warrior, 1);
        character[5, 8] = SpawnCharacter(characterType.Warrior, 1);
        character[5, 4] = SpawnCharacter(characterType.Drone, 1);
        character[3, 6] = SpawnCharacter(characterType.Drone, 1);
        character[7, 6] = SpawnCharacter(characterType.Drone, 1);
    
    }


    //spawns a single 3D character, assigns their team and type
    public Characters SpawnCharacter( characterType type, int team) {
        Characters c;
        if (team == 0 && type == characterType.Drone) {
            c = Instantiate(prefabs[(int)type - 1], transform.position,
                transform.rotation * Quaternion.Euler(0f, 180f, 0f)).GetComponent<Characters>();
        }
        else {
             c = Instantiate(prefabs[(int)type - 1], transform).GetComponent<Characters>();
        }
        c.type = type;
        c.team = team;
        c.GetComponent<MeshRenderer>().material = teamMaterial[team];     
        c.SetAttributes();
        
        return c;
    }


    //uses the single position function to position all characters
    public void AllPosition() {
        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {
                if (character[x, y] != null) {
                    SinglePosition(x, y, true);
                }
            }
        }
    }


    //places a single character in a particular position on the board
    public void SinglePosition(int x, int y, bool force = false) {
        character[x, y].SetX(x);
        character[x, y].SetY(y);
        character[x, y].transform.position = GetTileCenter(x, y);

    }


    //get the middle of the tile, useful for character positioning
    public Vector3 GetTileCenter(int x, int y) {
        return new Vector3(x * tileSize, 1, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }


    //selects a particular character
    public void SelectPiece(int x, int y) {
        RemoveHighlightMoves();
        Characters c = character[x, y];
        if (c != null) {
            Client.Instance.selectedPiece = c;
            StartMove = mouseOver;
            Debug.Log(Client.Instance.getSelected().type);
        }
    }


    //moves the position of the character
    public void Move(Characters c, int x, int y) {


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
            Debug.Log("screw you team!!! ");
        }
        else if (x > c.GetX() && y > c.GetY()) {
            c.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
            Debug.Log("lower left");
        }


        character[x, y].SetX(x);
        character[x, y].SetY(y);

        character[x, y].transform.position = GetTileCenter(x,y);


    }


    //when a character is selected, all their possibe moves are highlighted in blue
    public void HighlightMovesMethod() {
        foreach (Vector2Int i in HighlightMoves) {
            if (character[i.x, i.y] != null && character[i.x, i.y].team != currentTeam) {
                tiles[i.x, i.y].layer = LayerMask.NameToLayer("EnemyTile");
            }
            else {
                tiles[i.x, i.y].layer = LayerMask.NameToLayer("Highlight");
            }
        }
    }


    //removes all the highlights
    public void RemoveHighlightMoves() {
        foreach (Vector2Int i in HighlightMoves) {
            tiles[i.x, i.y].layer = LayerMask.NameToLayer("Tile");
        }
        HighlightMoves.Clear();
    }


    //checks if the mouse is hovering over a possible move 
    public bool MouseHighlight() {
        foreach (Vector2Int i in HighlightMoves) {
            if (i.x == mouseOver.x && i.y == mouseOver.y) {
                return true;
            }
        }
        return false;
    }
    public void Exit() {
        UI.Instance.Victory.SetActive(false);
        UI.Instance.startMenu.SetActive(true);
        UI.Instance.turns.SetActive(false);
        UI.Instance.status.SetActive(false);
    }
    public void ResetMap() {

        UI.Instance.Victory.SetActive(false);
        Client.Instance.setSelected(null);

        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {
                if (character[x, y] != null) {
                    Destroy(character[x, y].gameObject);
                }
                character[x, y] = null;
            }
        }
        IsTeam0Turn = true;       
        SpawnAll();
        AllPosition();
    }
    public void Victory(FixedString128Bytes winner, FixedString128Bytes looser) {
        UI.Instance.WinnerLoser.text = $"{winner} has won, {looser} has lost";
        UI.Instance.Victory.SetActive(true);
        UI.Instance.turns.SetActive(false);
        UI.Instance.status.SetActive(false);
    }


    //multiplayer
    public void RegisterEvents() { 
        
        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGame;
        NetUtility.C_MAKE_MOVE+= OnMakeMoveClient;
        NetUtility.C_CLIENT_DISCONNECT += OnDisconnectClient;
        NetUtility.C_USERNAME += OnUserNameClient;
    }

    public void Turn() {
        if (currentTeam == 0) {
            if (IsTeam0Turn) {
                UI.Instance.PlayersTurn.text = $"{Client.Instance.PlayerName}'s turn";
            }
            else {
                UI.Instance.PlayersTurn.text = $"{opponent}'s turn";
            }
        }
        if (currentTeam == 1) {
            if (IsTeam0Turn) {
                UI.Instance.PlayersTurn.text = $"{opponent}'s turn";

            }
            else {
                
                UI.Instance.PlayersTurn.text = $"{Client.Instance.PlayerName}'s turn";
            }
        }
    }


    //client Net messages recieved from the server
    public void OnUserNameClient(NetMessage msg) {
        NetUserName un = msg as NetUserName;
        Debug.Log($"my opponent is {un.PlayerName}");
        opponent = un.PlayerName;
    }


    public void OnWelcomeClient(NetMessage msg) {
        NetWelcome nw = msg as NetWelcome;
        currentTeam = nw.AssignedTeam;
        Debug.Log($"my assigned team is  { nw.AssignedTeam} ");

        NetUserName un = new NetUserName();
        un.PlayerName = UI.Instance.Playername.text;
        Client.Instance.SendToServer(un);

    }

    public void OnMakeMoveClient(NetMessage msg) {
        NetMakeMove mm = msg as NetMakeMove;
        
        if (mm.team != currentTeam) {
            characterType other = characterType.Warrior;// could not make null so i made it warrior as a placeholder
            if (character[mm.TargetX,mm.TargetY] != null) {
                other = character[mm.TargetX, mm.TargetY].type;
            }
            Characters target = character[mm.currentX, mm.currentY];          
            if (target.ValidMove(character, mm.currentX, mm.currentY, mm.TargetX, mm.TargetY, target)) {
                if (target.HasKilled) {
                    KilledStatus(target, other);
                }
                character[mm.TargetX, mm.TargetY] = target;
                Move(target, mm.TargetX, mm.TargetY);
                
                character[mm.currentX, mm.currentY] = null;
                target.HasAttcked = false;
                target.HasKilled = false;
                if (Player0Queen.GetHealth() < 1) {
                    if (currentTeam == 0) {
                        
                        Victory(opponent, Client.Instance.PlayerName);
                    }
                    else {
                        Victory(Client.Instance.PlayerName, opponent);
                    }
                }
                if (Player1Queen.GetHealth() < 1) {
                    if (currentTeam == 1) {

                        Victory(opponent, Client.Instance.PlayerName);
                    }
                    else {
                        Victory(Client.Instance.PlayerName, opponent);
                    }

                }
                IsTeam0Turn = !IsTeam0Turn;
                target = null;
                Client.Instance.setSelected(null);
                RemoveHighlightMoves();
                StartMove = Vector2Int.zero;
            }
            else if (target.HasAttcked == true || target.HasKilled == true) {
                AttackStatus( target,  other);

               target.HasAttcked = false;
                target.HasKilled = false;
                Client.Instance.setSelected(null);
                target = null;
                RemoveHighlightMoves();
                StartMove = Vector2Int.zero;
                IsTeam0Turn = !IsTeam0Turn;
            }

        }
    }

    public void OnStartGame(NetMessage msg) {
        SpawnAll();
        AllPosition();
        UI.Instance.Waiting.SetActive(false);
        UI.Instance.OnlineMenu.SetActive(false);
        UI.Instance.turns.SetActive(true);
        UI.Instance.status.SetActive(true);
        UI.Instance.StatusText.text = "Game Started";
    }

    public void OnDisconnectClient(NetMessage msg) {
        UI.Instance.DisconnectedPlayer.SetActive(true);
    }

    public void ToMakeMoveClient(int x1, int y1, int x2, int y2) {
        NetMakeMove mm = new NetMakeMove();
        mm.currentX = x1;
        mm.currentY = y1;
        mm.TargetX = x2;
        mm.TargetY = y2;
        mm.team = currentTeam;
        Client.Instance.SendToServer(mm);
    }
}
