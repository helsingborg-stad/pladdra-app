using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden
{
    public static class QuaternionExtensions
    {
        public static bool Approximately(this Quaternion quatA, Quaternion value, float acceptableRange)
        {
            return 1 - Mathf.Abs(Quaternion.Dot(quatA, value)) < acceptableRange;
        }
    }
}