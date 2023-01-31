using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden
{
    public static class DeviceFeature
    {
        public static bool IsPhone()
        {
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            return aspectRatio < 0.6f;
        }
    }
}

