using Unity.Entities;
using Unity.Mathematics;

public struct CustomHandJointComponent : IComponentData
{
    public Entity PlayerEntity;
    public Entity ControllerHandEntity;

    public  float SlowdownDistance;
    public float minSpeed;
    public float maxSpeed;
    public float acceleratorFactor;

}