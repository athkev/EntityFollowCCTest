using Unity.Entities;

public struct CustomHandJointComponent : IComponentData
{
    public Entity PlayerEntity;
    public Entity ControllerHandEntity;

    public float LinearSpringStrength;
    public float LinearDamping;
    public float LinearMaxForce;

    public float AngularSpringStrength; // In Degrees for easier editing, converted by system
    public float AngularDamping;
    public float AngularMaxForce;
}