using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTest : Unit
{
    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        ClearMoves();
        AddAttacks(PatternGen.DiamondMove(this, 8));
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
        return availableMoves;
    }
}
