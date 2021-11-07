using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum characterType {
    none = 0,
    Drone = 1,
    Warrior = 2,
    Queen = 3
}

public abstract class Characters : MonoBehaviour {

    public int team;
    public int currentX;
    public int currentY;
    public characterType type;
    public int health;
    public bool hasAttcked;

    


    public abstract bool ValidMove(Characters[,] board, int x1, int y1, int x2, int y2, Characters c);
    public abstract void attack(Characters opponent, Characters player);
    public abstract void SetHealth();
    public abstract void setMoves(int x, int y);



}
