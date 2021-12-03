using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDrone : Unit
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
        TryToAddMoves(PatternGen.Line(this, directions, range));
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
        LineMovement(directions, range);
        return availableMoves;
    }
}
