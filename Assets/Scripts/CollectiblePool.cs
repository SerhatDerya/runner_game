using System.Collections.Generic;
using UnityEngine;

public class CollectiblePool : GenericObjectPool<CollectibleController>
{
   public GameObject GetGameObject()
    {
        CollectibleController controller = GetObject();
        return controller?.gameObject;
    }
    
    public void ReturnGameObject(GameObject obj)
    {
        CollectibleController controller = obj.GetComponent<CollectibleController>();
        if (controller != null)
        {
            ReturnToPool(controller);
        }
        else
        {
            Debug.LogError($"GameObject does not have CollectibleController component!");
        }
    }
}