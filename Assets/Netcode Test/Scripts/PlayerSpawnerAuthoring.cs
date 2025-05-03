using Unity.Entities;
using UnityEngine;

public struct PlayerSpawner : IComponentData
{
    public Entity Player;
    public Entity PhysicsHand;
}

[DisallowMultipleComponent]
public class PlayerSpawnerAuthoring : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject HandPrefab;

    class Baker : Baker<PlayerSpawnerAuthoring>
    {
        public override void Bake(PlayerSpawnerAuthoring authoring)
        {
            PlayerSpawner component = default(PlayerSpawner);
            component.Player = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic);
            component.PhysicsHand = GetEntity(authoring.HandPrefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, component);
        }
    }
}