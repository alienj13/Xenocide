using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDrone : Unit
{
    private int moveRange = 5;
    private int attackRange = 1;

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        ClearMoves();
        AddMoves(PatternGen.SquareMove(this, moveRange));
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        ClearAttacks();
        AddAttacks(PatternGen.SquareAttack(this, attackRange));
        return availableAttacks;
    }
}
