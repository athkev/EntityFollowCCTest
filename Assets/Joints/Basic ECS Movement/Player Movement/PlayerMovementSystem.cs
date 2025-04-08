using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
partial class PlayerMovementSystem : SystemBase
{
    private JointsDemoInputActionAsset _jointsDemoInputActionAsset;

    protected override void OnCreate()
    {
        _jointsDemoInputActionAsset = new();
        _jointsDemoInputActionAsset.Enable();

        RequireForUpdate<PlayerMovementComponent>();
        RequireForUpdate<LocalTransform>();
    }

    protected override void OnUpdate()
    {
        foreach (var (transformBasedMovementComponent, physicsVelocity)
                 in SystemAPI.Query<RefRW<PlayerMovementComponent>, RefRW<PhysicsVelocity>>())
        {
            var movementInput = _jointsDemoInputActionAsset.Movement.PlayerMovement.ReadValue<Vector2>();
            var rotationInput = _jointsDemoInputActionAsset.Movement.PlayerRotation.ReadValue<float>();

            var movementSpeed = transformBasedMovementComponent.ValueRO.MovementSpeed;
            var rotationSpeed = transformBasedMovementComponent.ValueRO.RotationSpeed;

            physicsVelocity.ValueRW.Linear = new float3(movementInput.x, 0, movementInput.y) * movementSpeed;
            physicsVelocity.ValueRW.Angular = Vector3.up * rotationInput * Mathf.Deg2Rad * rotationSpeed;
        }
    }
}