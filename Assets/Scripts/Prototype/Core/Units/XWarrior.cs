using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XWarrior : Unit
{
    private Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };
    private float range = 10;

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        throw new System.NotImplementedException();
    }

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        throw new System.NotImplementedException();
    }

    public override HashSet<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        LineMovement(directions, range);
        return availableMoves;
    }
}
