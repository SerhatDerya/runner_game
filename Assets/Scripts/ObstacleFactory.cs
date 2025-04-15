using UnityEngine;
using System.Collections.Generic;

public class ObstacleFactory : GameObjectFactory
{
    public ObstacleFactory(List<GameObject> prefabs, Transform parent = null)
        : base(prefabs, parent)
    {
    }

    public override GameObject Create()
    {
        GameObject obstacle = base.Create();
        // Obstacle nesnesine özel ayarlar yapılabilir
        return obstacle;
    }
}