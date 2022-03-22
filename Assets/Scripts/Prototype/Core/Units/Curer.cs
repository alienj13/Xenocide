using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curer : Unit
{
    private int moveRange = 5;
    private int attackRange = 2;

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        AddMoves(PatternGen.DiamondMove(this, moveRange));
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        AddAttacks(PatternGen.DiamondAttack(this, attackRange));
        return availableAttacks;
    }

    // Unique attack mechanic
    // When killing an enemy, recover 5 HP
    public override bool AttackAt(Vector2Int coords)
    {
        // TODO: IMPLEMENT THIS
        return base.AttackAt(coords);
    }
}
