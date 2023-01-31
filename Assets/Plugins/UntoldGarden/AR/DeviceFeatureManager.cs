using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UntoldGarden;

namespace UntoldGarden.AR
{

    public class DeviceFeatureManager : MonoBehaviour
    {
        private void Awake()
        {
            // Debug.Log(DeviceFeature.IsPhone());
            // Debug.Log("width:" + Screen.width);
            // Debug.Log("height:" + Screen.height);
            // Debug.Log((float)Screen.width / (float)Screen.height);
            if (DeviceFeature.IsPhone())
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }

            //never sleep on mobile
#if UNITY_ANDROID || UNITY_IOS
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        }
    }
}