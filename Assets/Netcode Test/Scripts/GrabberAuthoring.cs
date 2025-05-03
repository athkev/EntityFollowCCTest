using Unity.Entities;
using UnityEngine;
using Unity.NetCode;
using Unity.Burst;
public struct GrabberComponent : IComponentData
{
    public Entity RigidbodyEntity; // assign after ghost physics hand is spawned
    public bool grabbing;
}
[DisallowMultipleComponent]
public class GrabberAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject rigidbody;
    class Baker : Baker<GrabberAuthoring>
    {
        public override void Bake(GrabberAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GrabberComponent()
            {
                RigidbodyEntity = GetEntity(authoring.rigidbody, TransformUsageFlags.Dynamic)
            });
        }
    }
}
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerInputGrabSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (input, grabberComponent, ghostOwner) in SystemAPI.Query<RefRO<PlayerInput>, RefRW<GrabberComponent>, RefRO<GhostOwner>>().WithAll<Simulate>())
        {
            if (!input.ValueRO.GrabButton) continue;
            if (grabberComponent.ValueRO.grabbing) continue;
            grabberComponent.ValueRW.grabbing = true;

            // grab item

            foreach (var (_, attachJoint, dynamicJoint, itemGhostOwner) in SystemAPI.Query<RefRO<ItemComponent>, RefRW<AttachJointComponent>, RefRO<DynamicJointComponent>, RefRW<GhostOwner>>())
            {
                UnityEngine.Debug.Log("grabed");
                //grab logic here
                attachJoint.ValueRW.Attach = true;
                attachJoint.ValueRW.AttachToEntity = grabberComponent.ValueRO.RigidbodyEntity;

                // set ghost owner to the grabber
                itemGhostOwner.ValueRW.NetworkId = ghostOwner.ValueRO.NetworkId;
            }
        }
    }
}