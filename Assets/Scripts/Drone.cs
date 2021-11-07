using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Characters
{
    private Vector2Int m;
    private List<Vector2Int> mV = new List<Vector2Int>();

    public override void SetHealth() {
        health = 20;
        
    }
    public override bool ValidMove(Characters[,] board, int x1, int y1, int x2, int y2, Characters c) {
        mV.Clear();
        setMoves(this.currentX, this.currentY);
        m = new Vector2Int(x2, y2);
        if (board[x2, y2] != null)
        {
            if (board[x2, y2].team != c.team) {
                if ((x2 == x1 + 2 || x2 == x1 - 2 || y2 == y1 + 2 || y2 == y1 - 2) || x2 == x1 + 1 || (x2 == x1 - 1 || y2 == y1 + 1 || y2 == y1 - 1)) {
                    Characters opponent = board[x2, y2];
                    attack(opponent, c);
                    if (death(board,opponent,x2,y2) == true) {
                        
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        else if(board[x2, y2] == null) {
            foreach (Vector2Int i in mV) {
                if (m == i) {
                    this.currentX = x2;
                    this.currentY = y2;
                    return true;
                }
            }

        }
        return false;
    }

    public override void attack(Characters opponent, Characters player) {

        opponent.health -= 10;
        Debug.Log(opponent.team + " has been attacked. " + "Their health is " + opponent.health);
        hasAttacked = true;
    }
    public bool death(Characters[,] board, Characters opponent,int x2, int y2) {
        if (opponent.health < 1) {
            Destroy(opponent.gameObject);
            Debug.Log("Opponent destroyed");
            board[x2,y2] = null;
            hasKilled = true;
            return true;
        }
        return false;
    }

    public override List<Vector2Int> setMoves(int x, int y) {
        //if (x + 1 <=9) {
        //    mV.Add(new Vector2Int(x + 1, y));//right
        //}
        //if (x - 1 >= 0) {
        //    mV.Add(new Vector2Int(x - 1, y));//left
        //}
        //if (y + 1 <= 9) {
        //    mV.Add(new Vector2Int(x, y + 1));//up
        //}
        //if (y - 1 >= 0) {
        //    mV.Add(new Vector2Int(x, y - 1));//down
        //}
        //if (x + 1 <= 9 && y+1 <= 9) {
        //    mV.Add(new Vector2Int(x + 1, y + 1));//diag up right
        //}
        //if (x - 1 >= 0 && y-1 >= 0) {
        //    mV.Add(new Vector2Int(x - 1, y - 1));//diag down left
        //}
        //if (x + 1 <= 9 && y-1 >= 0) {
        //    mV.Add(new Vector2Int(x + 1, y - 1));//diag down right
        //}
        //if (x - 1 >= 0 && y + 1 <= 9) {
        //    mV.Add(new Vector2Int(x - 1, y + 1));//diag up left
        //}

        mV = PatternGen.square(x, y, 2);

        return mV;

    }

}
