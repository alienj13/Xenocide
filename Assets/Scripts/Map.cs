using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {


    [SerializeField] private Material m;               //material of the tiles 
    [SerializeField] private GameObject[] prefabs;     //array of 3D models
    [SerializeField] private Material[] teamMaterial;  //the colour of player 1 or 2;
    [SerializeField] private float tileSize = 1.0f;    //math stuff
    [SerializeField] private float yoffset = 0.2f;     //math stuff
    [SerializeField] private Vector3 center = Vector3.zero; //math stuff
    List<Vector2Int> HighlightMoves = new List<Vector2Int>();//list to hold all the possible moves 

    private Characters[,] character;    //array to hold all characters on the board
    private Characters selected;        //which char is selected 
    private Characters Player1Queen;    //access to player 1 queens health
    private Characters Player2Queen;    //access to player 1 queens health
    private const int XCount = 10;      // X size of the tilemap
    private const int YCount = 10;      // Y size of the tilemap
    private GameObject[,] tiles;        // array of all tiles
    private Camera c;                   //a variable to control the camera 
    private Vector2Int hover;           //co ordinates for where the mouse is relative to the camera
    private Vector3 bounds;             // more math stuff
    private Vector2Int mouseOver;       //mouse position
    private Vector2Int StartMove;       //startingco-ordinates for player movemnet 
    private Vector2Int EndMove;         //movement destination
    private bool IsTeam0Turn;           //player turns

    //calls our functions 
    private void Awake() {
        IsTeam0Turn = true;
        generateTiles(tileSize, XCount, YCount);
        Spawnall();
        allPosition();
    }

    //calls function every frame
    private void Update() {
        if (!c) {
            c = Camera.main;
            return;
        }
        //changes tiles depending on where mouse is positioned to highlight tiles
        RaycastHit info;
        Ray ray = c.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover","Highlight"))) {
            Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);
            if (hover == -Vector2Int.one) {
                hover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            if (hover != hitPosition) {
                
                   if (MouseHighlight()) {
                    tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("Highlight");
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
                    if ((character[hitPosition.x, hitPosition.y].team == 0 && IsTeam0Turn) || (character[hitPosition.x, hitPosition.y].team == 1 && !IsTeam0Turn)) {
                        SelectPiece(x, y);
                        HighlightMoves = selected.setMoves(selected.GetX(),selected.GetY());
                        highlightmoves();
                      
                    }
                }
            }
            //release left mouse button 
            if (Input.GetMouseButtonUp(0)) {
                Removehighlightmoves();
                attemptMove((int)StartMove.x, (int)StartMove.y, x, y);
             
            }
        }
        else { 
            if (hover != -Vector2Int.one) {
                if (MouseHighlight()) {
                    tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("Highlight");
                }
                else {
                    tiles[hover.x, hover.y].layer = LayerMask.NameToLayer("Tile");
                }
              
                hover = -Vector2Int.one;
            }
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    
    //try to move, checks if the move is valid, turn changes here 
    private void attemptMove(int x1, int y1, int x2, int y2) {
        StartMove = new Vector2Int(x1, y1);
        EndMove = new Vector2Int(x2, y2);
        selected = character[x1, y1];

        if (selected != null) {
            if (StartMove == EndMove) {
                Move(selected, x1, y1);
                selected = null;
                Removehighlightmoves();
                StartMove = Vector2Int.zero;
                return;
            }

            else if (selected.ValidMove(character, x1, y1, x2, y2, selected) ) {
                character[x2, y2] = selected;
                character[x1, y1] = null;
                Move(selected, x2, y2);
                IsTeam0Turn = !IsTeam0Turn;
                selected = null;
                Removehighlightmoves();
                StartMove = Vector2Int.zero;
            }
            else if (selected.HasAttcked == true || selected.HasKilled == true) {
                    if (Player1Queen.GetHealth()<1) {
                    Debug.Log("Player 2 wins");
                }
                if (Player2Queen.GetHealth() < 1) {
                    Debug.Log("Player 1 wins");
                }
                selected.HasAttcked = false;
                    selected.HasKilled = false;
                    selected = null;
                    Removehighlightmoves();
                    StartMove = Vector2Int.zero;
                    IsTeam0Turn = !IsTeam0Turn;
            }
            
            else {
                Move(selected, x1, y1);
                selected = null;
                Removehighlightmoves();
                StartMove = Vector2Int.zero;
                return;
            }

        }
    }


    //use single tile creator to create a grid of tiles
    private void generateTiles(float size, int xcount, int ycount) {//creates a grid of 20x20 tiles
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
    private GameObject CreateSingleTile(float size, int x, int y) {//creates a single tile
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
    private Vector2Int GetTileIndex(GameObject hitInfo) {
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
    private void Spawnall() {
        character = new Characters[XCount, YCount];
        character[5, 0] = spawnCharacter(characterType.Queen, 0);
        Player1Queen = character[5, 0];
        character[6, 0] = spawnCharacter(characterType.Warrior, 0);
        character[4, 0] = spawnCharacter(characterType.Warrior, 0);
        character[5, 1] = spawnCharacter(characterType.Warrior, 0);
        character[5, 3] = spawnCharacter(characterType.Drone, 0);
        character[3, 3] = spawnCharacter(characterType.Drone, 0);
        character[7, 3] = spawnCharacter(characterType.Drone, 0);

        character[5, 9] = spawnCharacter(characterType.Queen, 1);
        Player2Queen = character[5, 9];
        character[6, 9] = spawnCharacter(characterType.Warrior, 1);
        character[4, 9] = spawnCharacter(characterType.Warrior, 1);
        character[5, 8] = spawnCharacter(characterType.Warrior, 1);
        character[5, 6] = spawnCharacter(characterType.Drone, 1);
        character[3, 6] = spawnCharacter(characterType.Drone, 1);
        character[7, 6] = spawnCharacter(characterType.Drone, 1);
    
    }


    //spawns a single 3D character, assigns their team and type
     private Characters spawnCharacter( characterType type, int team) {
        Characters c = Instantiate(prefabs[(int)type - 1], transform).GetComponent<Characters>();
        c.type = type;
        c.team = team;
        c.GetComponent<MeshRenderer>().material = teamMaterial[team];
        c.SetAttributes();
        return c;
    }


    //uses the single position function to position all characters
    private void allPosition() {
        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {
                if (character[x, y] != null) {
                    singlePosition(x, y, true);
                }
            }
        }
    }


    //places a character in a particular position on the board
    private void singlePosition(int x, int y, bool force = false) {
        character[x, y].SetX(x);
        character[x, y].SetY(y);
        character[x, y].transform.position = GetTileCenter(x, y);

    }


    //get the middle of the tile, useful for character positioning
    private Vector3 GetTileCenter(int x, int y) {
        return new Vector3(x * tileSize, 1, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }


    //selects a particular character
    private void SelectPiece(int x, int y) {
        Removehighlightmoves();
        Characters c = character[x, y];
        if (c != null) {
            selected = c;
            StartMove = mouseOver;
            Debug.Log(selected.type);
        }
    }


    //moves the position of the character
    private void Move(Characters c, int x, int y) {
        character[x, y].SetX(x);
        character[x, y].SetY(y);
        character[x, y].transform.position = GetTileCenter(x, y);
    }


    //when a character is selected, all their possibe moves are highlighted in blue
    private void highlightmoves() {
        foreach (Vector2Int i in HighlightMoves) {
            tiles[i.x, i.y].layer = LayerMask.NameToLayer("Highlight");
        }
    }


    //removes all the highlights
    private void Removehighlightmoves() {
        foreach (Vector2Int i in HighlightMoves) {
            tiles[i.x, i.y].layer = LayerMask.NameToLayer("Tile");
        }
        HighlightMoves.Clear();
    }


    //checks if the mouse is hovering over a possible move 
    private bool MouseHighlight() {
        foreach (Vector2Int i in HighlightMoves) {
            if (i.x == mouseOver.x && i.y == mouseOver.y) {
                return true;
            }
        }
        return false;
    }
}
