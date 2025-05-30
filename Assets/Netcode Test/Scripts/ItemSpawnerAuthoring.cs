using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
public class ItemSpawnerAuthoring : MonoBehaviour
{
    public GameObject ItemPrefab;

    class Baker : Baker<ItemSpawnerAuthoring>
    {
        public override void Bake(ItemSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new ItemSpawnerComponent
            {
                Item = GetEntity(authoring.ItemPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
public struct ItemSpawnerComponent : IComponentData
{
    public Entity Item;
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct SpawnItemSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (itemSpawner, entity) in SystemAPI.Query<RefRO<ItemSpawnerComponent>>().WithEntityAccess())
        {
            ecb.Instantiate(itemSpawner.ValueRO.Item);
            ecb.SetEnabled(entity, false);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}