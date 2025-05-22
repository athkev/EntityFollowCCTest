using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
partial struct CustomHandJointSystem : ISystem
{
    private const float SMOOTH_TIME = 0.2f; // Time to reach target
    private const float MAX_SPEED = 100f; // Maximum speed

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CustomHandJointComponent>();
        state.RequireForUpdate<PhysicsVelocity>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.fixedDeltaTime;
        
        foreach (var (customHandJointComponent, physicsVelocity, mass, localTransform, entity)
                 in SystemAPI.Query<RefRW<CustomHandJointComponent>, RefRW<PhysicsVelocity>, RefRO<PhysicsMass>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (math.isnan(localTransform.ValueRO.Position.x)||math.isnan(localTransform.ValueRO.Position.y)||math.isnan(localTransform.ValueRO.Position.z)
            || math.isnan(localTransform.ValueRO.Rotation.value.x)||math.isnan(localTransform.ValueRO.Rotation.value.y)||math.isnan(localTransform.ValueRO.Rotation.value.z))
            {
                localTransform.ValueRW.Position = float3.zero;
                physicsVelocity.ValueRW.Linear = float3.zero;
                physicsVelocity.ValueRW.Angular = float3.zero;
                localTransform.ValueRW.Rotation = quaternion.identity;
                continue;
            }

            // Get world position of Controller Hand Entity
            var controllerHandTransform = SystemAPI.GetComponent<LocalTransform>(customHandJointComponent.ValueRO.ControllerHandEntity);
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(customHandJointComponent.ValueRO.PlayerEntity);
            var controllerHandWorldPosition = playerTransform.Position + math.mul(playerTransform.Rotation, controllerHandTransform.Position);

            var direction = math.normalize(controllerHandWorldPosition - localTransform.ValueRO.Position);
            var distance = math.length(controllerHandWorldPosition - localTransform.ValueRO.Position);
            float desiredSpeed;
            if (distance < 0.1f) // Avoid jittering when very close
            {
                desiredSpeed = 0f;
            }
            else
            {
                // Linearly interpolate speed between minSpeed and maxSpeed
                desiredSpeed = math.lerp(customHandJointComponent.ValueRO.minSpeed, customHandJointComponent.ValueRO.maxSpeed, distance / customHandJointComponent.ValueRO.SlowdownDistance);
            }

            float3 targetVelocity = direction * desiredSpeed;

            physicsVelocity.ValueRW.Linear = targetVelocity * customHandJointComponent.ValueRO.acceleratorFactor;

        }
    }
}