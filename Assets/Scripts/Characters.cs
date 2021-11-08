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
    protected int currentX;
    protected int currentY;
    public characterType type;
    protected int health;
    protected int AttackPower;
    public bool HasAttcked;
    public bool HasKilled;
    protected Vector2Int Destination;
    protected List<Vector2Int> moves = new List<Vector2Int>();


    public abstract bool ValidMove(Characters[,] board, int x1, int y1, int x2, int y2, Characters c);
    public abstract void attack(Characters opponent, Characters player);
    public abstract void SetAttributes();
    public abstract List<Vector2Int> setMoves(int x, int y);

    public abstract int GetX();
    public abstract int GetY();
    public abstract void SetX(int x);
    public abstract void SetY(int y);
    public abstract int GetHealth();

    public abstract void HealthLoss(int h);

}
