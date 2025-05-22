using Unity.Entities;
using UnityEngine;

public class PlayerMovementAuthoring : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _rotationSpeed = 360;

    class Baker : Baker<PlayerMovementAuthoring>
    {
        public override void Bake(PlayerMovementAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerMovementComponent>(entity, new()
            {
                MovementSpeed = authoring._movementSpeed,
                RotationSpeed = authoring._rotationSpeed
            });
        }
    }
}