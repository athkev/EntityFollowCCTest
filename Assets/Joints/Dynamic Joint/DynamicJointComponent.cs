using Unity.Entities;

public struct DynamicJointComponent : IComponentData
{
    public Entity CurrentConnectedEntity;
}