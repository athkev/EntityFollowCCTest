using Unity.Mathematics;

public static class CustomHandJointMath
{
    public static float3 ClampLength(float3 v, float max)
    {
        var length = math.length(v);
        if (length > max)
        {
            var vNor = math.normalize(v);
            v = vNor * max;
        }

        return v;
    }
    public static float3 GetLinearVelocity(float3 position2, float3 position1, float deltaTime)
    {
        var deltaPosition = (position2 - position1);
        return deltaPosition / deltaTime;
    }
    public static float3 GetAngularVelocityEuler(quaternion rotation2, quaternion rotation1, float deltaTime)
    {
        var deltaRotation = GetDeltaRotationEuler(rotation2, rotation1); // in radiance
        float3 angularVelocityInRadiance = deltaRotation / deltaTime;
        return angularVelocityInRadiance;
    }
    public static float3 GetDeltaRotationEuler(quaternion q2, quaternion q1) // returns delta rotation in radiance
    {
        quaternion deltaRot = math.mul(math.inverse(q1), q2);
        return math.Euler(deltaRot);
    }
}