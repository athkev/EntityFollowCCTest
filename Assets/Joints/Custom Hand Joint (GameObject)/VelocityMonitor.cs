using UnityEngine;

[DefaultExecutionOrder(-1)]
public class VelocityMonitor : MonoBehaviour
{
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    private Vector3 _linearVelocity;
    private Vector3 _angularVelocity;

    public Vector3 LinearVelocity => _linearVelocity;
    public Vector3 AngularVelocity => _angularVelocity;

    private void FixedUpdate()
    {
        // These account parent translation/rotation as well if its a child
        _linearVelocity = GetLinearVelocity(transform.position, _lastPosition);
        _angularVelocity = GetAngularVelocity(transform.rotation, _lastRotation);

        _lastPosition = transform.position;
        _lastRotation = transform.rotation;
    }

    #region Helper Math

    private Vector3 GetLinearVelocity(Vector3 p2, Vector3 p1)
    {
        return (p2 - p1) / Time.fixedDeltaTime;
    }

    private Vector3 GetAngularVelocity(Quaternion q2, Quaternion q1)
    {
        Vector3 deltaRotation = GetDeltaRotationEuler(q2, q1);
        Vector3 angularVelocityInRadiance = deltaRotation / Time.fixedDeltaTime;
        return angularVelocityInRadiance;
    }
    private Vector3 GetDeltaRotationEuler(Quaternion q2, Quaternion q1)
    {
        Quaternion rotationErrorEuler = Quaternion.Inverse(q1) * q2;
        return rotationErrorEuler.ToEuler(); // .eulerAngles returns in degrees and its buggy
    }

    #endregion
}