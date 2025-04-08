using Unity.Entities;

public struct ControllerMovementComponent : IComponentData
{
    public float MovementSpeed;
    public float RotationSpeed;
}