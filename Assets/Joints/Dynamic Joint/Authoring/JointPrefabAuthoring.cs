using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class JointPrefabAuthoring : MonoBehaviour
{
    [SerializeField] private JointSettings _jointSettings;

    class Baker : Baker<JointPrefabAuthoring>
    {
        public override void Bake(JointPrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PhysicsConstrainedBodyPair>(entity);
            AddSharedComponent<PhysicsWorldIndex>(entity, new(0));

            var jointSettings = authoring._jointSettings;

            var bodyAFromJointSettings = jointSettings.BodyAFromJoint;
            var bodyBFromJointSettings = jointSettings.BodyBFromJoint;
            var bodyAFromJoint = new BodyFrame()
            {
                Position = bodyAFromJointSettings.Position,
                Axis = bodyAFromJointSettings.Axis,
                PerpendicularAxis = bodyAFromJointSettings.PerpendicularAxis,
            };
            var bodyBFromJoint = new BodyFrame()
            {
                Position = bodyBFromJointSettings.Position,
                Axis = bodyBFromJointSettings.Axis,
                PerpendicularAxis = bodyBFromJointSettings.PerpendicularAxis,
            };
            var physicsJoint = new PhysicsJoint()
            {
                JointType = JointType.Custom,
                BodyAFromJoint = bodyAFromJoint,
                BodyBFromJoint = bodyBFromJoint
            };

            var constraints = new FixedList512Bytes<Constraint>();
            if (jointSettings.ConstraintSettings1.Enable)
            {
                var constraint1Settings = jointSettings.ConstraintSettings1;
                constraints.Add(new Constraint()
                {
                    ConstrainedAxes = new bool3(constraint1Settings.ConstrainedAxesX, constraint1Settings.ConstrainedAxesY, constraint1Settings.ConstrainedAxesZ),
                    Type = constraint1Settings.Type,
                    Min = constraint1Settings.Min,
                    Max = constraint1Settings.Max,
                    SpringFrequency = constraint1Settings.SpringFrequency,
                    DampingRatio = constraint1Settings.DampingRatio,
                    MaxImpulse = constraint1Settings.MaxImpulse,
                    Target = constraint1Settings.Target,
                });
            }

            if (jointSettings.ConstraintSettings2.Enable)
            {
                var constraint2Settings = jointSettings.ConstraintSettings2;
                constraints.Add(new Constraint()
                {
                    ConstrainedAxes = new bool3(constraint2Settings.ConstrainedAxesX, constraint2Settings.ConstrainedAxesY, constraint2Settings.ConstrainedAxesZ),
                    Type = constraint2Settings.Type,
                    Min = constraint2Settings.Min,
                    Max = constraint2Settings.Max,
                    SpringFrequency = constraint2Settings.SpringFrequency,
                    DampingRatio = constraint2Settings.DampingRatio,
                    MaxImpulse = constraint2Settings.MaxImpulse,
                    Target = constraint2Settings.Target,
                });
            }

            if (jointSettings.ConstraintSettings3.Enable)
            {
                var constraint3Settings = jointSettings.ConstraintSettings3;
                constraints.Add(new Constraint()
                {
                    ConstrainedAxes = new bool3(constraint3Settings.ConstrainedAxesX, constraint3Settings.ConstrainedAxesY, constraint3Settings.ConstrainedAxesZ),
                    Type = constraint3Settings.Type,
                    Min = constraint3Settings.Min,
                    Max = constraint3Settings.Max,
                    SpringFrequency = constraint3Settings.SpringFrequency,
                    DampingRatio = constraint3Settings.DampingRatio,
                    MaxImpulse = constraint3Settings.MaxImpulse,
                    Target = constraint3Settings.Target,
                });
            }

            physicsJoint.SetConstraints(constraints);
            AddComponent<PhysicsJoint>(entity, physicsJoint);
        }
    }

    [Serializable]
    public struct JointSettings
    {
        public BodyFrameSettings BodyAFromJoint;
        public BodyFrameSettings BodyBFromJoint;
        public JointType JointType;
        public ConstraintSettings ConstraintSettings1;
        public ConstraintSettings ConstraintSettings2;
        public ConstraintSettings ConstraintSettings3;
    }

    [Serializable]
    public class ConstraintSettings
    {
        public bool Enable;

        public bool ConstrainedAxesX;
        public bool ConstrainedAxesY;
        public bool ConstrainedAxesZ;

        public ConstraintType Type;

        public float Min;
        public float Max;

        public float SpringFrequency;
        public float DampingRatio;
        public float SpringDamping;

        public Vector3 MaxImpulse;
        public Vector3 Target;
    }

    [Serializable]
    public struct BodyFrameSettings
    {
        public Vector3 Position;
        public Vector3 Axis;
        public Vector3 PerpendicularAxis;
    }
}