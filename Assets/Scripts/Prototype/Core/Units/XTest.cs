using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTest : Unit
{
    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        ClearMoves();
        AddMoves(PatternGen.DiamondMove(this, 4));
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        ClearAttacks();
        AddAttacks(PatternGen.DiamondAttack(this, 8));
        RemoveAttacks(PatternGen.DiamondAttack(this, 4));
        return availableAttacks;
    }

    // TODO: Remove in future commit
    public override HashSet<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        return availableMoves;
    }
}
