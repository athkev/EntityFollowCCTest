using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class ClientSidePhysicsSystemGroup : CustomPhysicsSystemGroup
{
    public ClientSidePhysicsSystemGroup() : base(1, true)
    {
        
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        UnityEngine.Debug.Log("ClientSidePhysicsSystemGroup OnUpdate");
        
    }
}
