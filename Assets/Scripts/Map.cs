using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{


 [SerializeField] private Material m;
 [SerializeField] private GameObject[] prefabs;
 [SerializeField] private Material[] teamMaterial;
 [SerializeField] private float tileSize = 1.0f;
 [SerializeField] private float yoffset = 0.2f;
 [SerializeField] private Vector3 center  = Vector3.zero;
 private Characters[,] character;
 private const int XCount = 20;
 private const int YCount = 20;
 private GameObject[,] tiles;
 private Camera c;
 private Vector2Int hover;
 private Vector3 bounds;

 private void Awake(){
    generateTiles(tileSize, XCount, YCount);//calls our functions 
    //spawnCharacter(characterType.Queen, 1);
    Spawnall();
    allPosition();
   }


 private void Update(){
  if(!c){
     c = Camera.main;
     return;
  }
  RaycastHit info;
  Ray ray = c.ScreenPointToRay(Input.mousePosition);
  if(Physics.Raycast(ray,out info,100,LayerMask.GetMask("Tile","Hover"))){
   Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);
      if(hover == -Vector2Int.one){
          hover = hitPosition;
          tiles[hitPosition.x,hitPosition.y].layer = LayerMask.NameToLayer("Hover");
      }
     if(hover != hitPosition){
        tiles[hover.x,hover.y].layer = LayerMask.NameToLayer("Tile");
        hover = hitPosition;
        tiles[hitPosition.x,hitPosition.y].layer = LayerMask.NameToLayer("Hover");
      }
  }else if(hover != -Vector2Int.one){
       tiles[hover.x,hover.y].layer = LayerMask.NameToLayer("Tile");
       hover = -Vector2Int.one;
     }
   }
  


 private void generateTiles(float size, int xcount, int ycount){//creates a grid of 20x20 tiles
  yoffset+=transform.position.y;
  bounds = new Vector3((xcount/2) * tileSize,0,(xcount/2) * tileSize)+center;
  tiles = new GameObject[xcount,ycount];
  for(int x = 0;x< xcount; x++){           
    for(int y = 0;y< ycount; y++){
        tiles[x,y] = CreateSingleTile(size,x,y);
       }
    }
  }
   

 private GameObject CreateSingleTile(float size, int x , int y){//creates a single tile
     GameObject obj = new GameObject(string.Format("X:{0}, Y:{1}",x,y));
     obj.transform.parent = transform;

    Mesh mesh = new Mesh();
    obj.AddComponent<MeshFilter>().mesh = mesh;
    obj.AddComponent<MeshRenderer>().material = m;

    Vector3[] vert = new Vector3[4];

    vert[0] = new Vector3(x * size,yoffset,y*size)-bounds;
    vert[1] = new Vector3(x * size,yoffset,(y+1)*size)-bounds;
    vert[2] = new Vector3((x+1) * size,yoffset,y*size)-bounds;
    vert[3] = new Vector3((x+1) * size,yoffset,(y+1)*size)-bounds;

     int[] t = new int[]{0,1,2,1,3,2};
     mesh.vertices = vert;
     mesh.triangles = t;
     mesh.RecalculateNormals();

     obj.layer = LayerMask.NameToLayer("Tile");
     obj.AddComponent<BoxCollider>();

     return obj;
  }


 private Vector2Int GetTileIndex(GameObject hitInfo){
  for(int x = 0;x< XCount; x++){
     for(int y = 0;y< YCount; y++){
        if(tiles[x,y] == hitInfo){
             return new Vector2Int(x,y);
           }
        }
     }
     return -Vector2Int.one;
 }
   

private void Spawnall(){
  character = new Characters[XCount,YCount];
  for(int i = 0;i<XCount;i++){
  character[i,0] = spawnCharacter(characterType.Drone,0);
  character[i,19] = spawnCharacter(characterType.Queen,1);
  }
  // character[1,0] = spawnCharacter(characterType.Queen,0);
  // character[2,0] = spawnCharacter(characterType.Queen,0);
  // character[3,0] = spawnCharacter(characterType.Queen,0);
  // character[4,0] = spawnCharacter(characterType.Queen,0);
  // character[5,0] = spawnCharacter(characterType.Queen,0);
  // character[6,0] = spawnCharacter(characterType.Queen,0);
  // character[7,0] = spawnCharacter(characterType.Queen,0);

   
}

private Characters spawnCharacter(characterType type, int team){
  Characters c = Instantiate(prefabs[(int)type-1],transform).GetComponent<Characters>();
  c.type = type;
  c.team = team;
  c.GetComponent<MeshRenderer>().material = teamMaterial[team];
  return c;
}
private void singlePosition(int x, int y, bool force = false){
   character[x,y].currentX = x;
   character[x,y].currentY = y;
   character[x,y].transform.position = GetTileCenter(x,y);

}

private void allPosition(){
  for(int x = 0;x< XCount; x++){           
    for(int y = 0;y< YCount; y++){
      if(character[x,y] != null){
         singlePosition( x, y, true);

      }
    }
  }
} 

  private Vector3 GetTileCenter(int x, int y){

return new Vector3(x*tileSize,1,y*tileSize)-bounds + new Vector3(tileSize/2,0,tileSize/2);
  

}

}
