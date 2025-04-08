using Unity.Collections;
using Unity.Entities;

public struct AttachJointComponent : IComponentData
{
    public bool Attach;
    public Entity AttachToEntity;
    public FixedList32Bytes<int> AttachJointPrefabIds;
}
