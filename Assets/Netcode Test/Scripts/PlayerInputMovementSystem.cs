using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Burst;
using Unity.Physics;

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
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var (input, player) in SystemAPI.Query<RefRO<PlayerInput>, RefRW<Player>>().WithAll<Simulate>())
        {
            if (player.ValueRO.Character == Entity.Null || player.ValueRO.Controller == Entity.Null) continue;
            var characterTransform = SystemAPI.GetComponent<LocalTransform>(player.ValueRO.Character);
            //var controllerTransform = SystemAPI.GetComponent<LocalTransform>(player.ValueRO.Controller);
            characterTransform.Position = input.ValueRO.CharacterPosition;
            //controllerTransform.Position = input.ValueRO.ControllerPosition;
            state.EntityManager.SetComponentData(player.ValueRO.Character, characterTransform);
            //state.EntityManager.SetComponentData(player.ValueRO.Controller, controllerTransform);


            if (player.ValueRO.ControllerBodyPair == Entity.Null)
            {
                // instead of setting the local transform of the hand, find physics joint and set offset position
                var handChild = state.EntityManager.GetBuffer<Child>(player.ValueRW.Controller);
                foreach (var child in handChild)
                {
                    if (!state.EntityManager.HasComponent<PhysicsConstrainedBodyPair>(child.Value)) continue;
                    if (!state.EntityManager.HasComponent<PhysicsJoint>(child.Value)) continue;

                    // Found joint entity
                    player.ValueRW.ControllerBodyPair = child.Value;
                    
                    // Connect player body to the hand bodypair
                    var bodyPair = new PhysicsConstrainedBodyPair(player.ValueRO.Controller, player.ValueRO.Character, false);
                    ecb.SetComponent(child.Value, bodyPair);
                }
            }


            // set offset position on the joint entity
            var physicsJoint = state.EntityManager.GetComponentData<PhysicsJoint>(player.ValueRO.ControllerBodyPair);
            var bodyFrame = new BodyFrame(new RigidTransform(input.ValueRO.ControllerRotation, input.ValueRO.ControllerPosition));
            physicsJoint.BodyBFromJoint = bodyFrame;
            ecb.SetComponent(player.ValueRO.ControllerBodyPair, physicsJoint);

            // physicsJoint.Body
            
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}