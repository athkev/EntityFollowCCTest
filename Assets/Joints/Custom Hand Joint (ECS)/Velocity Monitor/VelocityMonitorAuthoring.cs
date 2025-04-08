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
                LastWorldSpaceRotation = quaternion.identity // so we dont get NaN on first run
            });
        }
    }
}