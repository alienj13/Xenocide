using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Unit
{
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
}
