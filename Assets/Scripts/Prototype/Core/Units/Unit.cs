using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
[RequireComponent(typeof(IObjectTweener))]
public abstract class Unit : MonoBehaviour
{
    private MaterialSetter materialSetter;
    private IObjectTweener tweener;

    public Field field { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    // team was changed from prop to field. No idea if that would break anything though.
    [SerializeField] public PlayerTeam team;
    public bool hasMoved { get; private set; }
    public List<Vector2Int> availableMoves;

    public abstract List<Vector2Int> SelectAvailableSquares();

    private void Awake()
    {
        materialSetter = GetComponent<MaterialSetter>();
        tweener = GetComponent<IObjectTweener>();

        availableMoves = new List<Vector2Int>();
        hasMoved = false;
    }

    public void SetMaterial(Material material)
    {
        if (materialSetter == null)
            materialSetter = GetComponent<MaterialSetter>();
        materialSetter.SetSingleMaterial(material);
    }

    public bool IsFromSameTeam(Unit unit)
    {
        return team == unit.team;
    }

    public bool CanMoveTo(Vector2Int coords)
    {
        return availableMoves.Contains(coords);
    }

    public bool IsAttackingUnitOfType<T>() where T : Unit
    {
        foreach (var square in availableMoves)
        {
            if (field.GetUnitOnSquare(square) is T)
                return true;
        }
        return false;
    }

    public virtual void MoveUnit(Vector2Int coords)
    {
        Vector3 targetPosition = field.CalculatePositionFromCoords(coords);
        occupiedSquare = coords;
        hasMoved = true;

        tweener.MoveTo(transform, targetPosition);
    }

    protected void TryToAddMove(Vector2Int coords)
    {
        availableMoves.Add(coords);
    }

    public void SetData(Vector2Int coords, PlayerTeam team, Field field)
    {
        this.occupiedSquare = coords;
        this.team = team;
        this.field = field;

        transform.position = field.CalculatePositionFromCoords(coords);
    }


    public void LineMovement(Vector2Int[] directions, float range)
    {
        foreach (var direction in directions)
        {
            for (int i = 1; i <= range; i++)
            {
                Vector2Int nextCoords = occupiedSquare + direction * i;
                Unit unit = field.GetUnitOnSquare(nextCoords);

                if (!field.CheckIfCoordsAreOnField(nextCoords))
                    break;

                if (unit == null)
                    TryToAddMove(nextCoords);
                else if (!unit.IsFromSameTeam(this))
                {
                    TryToAddMove(nextCoords);
                    break;
                }
                else if (unit.IsFromSameTeam(this))
                    break;
            }
        }
    }

    protected Unit GetUnitInDirection<T>(PlayerTeam team, Vector2Int direction) where T : Unit
    {
        for (int i = 1; i <= Field.FIELD_SIZE; i++)
        {
            Vector2Int nextCoords = occupiedSquare + direction * i;
            Unit unit = field.GetUnitOnSquare(nextCoords);

            if (!field.CheckIfCoordsAreOnField(nextCoords))
                return null;
            if (unit != null)
            {
                if (unit.team != team || !(unit is T))
                    return null;
                else if (unit.team == team && unit is T)
                    return unit;
            }
        }
        return null;
    }
}
