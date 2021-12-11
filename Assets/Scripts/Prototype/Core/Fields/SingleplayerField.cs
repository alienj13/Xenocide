using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerField : Field
{
    public override void SetSelectedUnit(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSetSelectedUnit(intCoords);
    }

    public override void SelectedUnitMoved(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSelectedUnitMove(intCoords);
    }

    public override void SelectedUnitAttacked(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSelectedUnitAttack(intCoords);
    }
}
