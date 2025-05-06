using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct Player : IComponentData
{
    public Entity Character;
    public Entity Controller;
    public Entity ControllerBodyPair;
}

[DisallowMultipleComponent]
public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _character;
    [SerializeField] private GameObject _controller;
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Player>(entity, new()
            {
                Character = GetEntity(authoring._character, TransformUsageFlags.Dynamic),
                Controller = GetEntity(authoring._controller, TransformUsageFlags.Dynamic),
            });
        }
    }
}