using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
partial class ControllerMovementSystem : SystemBase
{
    private JointsDemoInputActionAsset _jointsDemoInputActionAsset;

    protected override void OnCreate()
    {
        _jointsDemoInputActionAsset = new();
        _jointsDemoInputActionAsset.Enable();

        RequireForUpdate<ControllerMovementComponent>();
        RequireForUpdate<LocalTransform>();
    }

    protected override void OnUpdate()
    {
        foreach (var (transformBasedMovementComponent, localToWorld)
                 in SystemAPI.Query<RefRW<ControllerMovementComponent>, RefRW<LocalTransform>>())
        {
            var movementInput = _jointsDemoInputActionAsset.Movement.ControllerMovement.ReadValue<Vector2>();
            var rotationInput = _jointsDemoInputActionAsset.Movement.ControllerRotation.ReadValue<float>();

            var movementSpeed = transformBasedMovementComponent.ValueRO.MovementSpeed;
            var rotationSpeed = transformBasedMovementComponent.ValueRO.RotationSpeed;

            localToWorld.ValueRW.Position = localToWorld.ValueRO.Position + new float3(movementInput.x, 0, movementInput.y) * movementSpeed * SystemAPI.Time.fixedDeltaTime;
            localToWorld.ValueRW.Rotation = math.mul(localToWorld.ValueRO.Rotation, quaternion.Euler(Vector3.up * rotationInput * rotationSpeed * math.TORADIANS * SystemAPI.Time.fixedDeltaTime));
        }
    }
}