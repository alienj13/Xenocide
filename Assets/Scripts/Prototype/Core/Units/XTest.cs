using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTest : Unit
{
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        return availableMoves;
    }
}
