using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Characters

{
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

    public override void setMoves(int x, int y) {
       //no moves
    }
}
