using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Pladdra
{
    public class DeeplinkTester : MonoBehaviour
    {
        public AbilityManager abilityManager;

        [TextArea(10, 10)][SerializeField] string deeplink;
        void Start()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(deeplink))
                abilityManager.OpenDeepLink(deeplink);
#endif

        }
    }
}