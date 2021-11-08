using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGen : MonoBehaviour
{

    // Temporary solution
    // Using Vector2Int and x,y
    public static List<Vector2Int> square(int x, int y, int range) {
        List<Vector2Int> results = new List<Vector2Int>();

        // Temporary solution: Hard-coded limits
        int width = 10;
        int height = 10;
        
        for (int i = (x - range); i <= (x + range); i++) {
            for (int j = (y - range); j <= (y + range); j++) {
                if (i == x && j == y)
                    continue;
                if (i < 0 || j < 0 || i >= width || j >= height)
                    continue;
                results.Add(new Vector2Int(i, j));
            }
        }

        return results;
    }

    // Temporary solution
    // Using Vector2Int and x,y
    public static List<Vector2Int> diamond(int x, int y, int range) {
        List<Vector2Int> results = new List<Vector2Int>();

        // Temporary solution: Hard-coded limits
        int width = 10;
        int height = 10;

        for (int i = (x - range); i <= (x + range); i++) {
            int k = Mathf.Abs(Mathf.Abs(i - x) - range);
            for (int j = (y - k); j <= (y + k); j++) {
                if (i == x && j == y)
                    continue;
                if (i < 0 || j < 0 || i >= width || j >= height)
                    continue;
                results.Add(new Vector2Int(i, j));
            }
        }

        return results;
    }

}
