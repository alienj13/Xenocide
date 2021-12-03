using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGen
{
    public static HashSet<Vector2Int> LineMove(Unit selectedUnit, Vector2Int[] directions, int range)
    {
        HashSet<Vector2Int> results = new HashSet<Vector2Int>();
        Field field = selectedUnit.Field;
        Vector2Int origin = selectedUnit.OccupiedSquare;

        foreach (var direction in directions)
        {
            for (int i = 1; i <= range; i++)
            {
                Vector2Int nextCoords = origin + direction * i;
                Unit unit = field.GetUnitOnSquare(nextCoords);

                if (!field.CheckIfCoordsAreOnField(nextCoords))
                    break;

                if (unit == null)
                    results.Add(nextCoords);
                else
                    break;
            }
        }

        return results;
    }

    public static HashSet<Vector2Int> SquareMove(Unit selectedUnit, int range)
    {
        HashSet<Vector2Int> results = new HashSet<Vector2Int>();
        Field field = selectedUnit.Field;
        Vector2Int origin = selectedUnit.OccupiedSquare;

        int xCoord = origin.x;
        int yCoord = origin.y;
        
        for (int i = (xCoord - range); i <= (xCoord + range); i++)
        {
            for (int j = (yCoord - range); j <= (yCoord + range); j++)
            {
                Vector2Int nextCoords = new Vector2Int(i, j);
                Unit unit = field.GetUnitOnSquare(nextCoords);

                if (i == xCoord && j == yCoord)
                    continue;
                if (!field.CheckIfCoordsAreOnField(nextCoords))
                    continue;

                if (unit == null)
                    results.Add(nextCoords);
            }
        }

        return results;
    }

    public static HashSet<Vector2Int> DiamondMove(Unit selectedUnit, int range)
    {
        HashSet<Vector2Int> results = new HashSet<Vector2Int>();
        Field field = selectedUnit.Field;
        Vector2Int origin = selectedUnit.OccupiedSquare;

        int xCoord = origin.x;
        int yCoord = origin.y;

        for (int i = (xCoord - range); i <= (xCoord + range); i++)
        {
            int k = Mathf.Abs(Mathf.Abs(i - xCoord) - range);
            for (int j = (yCoord - k); j <= (yCoord + k); j++)
            {
                Vector2Int nextCoords = new Vector2Int(i, j);
                Unit unit = field.GetUnitOnSquare(nextCoords);

                if (i == xCoord && j == yCoord)
                    continue;
                if (!field.CheckIfCoordsAreOnField(nextCoords))
                    continue;

                if (unit == null)
                    results.Add(nextCoords);
            }
        }

        return results;
    }
}
