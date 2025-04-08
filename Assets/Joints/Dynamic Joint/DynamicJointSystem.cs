using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct DynamicJointSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DynamicJointComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var jointPrefabBuffer = SystemAPI.GetSingletonBuffer<JointPrefabBuffer>();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (dynamicJointComponent, dynamicJointsBuffers, attachBodyComponent, detachBodyComponent, entity)
                 in SystemAPI.Query<RefRW<DynamicJointComponent>, DynamicBuffer<DynamicJointsBuffer>, RefRW<AttachJointComponent>, RefRW<DetachJointComponent>>().WithEntityAccess())
        {
            if (detachBodyComponent.ValueRO.Detach)
            {
                detachBodyComponent.ValueRW.Detach = false;
                TryDetachCurrentEntity(ref state, ref ecb, dynamicJointComponent, dynamicJointsBuffers);
            }

            if (attachBodyComponent.ValueRO.Attach)
            {
                TryDetachCurrentEntity(ref state, ref ecb, dynamicJointComponent, dynamicJointsBuffers);

                foreach (var jointPrefabId in attachBodyComponent.ValueRO.AttachJointPrefabIds)
                {
                    var jointPrefab = GetPrefab(jointPrefabBuffer, jointPrefabId);

                    var newJointEntity = ecb.Instantiate(jointPrefab);
                    ecb.SetName(newJointEntity, entity.ToFixedString());
                    ecb.AddComponent<Parent>(newJointEntity, new()
                    {
                        Value = entity
                    });
                    ecb.AddComponent<PreviousParent>(newJointEntity, new()
                    {
                        Value = entity
                    });
                    ecb.SetComponent<PhysicsConstrainedBodyPair>(newJointEntity, new PhysicsConstrainedBodyPair(entity, attachBodyComponent.ValueRO.AttachToEntity, false));
                    ecb.AppendToBuffer<DynamicJointsBuffer>(entity, new()
                    {
                        Entity = newJointEntity
                    });
                    ecb.AppendToBuffer<Child>(entity, new()
                    {
                        Value = newJointEntity
                    });
                }

                dynamicJointComponent.ValueRW.CurrentConnectedEntity = attachBodyComponent.ValueRO.AttachToEntity;

                attachBodyComponent.ValueRW.Attach = false;
                attachBodyComponent.ValueRW.AttachToEntity = Entity.Null;
                attachBodyComponent.ValueRW.AttachJointPrefabIds.Clear();
            }

            // If the connected entity is destroyed, then destroy the Fixed Joint as well
            if (!SystemAPI.Exists(dynamicJointComponent.ValueRO.CurrentConnectedEntity))
            {
                TryDetachCurrentEntity(ref state, ref ecb, dynamicJointComponent, dynamicJointsBuffers);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private bool TryDetachCurrentEntity(ref SystemState state, ref EntityCommandBuffer ecb, RefRW<DynamicJointComponent> dynamicJointComponent, DynamicBuffer<DynamicJointsBuffer> dynamicJointsBuffers)
    {
        if (!SystemAPI.Exists(dynamicJointComponent.ValueRO.CurrentConnectedEntity))
        {
            dynamicJointComponent.ValueRW.CurrentConnectedEntity = Entity.Null;
            dynamicJointsBuffers.Clear();
            return false;
        }

        foreach (var currentJointEntity in dynamicJointsBuffers)
        {
            if (SystemAPI.Exists(currentJointEntity.Entity))
            {
                ecb.DestroyEntity(currentJointEntity.Entity);
            }
        }

        dynamicJointComponent.ValueRW.CurrentConnectedEntity = Entity.Null;
        dynamicJointsBuffers.Clear();
        return true;
    }

    private Entity GetPrefab(DynamicBuffer<JointPrefabBuffer> jointPrefabBuffer, int id)
    {
        foreach (var jointPrefabBufferElement in jointPrefabBuffer)
        {
            if (jointPrefabBufferElement.Key == id)
            {
                return jointPrefabBufferElement.Prefab;
            }
        }

        return Entity.Null;
    }
}