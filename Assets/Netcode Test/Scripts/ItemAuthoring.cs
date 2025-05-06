using Unity.Entities;
using UnityEngine;
public struct ItemComponent : IComponentData
{
    
}
[DisallowMultipleComponent]
public class ItemAuthoring : MonoBehaviour
{

    class Baker : Baker<ItemAuthoring>
    {
        public override void Bake(ItemAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ItemComponent());
        }
    }
}