using UnityEngine;
using System.Collections.Generic;

public class CoinPattern
{
    public static List<Vector3> GetLinePattern(Vector3 startPosition, int count, float spacing)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            positions.Add(startPosition + new Vector3(0, 0, i * spacing));
        }
        return positions;
    }

    public static List<Vector3> GetJumpArcPattern(Vector3 obstaclePosition, int count, float arcHeight, float spacing)
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            float arcY = Mathf.Sin(t * Mathf.PI) * arcHeight;
            float zOffset = (t - 0.5f) * spacing * count;

            Vector3 pos = obstaclePosition + new Vector3(0, arcY, zOffset);
            positions.Add(pos);
        }

        return positions;
    }

}