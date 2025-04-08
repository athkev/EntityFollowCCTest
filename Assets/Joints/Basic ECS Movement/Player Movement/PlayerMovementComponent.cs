using Unity.Entities;

public struct PlayerMovementComponent : IComponentData
{
    public float MovementSpeed;
    public float RotationSpeed;
}