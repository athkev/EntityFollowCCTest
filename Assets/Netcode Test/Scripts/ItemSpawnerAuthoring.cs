using Unity.Entities;
using UnityEngine;

public struct ItemSpawnerComponent : IComponentData
{
    public Entity Item;
}
public struct ItemComponent : IComponentData
{
    
}

[DisallowMultipleComponent]
public class ItemSpawnerAuthoring : MonoBehaviour
{
    public GameObject ItemPrefab;

    class Baker : Baker<ItemSpawnerAuthoring>
    {
        public override void Bake(ItemSpawnerAuthoring authoring)
        {
            ItemSpawnerComponent component = default(ItemSpawnerComponent);
            component.Item = GetEntity(authoring.ItemPrefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, component);
        }
    }
}