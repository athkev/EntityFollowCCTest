using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class VelocityMonitorAuthoring : MonoBehaviour
{
    class Baker : Baker<VelocityMonitorAuthoring>
    {
        public override void Bake(VelocityMonitorAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<VelocityMonitorComponent>(entity, new()
            {
                LastLocalPosition = float3.zero,
                LastLocalRotation = quaternion.identity
            });
        }
    }
}