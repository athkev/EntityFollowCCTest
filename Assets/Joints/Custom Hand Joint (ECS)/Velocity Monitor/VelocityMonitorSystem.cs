using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(FollowCCSystem))]
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
        foreach (var (velocityMonitorComponent, localTransform)
                 in SystemAPI.Query<RefRW<VelocityMonitorComponent>, RefRO<LocalToWorld>>())
        {
            velocityMonitorComponent.ValueRW.CurrentLinearVelocity =
                CustomHandJointMath.GetLinearVelocity(localTransform.ValueRO.Position, velocityMonitorComponent.ValueRO.LastLocalPosition, deltaTime);
            velocityMonitorComponent.ValueRW.CurrentAngularVelocity =
                CustomHandJointMath.GetAngularVelocityEuler(localTransform.ValueRO.Rotation, velocityMonitorComponent.ValueRO.LastLocalRotation, deltaTime);

            velocityMonitorComponent.ValueRW.LastLocalPosition = localTransform.ValueRO.Position;
            velocityMonitorComponent.ValueRW.LastLocalRotation = localTransform.ValueRO.Rotation;
        }
    }
}