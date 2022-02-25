using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDrone : Unit
{
    private int moveRange = 5;
    private int attackRange = 1;

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        AddMoves(PatternGen.SquareMove(this, moveRange));
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        AddAttacks(PatternGen.SquareAttack(this, attackRange));
        return availableAttacks;
    }

    // [] Temporary solution
    public override void Die(Unit source)
    {
        Field.ShowExecutionAnimation(source, this);
        base.Die(source);
    }
}
