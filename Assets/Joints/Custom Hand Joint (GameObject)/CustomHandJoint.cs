using UnityEngine;

public class CustomHandJoint : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private VelocityMonitor _playerVelocityMonitor;

    [SerializeField] private Transform _controllerHandTransform;
    [SerializeField] private VelocityMonitor _controllerHandVelocityMonitor;

    [SerializeField] private Transform _physicalHandTransform;
    [SerializeField] private Rigidbody _physicalHandRb;

    [SerializeField] private float _linearSpringStrength = 10;
    [SerializeField] private float _linearDamping = 0.1f;
    [SerializeField] private float _linearMaxForce = 200;

    [SerializeField] private float _angularSpringStrength = 10;
    [SerializeField] private float _angularDamping = 0.1f;
    [SerializeField] private float _angularMaxForce = 200;

    private void FixedUpdate()
    {
        #region Calculate Linear Velocity of the Physical Hand (While accounting for player position and rotation change)

        Vector3 toController = _controllerHandTransform.position - _playerTransform.position;
        Vector3 tangentialVelocity = Vector3.Cross(_playerVelocityMonitor.AngularVelocity, toController); // Player rotation
        Vector3 positionError = _controllerHandTransform.position - _physicalHandRb.position;
        Vector3 velocityError = _controllerHandVelocityMonitor.LinearVelocity - _physicalHandRb.linearVelocity;
        Vector3 springLinearVelocity = (positionError * _linearSpringStrength) + (velocityError * _linearDamping);
        springLinearVelocity = Vector3.ClampMagnitude(springLinearVelocity, _linearMaxForce);

        _physicalHandRb.linearVelocity = springLinearVelocity + _playerVelocityMonitor.LinearVelocity + tangentialVelocity;

        #endregion

        #region Calculate Angular Velocity of the Physical Hand (While accounting for player position change)

        float angularSpringRad = _angularSpringStrength * Mathf.Deg2Rad;
        float angularDampingRad = _angularDamping * Mathf.Deg2Rad;
        float angularMaxForceRad = _angularMaxForce * Mathf.Deg2Rad;
        Vector3 rotationErrorEuler = GetDeltaRotationEuler(_controllerHandTransform.rotation, _physicalHandTransform.rotation);
        Vector3 angularVelocityError = _controllerHandVelocityMonitor.AngularVelocity - _physicalHandRb.angularVelocity;
        Vector3 springAngularVelocity = (rotationErrorEuler * angularSpringRad) + (angularVelocityError * angularDampingRad);
        springAngularVelocity = Vector3.ClampMagnitude(springAngularVelocity, angularMaxForceRad);

        _physicalHandRb.angularVelocity = springAngularVelocity + _playerVelocityMonitor.AngularVelocity;

        #endregion
    }

    private Vector3 GetDeltaRotationEuler(Quaternion q2, Quaternion q1)
    {
        Quaternion rotationErrorEuler = Quaternion.Inverse(q1) * q2;
        return rotationErrorEuler.ToEuler(); // .eulerAngles returns in degrees and its buggy
    }
}