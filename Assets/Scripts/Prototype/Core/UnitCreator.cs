using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] unitPrefabs;
    [SerializeField] private Material player1Material;
    [SerializeField] private Material player2Material;

    private Dictionary<string, GameObject> nameToUnitDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (var unit in unitPrefabs)
        {
            nameToUnitDict.Add(unit.GetComponent<Unit>().GetType().ToString(), unit);
        }
    }

    public GameObject CreateUnit(Type type)
    {
        GameObject prefab = nameToUnitDict[type.ToString()];
        if (prefab)
        {
            GameObject newUnit = Instantiate(prefab);
            return newUnit;
        }
        return null;
    }

    public Material GetTeamMaterial(PlayerTeam team)
    {
        if (team == PlayerTeam.P1)
            return player1Material;
        else if (team == PlayerTeam.P2)
            return player2Material;
        else
            return null;
    }


}
