using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleManager
{
    private Dictionary<int, HashSet<float>> laneObstacles = new Dictionary<int, HashSet<float>>();

    public void AddObstacle(int laneIndex, float zPosition)
    {
        if (!laneObstacles.ContainsKey(laneIndex))
        {
            laneObstacles[laneIndex] = new HashSet<float>();
        }
        laneObstacles[laneIndex].Add(zPosition);
    }

    public bool IsObstacleInLane(int laneIndex, float zPosition, float tolerance = 2f)
    {
        if (!laneObstacles.ContainsKey(laneIndex)) return false;

        return laneObstacles[laneIndex].Any(pos => Mathf.Abs(pos - zPosition) <= tolerance);
    }
}