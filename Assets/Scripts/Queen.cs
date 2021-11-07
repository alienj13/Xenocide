using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Characters

{
    private List<Vector2Int> mV = new List<Vector2Int>();
    public override void SetHealth() {
        health = 100;
    }
    public override bool ValidMove(Characters[,] board, int x1, int y1, int x2, int y2, Characters c) {
        //no valid moves
        return false;
    }
    public override void attack(Characters opponent, Characters player) {

        opponent.health -= 5;
        
        
    }

    public override List<Vector2Int> setMoves(int x, int y) {
        mV.Add(new Vector2Int(x , y ));
        return mV;
    }
}
