using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldInputHandler : MonoBehaviour, IInputHandler
{
    private Field field;

    private void Awake()
    {
        field = GetComponent<Field>();
    }
    public void ProcessInput(Vector3 inputPosition, GameObject selectedObject, Action callback)
    {
        // Debug:
        //Debug.Log("FIH, input Position: " + inputPosition);
        var mousePosition = Input.mousePosition;
        //Debug.Log("FIH, mouse Position: " + mousePosition);
        if (mousePosition.x <= (Screen.width * 0.75f))
            field.OnSquareSelected(inputPosition);
    }
}
