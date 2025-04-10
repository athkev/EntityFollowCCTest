using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(VelocityMonitorSystem))]
partial struct CustomHandJointSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CustomHandJointComponent>();
        state.RequireForUpdate<PhysicsVelocity>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (customHandJointComponent, physicsVelocity, entity)
                 in SystemAPI.Query<RefRW<CustomHandJointComponent>, RefRW<PhysicsVelocity>>().WithEntityAccess())
        {
            var playerVelocityMonitorComponent = SystemAPI.GetComponent<VelocityMonitorComponent>(customHandJointComponent.ValueRO.PlayerEntity);
            physicsVelocity.ValueRW.Linear = playerVelocityMonitorComponent.CurrentLinearVelocity;
        }
    }
}