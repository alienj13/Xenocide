using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum characterType {
    none = 0,
    Drone = 1,
    Warrior = 2,
    Queen = 3
}

public class Characters : MonoBehaviour {

    public int team;
    public int currentX;
    public int currentY;
    public characterType type;


    public bool ValidMove(Characters[,] board, int x1, int y1, int x2, int y2) {
        if (board[x2, y2] != null) {
            return false;
        }
        return true;
    }


}
