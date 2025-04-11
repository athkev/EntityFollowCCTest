using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Unity.Mathematics;

public struct PlayerInput : IInputComponentData
{
    public float3 CharacterPosition;
    public float3 ControllerPosition;
}

[DisallowMultipleComponent]
public class PlayerInputAuthoring : MonoBehaviour
{
    class PlayerInputBaking : Unity.Entities.Baker<PlayerInputAuthoring>
    {
        public override void Bake(PlayerInputAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerInput>(entity);
        }
    }
}

[WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct SamplePlayerInput : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamInGame>();
        state.RequireForUpdate<PlayerSpawner>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ccMove = SimpleCCMove.Instance;
        if (ccMove == null) return;
        foreach (var playerInput in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>())
        {
            playerInput.ValueRW.CharacterPosition = ccMove.transform.position;
            playerInput.ValueRW.ControllerPosition = ccMove._controller.transform.position;
        }
    }
}