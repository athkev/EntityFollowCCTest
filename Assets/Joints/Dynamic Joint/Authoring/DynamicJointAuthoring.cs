using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class DynamicJointAuthoring : MonoBehaviour
{
    [SerializeField] private bool _attachOnStart;
    [SerializeField] private GameObject _attachTo;
    [SerializeField] private List<int> _jointPrefabIds;

    class Baker : Baker<DynamicJointAuthoring>
    {
        public override void Bake(DynamicJointAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddBuffer<Child>(entity);
            AddBuffer<DynamicJointsBuffer>(entity);

            AddComponent<DynamicJointComponent>(entity);
            var jointPrefabIds = new FixedList64Bytes<int>();
            foreach (var jointPrefabReference in authoring._jointPrefabIds)
            {
                jointPrefabIds.Add(jointPrefabReference);
            }

            AddComponent<AttachJointComponent>(entity, new()
            {
                Attach = authoring._attachOnStart,
                AttachToEntity = authoring._attachOnStart ? GetEntity(authoring._attachTo, TransformUsageFlags.Dynamic) : Entity.Null,
                AttachJointPrefabIds = jointPrefabIds
            });
            AddComponent<DetachJointComponent>(entity);
        }
    }
}