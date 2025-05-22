using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ForceParentAuthoring : MonoBehaviour
{
    class Baker : Baker<ForceParentAuthoring>
    {
        public override void Bake(ForceParentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            if (authoring.transform.parent != null)
            {
                var parentEntity = GetEntity(authoring.transform.parent, TransformUsageFlags.None);
                AddComponent<Parent>(entity, new()
                {
                    Value = parentEntity
                });
            }
        }
    }
}