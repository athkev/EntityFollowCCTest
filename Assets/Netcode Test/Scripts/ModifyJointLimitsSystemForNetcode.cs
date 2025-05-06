using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using static Unity.Physics.Math;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Collections;

[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup), OrderLast = true)]
partial struct ModifyJointLimitsSystemForNetcode : ISystem, ISystemStartStop
{
    [BurstCompile]
    private void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<PhysicsJoint>();
    }

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        // here you can add a Tag component to only affect the joint with a hand component or something.
        foreach (var joint in SystemAPI.Query<RefRW<PhysicsJoint>>())
        {
            if (joint.ValueRW.JointType == JointType.LimitedHinge)
            {
                FixedList512Bytes<Constraint> list = new FixedList512Bytes<Constraint>();

                foreach (var value in joint.ValueRO.GetConstraints())
                {
                    var copy = value;
                    copy.DampingRatio = 0;
                    list.Add(copy);
                }

                joint.ValueRW.SetConstraints(list);
            }
        }
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // use the component tag in the OnStartRunning function to filter here the joint as well.
        foreach (var joint in SystemAPI.Query<RefRW<PhysicsJoint>>())
        {
            if (joint.ValueRW.JointType == JointType.LimitedHinge)
            {
                joint.ValueRW.SetLimitedHingeRange(new FloatRange(0, 0));
            }
        }
    }
}