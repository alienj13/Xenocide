using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class XPlayer
{
    public PlayerName team { get; set; }
    public Field field { get; set; }
    public List<Unit> activeUnits { get; private set; }

    public XPlayer(PlayerName team, Field field)
    {
        this.team = team;
        this.field = field;
        activeUnits = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        if (!activeUnits.Contains(unit))
            activeUnits.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (activeUnits.Contains(unit))
            activeUnits.Remove(unit);
    }

    public void GenerateAllPosibleMoves()
    {
        foreach (var unit in activeUnits)
        {
            if (field.HasUnit(unit))
                unit.SelectAvailableSquares();
        }
    }

    public int GetNumberOfAvailableUnits()
    {
        return activeUnits.Count;
    }

    public Unit[] GetUnitsAttackingOppositeUnitOfType<T>() where T : Unit
    {
        return activeUnits.Where(p => p.IsAttackingUnitOfType<T>()).ToArray();
    }

    public Unit[] GetUnitsOfType<T>() where T : Unit
    {
        return activeUnits.Where(p => p is T).ToArray();
    }

    public void RemoveMovesEnablingAttackOnUnit<T>(XPlayer opponent, Unit selectedUnit) where T : Unit
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>();
        foreach (var coords in selectedUnit.availableMoves)
        {
            Unit unitOnSquare = field.GetUnitOnSquare(coords);
            field.UpdateFieldOnUnitMove(coords, selectedUnit.occupiedSquare, selectedUnit, null);
            opponent.GenerateAllPosibleMoves();
            if (opponent.CheckIfIsAttackingUnit<T>())
                coordsToRemove.Add(coords);
            field.UpdateFieldOnUnitMove(selectedUnit.occupiedSquare, coords, selectedUnit, unitOnSquare);
        }
        foreach (var coords in coordsToRemove)
            selectedUnit.availableMoves.Remove(coords);
    }

    public void OnGameRestarted()
    {
        activeUnits.Clear();
    }

    private bool CheckIfIsAttackingUnit<T>() where T : Unit
    {
        foreach (var unit in activeUnits)
        {
            if (field.HasUnit(unit) && unit.IsAttackingUnitOfType<T>())
                return true;
        }
        return false;
    }

    public bool CanHideUnitFromAttack<T>(XPlayer opponent) where T : Unit
    {
        foreach (var unit in activeUnits)
        {
            foreach (var coords in unit.availableMoves)
            {
                Unit unitOnCoords = field.GetUnitOnSquare(coords);
                field.UpdateFieldOnUnitMove(coords, unit.occupiedSquare, unit, null);
                opponent.GenerateAllPosibleMoves();
                if (!opponent.CheckIfIsAttackingUnit<T>())
                {
                    field.UpdateFieldOnUnitMove(unit.occupiedSquare, coords, unit, unitOnCoords);
                    return true;
                }
                field.UpdateFieldOnUnitMove(unit.occupiedSquare, coords, unit, unitOnCoords);
            }
        }
        return false;
    }
}
