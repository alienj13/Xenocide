using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum characterType{
    none = 0,
    Drone = 1,
    Warrior = 2,
    Queen = 3
}
 
public class Characters : MonoBehaviour
{
    
public int team;
public int currentX;
public int currentY;
public characterType type;

private Vector3 TargetPositon;
private Vector3 TargetScale;



}