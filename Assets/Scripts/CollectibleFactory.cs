using UnityEngine;
using System.Collections.Generic;

public class CollectibleFactory : GameObjectFactory
{
    public CollectibleFactory(List<GameObject> prefabs, Transform parent = null)
        : base(prefabs, parent)
    {
    }

    public override GameObject Create()
    {
        GameObject collectible = base.Create();
        // Collectible nesnesine özel ayarlar yapılabilir
        Debug.Log("Collectible created with special settings!");
        return collectible;
    }
}