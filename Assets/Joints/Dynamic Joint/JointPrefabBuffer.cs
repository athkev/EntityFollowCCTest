using Unity.Entities;

public struct JointPrefabBuffer : IBufferElementData
{
    public int Key;
    public Entity Prefab;
}