using UnityEngine;
using System.Collections.Generic;

public class BuildingFactory : GameObjectFactory
{
    public BuildingFactory(List<GameObject> prefabs, Transform parent = null)
        : base(prefabs, parent)
    {
    }

    public override GameObject Create()
    {
        GameObject building = base.Create();
        // Building nesnesine özel ayarlar yapılabilir
        return building;
    }
}