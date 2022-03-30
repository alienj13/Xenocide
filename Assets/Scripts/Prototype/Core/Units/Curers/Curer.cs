using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Curer : Unit
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
        if (base.AttackAt(coords))
        {
            Unit enemy = Field.GetUnitOnSquare(coords);
            // Temporary solution:
            // If enemy unit cannot be found after attack, then enemy is considered dead
            if (enemy == null)
            {
                Heal(5, this);
                UpdateUnitDetails();
            }

            return true;
        }
        return false;
    }

    // [] Temporary solution
    public override void MoveUnit(Vector2Int coords)
    {
        Vector3 offset;
        if (Team == PlayerTeam.P1)
            offset = new Vector3(1f, -2.3f, 2.5f);
        else
            offset = new Vector3(-1f, -2.3f, -2.5f);
        Vector3 targetPosition = Field.CalculatePositionFromCoords(coords) + offset;
        

        // [] Temporary solution
        targetPosition.y = transform.position.y;

        OccupiedSquare = coords;
        HasMoved = true;

        tweener.MoveTo(transform, targetPosition);

        SoundEffects.Instance.PlayMovement();
    }
}
