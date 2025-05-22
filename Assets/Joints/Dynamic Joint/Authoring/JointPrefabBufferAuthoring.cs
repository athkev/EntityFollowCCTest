using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class JointPrefabBufferAuthoring : MonoBehaviour
{
    [SerializeField] private List<JointPrefabData> _joinPrefabDataList;

    class Baker : Baker<JointPrefabBufferAuthoring>
    {
        public override void Bake(JointPrefabBufferAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var jointPrefabBuffer = AddBuffer<JointPrefabBuffer>(entity);

            foreach (var jointPrefabData in authoring._joinPrefabDataList)
            {
                jointPrefabBuffer.Add(new JointPrefabBuffer()
                {
                    Key = jointPrefabData.Key,
                    Prefab = GetEntity(jointPrefabData.Prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    [Serializable]
    public class JointPrefabData
    {
        public int Key;
        public GameObject Prefab;
    }
}