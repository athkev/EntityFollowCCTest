using Unity.Entities;
using UnityEngine;

public class ControllerMovementAuthoring : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _rotationSpeed = 360;

    class Baker : Baker<ControllerMovementAuthoring>
    {
        public override void Bake(ControllerMovementAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<ControllerMovementComponent>(entity, new()
            {
                MovementSpeed = authoring._movementSpeed,
                RotationSpeed = authoring._rotationSpeed
            });
        }
    }
}