using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XWarrior : Unit
{
    private int moveRange = 6;
    private int attackRange = 2;

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        AddMoves(PatternGen.CrossMove(this, moveRange));
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        AddAttacks(PatternGen.CrossAttack(this, attackRange));
        return availableAttacks;
    }

}
