using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
partial struct VelocityMonitorSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<VelocityMonitorComponent>();
        state.RequireForUpdate<LocalTransform>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        foreach (var (velocityMonitorComponent, localToWorld)
                 in SystemAPI.Query<RefRW<VelocityMonitorComponent>, RefRO<LocalToWorld>>().WithAll<Parent>())
        {
            velocityMonitorComponent.ValueRW.CurrentLinearVelocity =
                CustomHandJointMath.GetLinearVelocity(localToWorld.ValueRO.Position, velocityMonitorComponent.ValueRO.LastWorldSpacePosition, deltaTime);
            velocityMonitorComponent.ValueRW.CurrentAngularVelocity =
                CustomHandJointMath.GetAngularVelocityEuler(localToWorld.ValueRO.Rotation, velocityMonitorComponent.ValueRO.LastWorldSpaceRotation, deltaTime);

            velocityMonitorComponent.ValueRW.LastWorldSpacePosition = localToWorld.ValueRO.Position;
            velocityMonitorComponent.ValueRW.LastWorldSpaceRotation = localToWorld.ValueRO.Rotation;
        }

        foreach (var (velocityMonitorComponent, localTransform)
                 in SystemAPI.Query<RefRW<VelocityMonitorComponent>, RefRO<LocalTransform>>().WithNone<Parent>())
        {
            velocityMonitorComponent.ValueRW.CurrentLinearVelocity =
                CustomHandJointMath.GetLinearVelocity(localTransform.ValueRO.Position, velocityMonitorComponent.ValueRO.LastWorldSpacePosition, deltaTime);
            velocityMonitorComponent.ValueRW.CurrentAngularVelocity =
                CustomHandJointMath.GetAngularVelocityEuler(localTransform.ValueRO.Rotation, velocityMonitorComponent.ValueRO.LastWorldSpaceRotation, deltaTime);

            velocityMonitorComponent.ValueRW.LastWorldSpacePosition = localTransform.ValueRO.Position;
            velocityMonitorComponent.ValueRW.LastWorldSpaceRotation = localTransform.ValueRO.Rotation;
            UnityEngine.Debug.Log($"VelocityMonitorSystem: {localTransform.ValueRO.Position}");

        }
    }
}