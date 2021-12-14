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
    [SerializeField] private float tileSize = 2.0f;    
    [SerializeField] private float yoffset = 0.2f;     
    [SerializeField] private Vector3 center = Vector3.zero; 
    [SerializeField] private Camera cam0;  //camera for player assigned to team 0
    [SerializeField] private Camera cam1;  //camera for player assigned to team 1

    List<Vector2Int> HighlightMoves = new List<Vector2Int>();//list to hold all the possible moves 
    private Characters[,] character;    //array to hold all characters on the board
    private const int XCount = 20;      // X size of the tilemap
    private const int YCount = 20;      // Y size of the tilemap
    private GameObject[,] tiles;        // array of all tiles
    public   Camera c;                   //a variable to control the camera 
    private Vector2Int hover;           //co ordinates for where the mouse is relative to the camera
    private Vector3 bounds;             // positioning for tiles
    private Vector2Int mouseOver;       //mouse position
    private float speed = 20;
    

    //calls our functions 
    public void Awake() {
        Instance = this;
      
        GenerateTiles(tileSize, XCount, YCount);
        character = new Characters[XCount, YCount];
        SpawnAll();
        //AllPosition();

    }

    //calls function every frame
    private void Update() {
        if (Client.Instance.getCurrentTeam() == 0) {
            cam0.enabled = true;
            cam1.enabled = false;
            c = cam0;
        }
        else if (Client.Instance.getCurrentTeam() == 1) {
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
            if (Client.Instance.getCurrentTeam() == 1) {
                c.transform.Translate(0, 0, speed * Time.deltaTime, Space.World);
            }
            else {
                c.transform.Translate(0, 0, -speed * Time.deltaTime, Space.World);
            }
        }
        if (Input.GetKey(KeyCode.W)) {
            if (Client.Instance.getCurrentTeam() == 1) {
                c.transform.Translate(0, 0, -speed * Time.deltaTime, Space.World);
            }
            else {
                c.transform.Translate(0, 0, speed * Time.deltaTime, Space.World);
            }
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
                    if (character[hover.x, hover.y] != null && character[hover.x, hover.y].team != Client.Instance.getCurrentTeam()) {
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
                    Client.Instance.ViewUnit(character[hitPosition.x, hitPosition.y]);
                    if ((character[hitPosition.x, hitPosition.y].team == 0 && Client.Instance.getTurn() && Client.Instance.getCurrentTeam() == 0) ||
                        (character[hitPosition.x, hitPosition.y].team == 1 && !Client.Instance.getTurn() && Client.Instance.getCurrentTeam() == 1)) {
                        Client.Instance.SelectPiece(x, y, character);
                        Client.Instance.ViewUnit(character[hitPosition.x, hitPosition.y]);
                        HighlightMoves = Client.Instance.getSelected().setMoves(Client.Instance.getSelected().GetX(), Client.Instance.getSelected().GetY());
                        HighlightMovesMethod();
                    }
                }
            }
            //release left mouse button 
            if (Input.GetMouseButtonUp(0)) {
                RemoveHighlightMoves();
                Client.Instance.AttemptMove((int)Client.Instance.getStarMove().x, (int)Client.Instance.getStarMove().y, x, y);     
            }
            
        }
        else { 
            if (hover != -Vector2Int.one) {
                if (MouseHighlight()) {
                    if (character[hover.x, hover.y] != null && character[hover.x, hover.y].team != Client.Instance.getCurrentTeam()) {
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

    public Vector2Int getMouseover() {
        return mouseOver;
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
  

        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {

                if (character[x, y] != null) {
                    Debug.Log($"destroy {character[x, y].GetType()} at {x}{y}");
                    Destroy(character[x, y].gameObject);
                    character[x, y] = null;
                }
               

            }
        }
 

        character = new Characters[XCount, YCount];


        character[6, 0] = SpawnCharacter(characterType.BlackWarrior, 0);
        character[4, 0] = SpawnCharacter(characterType.BlackWarrior, 0);
        character[5, 1] = SpawnCharacter(characterType.BlackWarrior, 0);
         character[5, 3] = SpawnCharacter(characterType.BlackDrone, 0);
         character[3, 3] = SpawnCharacter(characterType.BlackDrone, 0);
         character[7, 8] = SpawnCharacter(characterType.BlackDrone, 0);


        character[6, 9] = SpawnCharacter(characterType.RedWarrior, 1);
        character[4, 9] = SpawnCharacter(characterType.RedWarrior, 1);
  
  
        character[5, 8] = SpawnCharacter(characterType.RedWarrior, 1);
        character[5, 4] = SpawnCharacter(characterType.RedDrone, 1);
         character[3, 6] = SpawnCharacter(characterType.RedDrone, 1);
         character[7, 6] = SpawnCharacter(characterType.RedDrone, 1);


        character[5, 0] = SpawnCharacter(characterType.Queen, 0);
        character[5, 9] = SpawnCharacter(characterType.Queen, 1);
        Client.Instance.SetQueen1(character[5, 9]);
        Client.Instance.SetQueen0(character[5, 0]);


      
            for (int x = 0; x < XCount; x++) {
                for (int y = 0; y < YCount; y++) {
                    if (character[x, y] != null) {
                        SinglePosition(x, y, true);
                    }
                }
            }
        
    }


    //spawns a single 3D character, assigns their team and type
    public Characters SpawnCharacter( characterType type, int team) {
        Characters c;
        if (team == 0 && type != characterType.Queen) {
            c = Instantiate(prefabs[(int)type ], transform.position,
                transform.rotation * Quaternion.Euler(0f, 180f, 0f)).GetComponent<Characters>();
        }
        else if (type == characterType.Queen) {
            c = Instantiate(prefabs[(int)type], transform).GetComponent<Characters>();
            c.GetComponent<MeshRenderer>().material = teamMaterial[team]; 
        }
        else {
             c = Instantiate(prefabs[(int)type ], transform).GetComponent<Characters>();
        }
        c.type = type;
        c.team = team;
         
        c.SetAttributes();
        //c.SetX();
        
        return c;
    }


    //uses the single position function to position all characters
  


    //places a single character in a particular position on the board
    public void SinglePosition(int x, int y, bool force = false) {
        character[x, y].SetX(x);
        character[x, y].SetY(y);
        character[x, y].SetPosition (GetTileCenter(x, y),force);

    }


    //get the middle of the tile, useful for character positioning
    public Vector3 GetTileCenter(int x, int y) {
        return new Vector3(x * tileSize, 1, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }


    //when a character is selected, all their possibe moves are highlighted in blue
    public void HighlightMovesMethod() {
        foreach (Vector2Int i in HighlightMoves) {
            if (character[i.x, i.y] != null && character[i.x, i.y].team != Client.Instance.getCurrentTeam()) {
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
   
    public void ResetMap() {

 
        Client.Instance.setSelected(null);

        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {

                if (character[x, y] != null) {
                    Debug.Log($"destroy {character[x,y].GetType()} at {x}{y}");
                    Destroy(character[x, y].gameObject);
                }
                character[x, y] = null;
                
            }
        }
        Client.Instance.setTurn(true);
    }
   

    public Characters GetCharacters(int x, int y) {
        return character[x,y];
    }

    public void SetCharacters(int x, int y, Characters c) {
        character[x, y] = c;
    }
    public Characters[,] getBoard() {
        return character;
    }
    
    public void Turn() {
        if (Client.Instance.getCurrentTeam() == 0) {
            if (Client.Instance.getTurn()) {
                UI.Instance.PlayersTurn.text = $"{Client.Instance.PlayerName}'s turn";
            }
            else {
                UI.Instance.PlayersTurn.text = $"{Client.Instance.getOpponent()}'s turn";
            }
        }
        if (Client.Instance.getCurrentTeam() == 1) {
            if (Client.Instance.getTurn()) {
                UI.Instance.PlayersTurn.text = $"{Client.Instance.getOpponent()}'s turn";

            }
            else {
                
                UI.Instance.PlayersTurn.text = $"{Client.Instance.PlayerName}'s turn";
            }
        }
    }



   

}
