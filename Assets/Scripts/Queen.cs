using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Characters

{
    public override void SetAttributes() {
        health = 100;
        AttackPower = 10;
    }
    public override bool ValidMove(Characters[,] board, int x1, int y1, int x2, int y2, Characters c) {
        //no valid moves
        return false;
    }
    public override void attack(Characters opponent, Characters player) {
        //no attack
    }

    public override List<Vector2Int> setMoves(int x, int y) {
        moves.Add(new Vector2Int(x , y ));//queen cannot move
        return moves;
    }

    public override int GetX() {
        return currentX;
    }

    public override int GetY() {
        return currentY;
    }

    public override void SetX(int x) {
        currentX = x;
    }

    public override void SetY(int y) {
        currentY = y;
    }

    public override int GetHealth() {
        return health;
    }

    public override void HealthLoss(int h) {
        health -= h;
    }
}
