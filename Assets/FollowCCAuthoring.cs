using Unity.Entities;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;
using Unity.Physics.Systems;
using Unity.Mathematics;
public class FollowCCAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _controller;
    class Baker : Baker<FollowCCAuthoring>
    {
        public override void Bake(FollowCCAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<FollowCCComponent>(entity, new FollowCCComponent
            {
                controller = GetEntity(authoring._controller, TransformUsageFlags.Dynamic)
            });

        }
    }
}

public struct FollowCCComponent : IComponentData
{
    public Entity controller;
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst =true)]
public partial struct FollowCCSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FollowCCComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ccMove = SimpleCCMove.Instance;
        if (ccMove == null) return;
        foreach (var (followCC, localTransform, entity)
                 in SystemAPI.Query<RefRW<FollowCCComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            localTransform.ValueRW.Position = ccMove.transform.position;

            var controllerTransform = state.EntityManager.GetComponentData<LocalTransform>(followCC.ValueRO.controller);

            var relativePos = ccMove._controller.position - (Vector3)localTransform.ValueRW.Position;

            // If the controller entity is the child of the root
            Quaternion inverseParentRot = Quaternion.Inverse(localTransform.ValueRW.Rotation);
            var localPos = inverseParentRot * relativePos;
            controllerTransform.Position = localPos + new Vector3(0, 1f, 0);

            // If the controller entity gets unparented
            //controllerTransform.Position = ccMove._controller.position + new Vector3(0,1,0);

            state.EntityManager.SetComponentData(followCC.ValueRO.controller, controllerTransform);
            UnityEngine.Debug.Log($"FollowCCSystem: {localTransform.ValueRW.Position}");
        }
    }
}