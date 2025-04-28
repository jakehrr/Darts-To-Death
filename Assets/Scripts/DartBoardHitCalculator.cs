using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartBoardHitCalculator : MonoBehaviour
{
    private int[] dartboardNumber = new int[]
    {
        6, 13, 4, 18, 1, 20, 5, 12, 9, 14,
        11, 8, 16, 7, 19, 3, 17, 2, 15, 10
    };

    public float GetAngleFromCentre(Vector2 dartPosition)
    {
        float angle = Mathf.Atan2(dartPosition.y, dartPosition.x) * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360f;

        return angle;
    }

    public int GetDartboardSection(Vector2 dartPosition)
    {
        float angle = GetAngleFromCentre(dartPosition);

        float adjustAngle = (angle + 9f) % 360f;

        int sectionIndex = Mathf.FloorToInt(adjustAngle / 18f);
        return dartboardNumber[sectionIndex];
    }
}
