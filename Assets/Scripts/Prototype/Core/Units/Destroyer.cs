using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : Unit
{
    private int moveRange = 5;
    private int attackRange = 1;

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        AddMoves(PatternGen.CrossMove(this, moveRange));
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        AddAttacks(PatternGen.SquareAttack(this, attackRange));
        return availableAttacks;
    }
}
