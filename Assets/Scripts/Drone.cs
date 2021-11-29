using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Characters
{
    public override void SetAttributes() {
        health = 20;
        AttackPower = 10;
    }
    public override bool ValidMove(Characters[,] board, int x1, int y1, int x2, int y2, Characters c) {
        moves.Clear();
        setMoves(this.currentX, this.currentY);
        Destination = new Vector2Int(x2, y2);
        foreach(Vector2Int i in moves) {
            if (Destination == i) {
                if (board[x2, y2] != null) {
                    if (board[x2, y2].team != c.team) {
                        Characters opponent = board[x2, y2];
                        attack(opponent, c);
                        if (death(board, opponent, x2, y2) == true) {
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                else {
                    return true;
                }
            }
        }
        return false;
    }

    public override void attack(Characters opponent, Characters player) {

        opponent.HealthLoss(AttackPower);      
        HasAttcked = true;
    }
    public bool death(Characters[,] board, Characters opponent, int x2, int y2) {
        if (opponent.GetHealth() < 1) {
            Destroy(opponent.gameObject);
            board[x2, y2] = null;
            HasKilled = true;
            return true;
        }
        return false;
    }

    public override List<Vector2Int> setMoves(int x, int y) {
        if (x + 1 <= 9) {
            moves.Add(new Vector2Int(x + 1, y));//right
        }
        if (x - 1 >= 0) {
            moves.Add(new Vector2Int(x - 1, y));//left
        }
        if (y + 1 <= 9) {
            moves.Add(new Vector2Int(x, y + 1));//up
        }
        if (y - 1 >= 0) {
            moves.Add(new Vector2Int(x, y - 1));//down
        }
        if (x + 1 <= 9 && y + 1 <= 9) {
            moves.Add(new Vector2Int(x + 1, y + 1));//diag up right
        }
        if (x - 1 >= 0 && y - 1 >= 0) {
            moves.Add(new Vector2Int(x - 1, y - 1));//diag down left
        }
        if (x + 1 <= 9 && y - 1 >= 0) {
            moves.Add(new Vector2Int(x + 1, y - 1));//diag down right
        }
        if (x - 1 >= 0 && y + 1 <= 9) {
            moves.Add(new Vector2Int(x - 1, y + 1));//diag up left
        }
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
