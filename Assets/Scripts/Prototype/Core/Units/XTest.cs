using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTest : Unit
{
    public override HashSet<Vector2Int> GenerateAvailableAttacks()
    {
        throw new System.NotImplementedException();
    }

    public override HashSet<Vector2Int> GenerateAvailableMoves()
    {
        throw new System.NotImplementedException();
    }

    public override HashSet<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        return availableMoves;
    }
}
