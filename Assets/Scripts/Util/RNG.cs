using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNG : MonoBehaviour
{

    public static int normalIntRange(int min, int max) {
        return min + (int)((Random.value + Random.value) * (Mathf.Abs(max - min) + 1) / 2f);
    }
   
}
