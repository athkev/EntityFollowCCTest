using System.Linq.Expressions;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(VelocityMonitorSystem))]
[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
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
            var offset = new float3(0, 1, 0);
            var playerVelocityMonitorComponent = SystemAPI.GetComponent<VelocityMonitorComponent>(customHandJointComponent.ValueRO.PlayerEntity);
            var controllerVelocityMonitorComponent = SystemAPI.GetComponent<VelocityMonitorComponent>(customHandJointComponent.ValueRO.ControllerHandEntity);

            var (playerPosition, playerRotation) = GetWorldPositionAndRotationOfEntity(ref state, customHandJointComponent.ValueRO.PlayerEntity);
            var (controllerHandPosition, controllerHandRotation) = GetWorldPositionAndRotationOfEntity(ref state, customHandJointComponent.ValueRO.ControllerHandEntity);
            var (physicsHandPosition, physicsHandRotation) = GetWorldPositionAndRotationOfEntity(ref state, entity);

            //var transform = state.EntityManager.GetComponentData<LocalTransform>(entity);
            //var targetTransform = new RigidTransform(controllerHandRotation, controllerHandPosition + new float3(0,1,0));
            //physicsVelocity.ValueRW = PhysicsVelocity.CalculateVelocityToTarget(
            //    state.EntityManager.GetComponentData<PhysicsMass>(entity), transform.Position, transform.Rotation,
            //    targetTransform, 1 / SystemAPI.Time.DeltaTime);

            
            #region Calculate Linear Velocity of the Physical Hand (While accounting for player position and rotation change)

            float3 toController = controllerHandPosition + offset - playerPosition;
            float3 tangentialVelocity = math.cross(playerVelocityMonitorComponent.CurrentAngularVelocity, toController); // Player rotation
            float3 positionError = controllerHandPosition + offset - physicsHandPosition;
            float3 velocityError = (controllerVelocityMonitorComponent.CurrentLinearVelocity - physicsVelocity.ValueRO.Linear);
            float3 springLinearVelocity = (positionError * customHandJointComponent.ValueRO.LinearSpringStrength) + (velocityError * customHandJointComponent.ValueRO.LinearDamping);
            springLinearVelocity = CustomHandJointMath.ClampLength(springLinearVelocity, customHandJointComponent.ValueRO.LinearMaxForce);

            physicsVelocity.ValueRW.Linear = springLinearVelocity + playerVelocityMonitorComponent.CurrentLinearVelocity + tangentialVelocity;
            
            #endregion  

            #region Calculate Angular Velocity of the Physical Hand (While accounting for player position change)

            float angularSpringRad = customHandJointComponent.ValueRO.AngularSpringStrength * math.TORADIANS;
            float angularDampingRad = customHandJointComponent.ValueRO.AngularDamping * math.TORADIANS;
            float angularMaxForce = customHandJointComponent.ValueRO.AngularMaxForce * math.TORADIANS;
            var rotationError = CustomHandJointMath.GetDeltaRotationEuler(physicsHandRotation, controllerHandRotation);
            float3 angularVelocityError = controllerVelocityMonitorComponent.CurrentAngularVelocity - physicsVelocity.ValueRO.Angular;
            float3 springAngularVelocity = (-rotationError * angularSpringRad) + (angularVelocityError * angularDampingRad);
            springAngularVelocity = CustomHandJointMath.ClampLength(springAngularVelocity, angularMaxForce);

            physicsVelocity.ValueRW.Angular = springAngularVelocity + playerVelocityMonitorComponent.CurrentAngularVelocity;

            #endregion
            
        }
    }

    private (float3, quaternion) GetWorldPositionAndRotationOfEntity(ref SystemState state, Entity entity)
    {
        if (SystemAPI.HasComponent<LocalToWorld>(entity))
        {
            var localToWorld = SystemAPI.GetComponent<LocalToWorld>(entity);
            return (localToWorld.Position, localToWorld.Rotation);
        }

        if (SystemAPI.HasComponent<LocalTransform>(entity))
        {
            var localTransform = SystemAPI.GetComponent<LocalTransform>(entity);
            return (localTransform.Position, localTransform.Rotation);
        }

        return (float3.zero, quaternion.identity);
    }
}