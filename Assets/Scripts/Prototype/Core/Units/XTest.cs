using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTest : HitParticleEmitter
{
    private Vector2Int[] directions =
    {
        Vector2Int.left,
        Vector2Int.right,
    };
    private int moveRange = 20;

    public override void StartTurn()
    {
        ActionCount = 3;
    }

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        AddMoves(PatternGen.LineMove(this, directions, moveRange));
        return availableMoves;
    }

    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        PlayerTeam oppositeTeam = (this.Team == PlayerTeam.P1) ? PlayerTeam.P2 : PlayerTeam.P1;
        Vector2Int direction = (this.Team == PlayerTeam.P1) ? Vector2Int.up : Vector2Int.down;
        // For opposite team unit
        Unit unit = GetUnitInDirection<Unit>(oppositeTeam, direction);
        if (unit)
            AddAttack(unit.OccupiedSquare);
        // For neutral units
        unit = GetUnitInDirection<Unit>(PlayerTeam.Neutral, direction);
        if (unit)
            AddAttack(unit.OccupiedSquare);
        return availableAttacks;
    }

    // Note: Copy this to a ranged unit in the future.

    //private int moveRange = 3;
    //// TODO: Improve this
    //private int attackRangeMin = 4;
    //private int attackRangeMax = 6;

    //public override HashSet<Vector2Int> GenerateAvailableMoves()
    //{
    //    ClearMoves();
    //    AddMoves(PatternGen.DiamondMove(this, moveRange));
    //    return availableMoves;
    //}

    //public override HashSet<Vector2Int> GenerateAvailableAttacks()
    //{
    //    ClearAttacks();
    //    // TODO: Improve this
    //    AddAttacks(PatternGen.DiamondAttack(this, attackRangeMax));
    //    RemoveAttacks(PatternGen.DiamondAttack(this, attackRangeMin - 1));
    //    return availableAttacks;
    //}
}
