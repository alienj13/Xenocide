using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour
{
    [SerializeField] private Material freeSquareMaterial;
    [SerializeField] private Material opponentSquareMaterial;
    [SerializeField] private GameObject selectorPrefab;
    [SerializeField] private GameObject subselectorPrefab;
    private List<GameObject> instantiatedSelectors = new List<GameObject>();
    private HashSet<Vector3> instantiatedPositions = new HashSet<Vector3>();

    public void ShowSelection(Dictionary<Vector3, bool> squaresData)
    {
        //ClearSelection();
        foreach (var data in squaresData)
        {
            if (!instantiatedPositions.Contains(data.Key))
            {
                GameObject selector = Instantiate(selectorPrefab, data.Key, Quaternion.identity);
                instantiatedSelectors.Add(selector);
                instantiatedPositions.Add(data.Key);
                foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>())
                    setter.SetSingleMaterial((data.Value) ? freeSquareMaterial : opponentSquareMaterial);
            }
            else
            {
                // Temporary solution for overlapping selectors
                Vector3 newPos = data.Key;
                newPos.y += 0.1f;
                GameObject subselector = Instantiate(subselectorPrefab, newPos, Quaternion.identity);
                subselector.transform.Rotate(new Vector3(0, 45, 0));
                instantiatedSelectors.Add(subselector);
                instantiatedPositions.Add(newPos);
                foreach (var setter in subselector.GetComponentsInChildren<MaterialSetter>())
                    setter.SetSingleMaterial((data.Value) ? freeSquareMaterial : opponentSquareMaterial);
            }
        }
    }

    public void ClearSelection()
    {
        foreach (var selector in instantiatedSelectors)
            Destroy(selector.gameObject);
        instantiatedSelectors.Clear();
        instantiatedPositions.Clear();
    }
}
