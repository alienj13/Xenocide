using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Characters
{
   
    public override bool ValidMove(Characters[,] board, int x1, int y1, int x2, int y2, Characters c) {
        if (board[x2, y2] != null) {

            return false;
        }
        else {

            if (x2 == x1 + 2 || x2 == x1 - 2 || y2 == y1 + 2 || y2 == y1 - 2) {
                return true;
            }
            else if (x2 == x1 + 1 || x2 == x1 - 1 || y2 == y1 + 1 || y2 == y1 - 1) {
                return true;
            }
        }
        return false;
    }

}
