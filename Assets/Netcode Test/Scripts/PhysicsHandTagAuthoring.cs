using Unity.Entities;
using UnityEngine;
using Unity.NetCode;
using Unity.Burst;
// This is for assigning physics hand to Player (VrComponent) and grabber for client side
public struct PhysicsHandTag : IComponentData
{
}
[DisallowMultipleComponent]
public class PhysicsHandTagAuthoring : MonoBehaviour
{
    class Baker : Baker<PhysicsHandTagAuthoring>
    {
        public override void Bake(PhysicsHandTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PhysicsHandTag());
        }
    }
}