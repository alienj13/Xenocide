using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Field/Layout")]
public class FieldLayout : ScriptableObject
{
    [Serializable]
    private class FieldSquareSetup
    {
        public Vector2Int position;
        public UnitType unitType;
        public PlayerTeam playerName;
    }

    [SerializeField] private FieldSquareSetup[] fieldSquares;

    public int GetUnitsCount()
    {
        return fieldSquares.Length;
    }

    public Vector2Int GetSquareCoordsAtIndex(int index)
    {
        if (index < 0 || index >= fieldSquares.Length)
        {
            Debug.LogError("Index of piece (" + index + ") is out of bound.");
            return new Vector2Int(-1, -1);
        }
        Vector2Int unitPosition = fieldSquares[index].position;
        return new Vector2Int(unitPosition.x, unitPosition.y);
    }

    public String GetSquareUnitNameAtIndex(int index)
    {
        if (index < 0 || index >= fieldSquares.Length)
        {
            Debug.LogError("Index of piece (" + index + ") is out of bound.");
            return "";
        }
        return fieldSquares[index].unitType.ToString();
    }

    public PlayerTeam GetSquarePlayerNameAtIndex(int index)
    {
        if (index < 0 || index >= fieldSquares.Length)
        {
            Debug.LogError("Index of piece (" + index + ") is out of bound.");
            return PlayerTeam.P1;
        }
        return fieldSquares[index].playerName;
    }
}
