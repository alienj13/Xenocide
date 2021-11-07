using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {


    [SerializeField] private Material m;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yoffset = 0.2f;
    [SerializeField] private Vector3 center = Vector3.zero;
    List<Vector2Int> HighlightMoves = new List<Vector2Int>();
    private Characters[,] character;
    private Characters selected;
    private const int XCount = 10;
    private const int YCount = 10;
    private GameObject[,] tiles;
    private Camera c;
    private Vector2Int hover;
    private Vector3 bounds;
    private Vector2Int mouseOver;
    private Vector2Int StartMove;
    private Vector2Int EndMove;
    private bool IsTeam0Turn;

    private void Awake() {
        IsTeam0Turn = true;
        generateTiles(tileSize, XCount, YCount);//calls our functions 
        Spawnall();
        allPosition();
    }


    private void Update() {
        if (!c) {
            c = Camera.main;
            return;
        }
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
          
            if (Input.GetMouseButtonDown(0)) {
                if (character[hitPosition.x, hitPosition.y] != null ) {
                    if ((character[hitPosition.x, hitPosition.y].team == 0 && IsTeam0Turn) || (character[hitPosition.x, hitPosition.y].team == 1 && !IsTeam0Turn)) {
                        SelectPiece(x, y);
                        HighlightMoves = selected.setMoves(selected.currentX,selected.currentY);
                        highlightmoves();
                      
                    }
                }
            }
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

    private void highlightmoves() {
        foreach (Vector2Int i in HighlightMoves) {
            tiles[i.x, i.y].layer = LayerMask.NameToLayer("Highlight");
        }
        //HighlightMoves.Clear();
    }

    private void Removehighlightmoves() {
        foreach (Vector2Int i in HighlightMoves) {
            tiles[i.x, i.y].layer = LayerMask.NameToLayer("Tile");
        }
        HighlightMoves.Clear();
    }
    private bool MouseHighlight() {
        foreach (Vector2Int i in HighlightMoves) {
            if(i.x == mouseOver.x && i.y == mouseOver.y){
                return true;
            }
        }
    return false;
    }

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
            else if (selected.hasAttcked == true || selected.hasKilled == true) {
                    selected.hasAttcked = false;
                    selected.hasKilled = false;
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


    private Vector2Int GetTileIndex(GameObject hitInfo) {
        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {
                if (tiles[x, y] == hitInfo) {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one;
    }


    private void Spawnall() {
        character = new Characters[XCount, YCount];    
        character[5, 0] = spawnCharacter(characterType.Queen, 0);
        character[6, 0] = spawnCharacter(characterType.Warrior, 0);
        character[4, 0] = spawnCharacter(characterType.Warrior, 0);
        character[5, 1] = spawnCharacter(characterType.Warrior, 0);
        character[5, 3] = spawnCharacter(characterType.Drone, 0);
        character[3, 3] = spawnCharacter(characterType.Drone, 0);
        character[7, 3] = spawnCharacter(characterType.Drone, 0);

        character[5, 9] = spawnCharacter(characterType.Queen, 1);
        character[6, 9] = spawnCharacter(characterType.Warrior, 1);
        character[4, 9] = spawnCharacter(characterType.Warrior, 1);
        character[5, 8] = spawnCharacter(characterType.Warrior, 1);
        character[5, 6] = spawnCharacter(characterType.Drone, 1);
        character[3, 6] = spawnCharacter(characterType.Drone, 1);
        character[7, 6] = spawnCharacter(characterType.Drone, 1);
    }

    private Characters spawnCharacter(characterType type, int team) {
        Characters c = Instantiate(prefabs[(int)type - 1], transform).GetComponent<Characters>();
        c.type = type;
        c.team = team;
        c.GetComponent<MeshRenderer>().material = teamMaterial[team];
        c.SetHealth();
        return c;
    }


    private void allPosition() {
        for (int x = 0; x < XCount; x++) {
            for (int y = 0; y < YCount; y++) {
                if (character[x, y] != null) {
                    singlePosition(x, y, true);
                }
            }
        }
    }

    private void singlePosition(int x, int y, bool force = false) {
        character[x, y].currentX = x;
        character[x, y].currentY = y;
        character[x, y].transform.position = GetTileCenter(x, y);

    }

    private Vector3 GetTileCenter(int x, int y) {
        return new Vector3(x * tileSize, 1, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    private void SelectPiece(int x, int y) {
        Removehighlightmoves();
        Characters c = character[x, y];
        if (c != null) {
            selected = c;
            StartMove = mouseOver;
            Debug.Log(selected.type);
        }
    }

    private void Move(Characters c, int x, int y) {
        character[x, y].currentX = x;
        character[x, y].currentY = y;
        character[x, y].transform.position = GetTileCenter(x, y);
    }
}
