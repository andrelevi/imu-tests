using UnityEngine;

namespace IMUTest.Scripts
{
    public static class MathUtils
    {
        public static Quaternion Euler(float roll, float pitch, float yaw)
        {
            yaw*=Mathf.Deg2Rad;
            pitch*=Mathf.Deg2Rad;
            roll*=Mathf.Deg2Rad;

            double yawOver2 = yaw * 0.5f;
            float cosYawOver2 = (float)System.Math.Cos(yawOver2);
            float sinYawOver2 = (float)System.Math.Sin(yawOver2);
            double pitchOver2 = pitch * 0.5f;
            float cosPitchOver2 = (float)System.Math.Cos(pitchOver2);
            float sinPitchOver2 = (float)System.Math.Sin(pitchOver2);
            double rollOver2 = roll * 0.5f;
            float cosRollOver2 = (float)System.Math.Cos(rollOver2);
            float sinRollOver2 = (float)System.Math.Sin(rollOver2);            
            Quaternion result;
            result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.x = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return result;
        }
    }
}
