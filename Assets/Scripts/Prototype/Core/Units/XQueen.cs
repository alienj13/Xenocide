using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XQueen : Unit
{
    private Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };
    private int range = 10;

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        ClearMoves();
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        ClearAttacks();
        return availableAttacks;
    }

    // TODO: Remove in future commit
    public override HashSet<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        LineMovement(directions, range);
        return availableMoves;
    }
}
