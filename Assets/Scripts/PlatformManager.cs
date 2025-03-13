using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{

    public GameObject platformPrefab;
    public Transform playerTransform;
    private GameObject currentPlatform;
    private int platformCount = 2;

    private Queue<GameObject> platformQueue = new Queue<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        platformQueue.Enqueue(platformPrefab);
        currentPlatform = platformPrefab;
        spawnPlatform();
    }

    void spawnPlatform()
    {
        
        
        if(platformQueue.Count == platformCount)
        {
            Destroy(platformQueue.Peek(), 1f);
            platformQueue.Dequeue();
        }

        GameObject oldPlatform = platformQueue.Peek();
        
        
        float platformX = oldPlatform.transform.position.x;
        float platformY = oldPlatform.transform.position.y;
        float platformZ = oldPlatform.transform.position.z;
        Vector3 newPlatformZ = new Vector3(platformX, platformY, oldPlatform.transform.position.z + 200);
        GameObject newPlatform = Instantiate(oldPlatform, newPlatformZ, Quaternion.identity);
        platformQueue.Enqueue(newPlatform);
        currentPlatform = platformQueue.Peek();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject lastPlatform = platformQueue.ToArray()[platformQueue.Count - 1];
        if (playerTransform.position.z >= lastPlatform.transform.position.z - (lastPlatform.transform.localScale.z/2))
        {
            currentPlatform = lastPlatform;
        }
        if(currentPlatform == lastPlatform)
        {
            spawnPlatform();
        }
    }
}
