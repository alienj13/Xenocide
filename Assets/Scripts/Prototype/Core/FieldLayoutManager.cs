using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldLayoutManager : MonoBehaviour
{
    [SerializeField] private FieldLayout[] fieldLayouts_P1;
    [SerializeField] private FieldLayout[] fieldLayouts_P2;

    public List<FieldLayout> getPlayerLayouts(PlayerTeam team, int rank)
    {
        List<FieldLayout> result = new List<FieldLayout>();

        switch (team)
        {
            case PlayerTeam.P1:
                result.Add(fieldLayouts_P1[0]);
                if (rank >= 3)
                    result.Add(fieldLayouts_P1[1]);
                if (rank >= 5)
                    result.Add(fieldLayouts_P1[2]);
                break;
            case PlayerTeam.P2:
                result.Add(fieldLayouts_P2[0]);
                if (rank >= 3)
                    result.Add(fieldLayouts_P2[1]);
                if (rank >= 5)
                    result.Add(fieldLayouts_P2[2]);
                break;
        }

        return result;
    }
}
