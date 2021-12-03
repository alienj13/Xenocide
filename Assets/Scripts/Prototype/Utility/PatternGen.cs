using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGen
{
    public static HashSet<Vector2Int> Line(Unit selectedUnit, Vector2Int[] directions, int range)
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();
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
                    result.Add(nextCoords);
                else
                    break;
            }
        }

        return result;
    }
}
