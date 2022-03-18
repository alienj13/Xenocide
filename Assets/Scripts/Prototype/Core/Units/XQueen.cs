using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XQueen : HitParticleEmitter
{
    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        return availableAttacks;
    }

    // When the Queen dies, do not destroy GameObject
    // This is for the GameController to end the game
    public override void Die(Unit source)
    {
        return;
    }
}
