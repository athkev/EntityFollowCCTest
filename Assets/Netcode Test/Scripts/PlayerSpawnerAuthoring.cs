using Unity.Entities;
using UnityEngine;

public struct PlayerSpawner : IComponentData
{
    public Entity Player;
}

[DisallowMultipleComponent]
public class PlayerSpawnerAuthoring : MonoBehaviour
{
    public GameObject PlayerPrefab;

    class Baker : Baker<PlayerSpawnerAuthoring>
    {
        public override void Bake(PlayerSpawnerAuthoring authoring)
        {
            PlayerSpawner component = default(PlayerSpawner);
            component.Player = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, component);
        }
    }
}