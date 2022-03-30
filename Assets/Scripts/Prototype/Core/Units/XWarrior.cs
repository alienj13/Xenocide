using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XWarrior : Unit
{
    private int moveRange = 8;
    private int attackRange = 3;

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
    
    protected override void PlayDamageSound()
    {
        SoundEffects.Instance.PlayWarriorHit();
    }

}
