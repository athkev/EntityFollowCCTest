using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Burst;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerInputMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerInput>();
        state.RequireForUpdate<Player>();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (input, player) in SystemAPI.Query<RefRO<PlayerInput>, RefRO<Player>>().WithAll<Simulate>())
        {
            if (player.ValueRO.Character == Entity.Null || player.ValueRO.Controller == Entity.Null) continue;
            var characterTransform = SystemAPI.GetComponent<LocalTransform>(player.ValueRO.Character);
            var controllerTransform = SystemAPI.GetComponent<LocalTransform>(player.ValueRO.Controller);
            characterTransform.Position = input.ValueRO.CharacterPosition;
            controllerTransform.Position = input.ValueRO.ControllerPosition;
            state.EntityManager.SetComponentData(player.ValueRO.Character, characterTransform);
            state.EntityManager.SetComponentData(player.ValueRO.Controller, controllerTransform);
        }
    }
}