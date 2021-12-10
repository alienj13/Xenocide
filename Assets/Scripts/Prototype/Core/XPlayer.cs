using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class XPlayer
{
    public PlayerTeam Team { get; set; }
    public Field Field { get; set; }
    public List<Unit> ActiveUnits { get; private set; }

    #region Initialize and Restart
    public XPlayer(PlayerTeam team, Field field)
    {
        this.Team = team;
        this.Field = field;
        ActiveUnits = new List<Unit>();
    }

    public void OnGameRestarted()
    {
        ActiveUnits.Clear();
    }
    #endregion

    #region Add and Remove Units
    public void AddUnit(Unit unit)
    {
        if (!ActiveUnits.Contains(unit))
            ActiveUnits.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (ActiveUnits.Contains(unit))
            ActiveUnits.Remove(unit);
    }
    #endregion

    #region Generate actions
    public void GenerateAllPossibleActions()
    {
        //GenerateAllPossibleMoves();
        //GenerateAllPossibleAttacks();
        foreach (var unit in ActiveUnits)
        {
            if (Field.HasUnit(unit))
                unit.GenerateActions();
        }
    }

    public void GenerateAllPossibleMoves()
    {
        foreach (var unit in ActiveUnits)
        {
            if (Field.HasUnit(unit))
                unit.GenerateAvailableMoves();
        }
    }

    public void GenerateAllPossibleAttacks()
    {
        foreach (var unit in ActiveUnits)
        {
            if (Field.HasUnit(unit))
                unit.GenerateAvailableAttacks();
        }
    }
    #endregion

    #region Turn system
    public void OnTurnStart()
    {
        foreach (var unit in ActiveUnits)
        {
            if (Field.HasUnit(unit))
                unit.StartTurn();
        }
        GenerateAllPossibleActions();
    }

    public void OnTurnEnd()
    {
        foreach (var unit in ActiveUnits)
        {
            if (Field.HasUnit(unit))
                unit.EndTurn();
        }
    }
    #endregion

    #region Utility
    public int GetNumberOfAvailableUnits()
    {
        return ActiveUnits.Count;
    }

    public Unit[] GetUnitsOfType<T>() where T : Unit
    {
        return ActiveUnits.Where(p => p is T).ToArray();
    }
    #endregion
}