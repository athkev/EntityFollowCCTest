using Unity.Entities;
using Unity.Mathematics;

public struct VelocityMonitorComponent : IComponentData
{
    public float3 LastWorldSpacePosition;
    public quaternion LastWorldSpaceRotation;

    public float3 CurrentLinearVelocity;
    public float3 CurrentAngularVelocity; // Euler, In radians
}