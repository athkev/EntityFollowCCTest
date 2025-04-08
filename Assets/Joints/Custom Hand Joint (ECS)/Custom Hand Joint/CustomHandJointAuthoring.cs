using Unity.Entities;
using UnityEngine;

public class CustomHandJointAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _playerMovementGameObject;
    [SerializeField] private GameObject _controllerHandGameObject;

    [SerializeField] private float _linearSpringStrength = 10;
    [SerializeField] private float _linearDamping = 0.1f;
    [SerializeField] private float _linearMaxForce = 200;

    [SerializeField] private float _angularSpringStrength = 10;
    [SerializeField] private float _angularDamping = 0.1f;
    [SerializeField] private float _angularMaxForce = 200;

    class Baker : Baker<CustomHandJointAuthoring>
    {
        public override void Bake(CustomHandJointAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var playerMovementEntity = GetEntity(authoring._playerMovementGameObject, TransformUsageFlags.Dynamic);
            var controllerHandEntity = GetEntity(authoring._controllerHandGameObject, TransformUsageFlags.Dynamic);


            AddComponent<CustomHandJointComponent>(entity, new()
            {
                PlayerEntity = playerMovementEntity,
                ControllerHandEntity = controllerHandEntity,

                LinearSpringStrength = authoring._linearSpringStrength,
                LinearDamping = authoring._linearDamping,
                LinearMaxForce = authoring._linearMaxForce,

                AngularSpringStrength = authoring._angularSpringStrength,
                AngularDamping = authoring._angularDamping,
                AngularMaxForce = authoring._angularMaxForce,
            });
        }
    }
}