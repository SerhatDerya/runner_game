using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstaclePool obstaclePool;
    [SerializeField] private LaneManager laneManager;

    private Queue<GameObject> activeObstacles = new Queue<GameObject>();

    [SerializeField] private float milestoneInterval = 30f;
    [SerializeField] private float obstacleSpawnChance = 0.7f;
    
    public void SpawnObstacles(GameObject platform)
    {
        float platformHeight = platform.GetComponent<Collider>().bounds.extents.y;

        float spawnAreaMinZ = platform.transform.position.z - platform.transform.localScale.z/2;
        float spawnAreaMaxZ = platform.transform.position.z + platform.transform.localScale.z/2;

        float spawnY = platform.transform.position.y + platformHeight + 0.5f;

        int laneCount = laneManager.laneCount;
        int milestoneCount = Mathf.FloorToInt(platform.transform.localScale.z / milestoneInterval);

        for (int i = 1; i <= milestoneCount; i++)
        {
            int obstacleFullCountOfMilestone = 0;
            for (int j = 0; j < laneCount; j++){
                float randomChanceValue = Random.Range(0f, 1f);
                float spawnZ = spawnAreaMinZ + i * milestoneInterval;
                if (obstacleFullCountOfMilestone != laneCount-1)
                {
                    if (randomChanceValue <= obstacleSpawnChance)
                    {
                        float spawnX = laneManager.GetLanePosition(j);
                        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

                        // Havuzdan engel al ve pozisyonunu ayarla
                        GameObject obstacle = obstaclePool.GetObstacle();
                        obstacle.transform.position = spawnPosition;
                        activeObstacles.Enqueue(obstacle);
                        if(obstacle.tag == "ObstacleFull")
                        {
                            obstacleFullCountOfMilestone++;
                        }
                    }
                }
                else
                {
                    break;
                }
                
            }
        }
    }

    public void ClearObstacles(GameObject platform)
    {
        float platformMinZ = platform.transform.position.z - platform.transform.localScale.z/2;
        float platformMaxZ = platform.transform.position.z + platform.transform.localScale.z/2;
        for(int i=0; i<activeObstacles.Count; i++)
        {   
            GameObject peekObstacle = activeObstacles.Peek();
            if(peekObstacle.transform.position.z >= platformMinZ && peekObstacle.transform.position.z <= platformMaxZ)
            {
                obstaclePool.ReturnToPool(activeObstacles.Dequeue());
            }
            else
            {
                break;
            }
            
        }
        
    }
    
    public void ChangeObstaclePosition(GameObject oldPlatform, GameObject newPlatform)
    {
        // Önce tüm engellerin yarısını havuza geri gönder
        ClearObstacles(oldPlatform);
        
        // Sonra platform için yeni engeller oluştur
        SpawnObstacles(newPlatform);
    }
}