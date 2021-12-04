using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTest : Unit
{
    private int moveRange = 3;
    // TODO: Improve this
    private int attackRangeMin = 4;
    private int attackRangeMax = 6;

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        ClearMoves();
        AddMoves(PatternGen.DiamondMove(this, moveRange));
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        ClearAttacks();
        // TODO: Improve this
        AddAttacks(PatternGen.DiamondAttack(this, attackRangeMax));
        RemoveAttacks(PatternGen.DiamondAttack(this, attackRangeMin - 1));
        return availableAttacks;
    }
}
