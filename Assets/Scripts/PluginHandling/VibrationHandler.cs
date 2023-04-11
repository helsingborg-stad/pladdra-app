using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
using Lofelt.NiceVibrations;
#endif

namespace Pladdra.Plugins
{
    public class VibrationHandler : MonoBehaviour
    {
        public static void PositiveVibration()
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            HapticPatterns.PlayEmphasis(1, 1f);
            Debug.Log("Positive vibration");
#endif
        }

        public static void NegativeVibration()
        {
            Vibrate(0.5f, 0.5f, 0.5f);
            Debug.Log("Negative vibration");
        }
        public static void Vibrate(float amplitude, float frequency, float duration)
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            HapticPatterns.PlayConstant(amplitude, frequency, duration);
#endif
        }

        public static void ChangeAmplitude(float amplitude)
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            HapticController.clipLevel = amplitude;
#endif
        }

        public static void ChangeFrequency(float frequency)
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            HapticController.clipFrequencyShift = frequency;
#endif
        }

        public static bool HapticsSupported()
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            return DeviceCapabilities.isVersionSupported;
#else
return false;
#endif
        }

        public static bool AdvancedSupported()
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            return DeviceCapabilities.meetsAdvancedRequirements;
#else
return false;
#endif
        }

        public static bool AmplitudeModulationSupported()
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            return DeviceCapabilities.hasAmplitudeModulation;
#else
return false;
#endif
        }

        public static bool FrequencyModulationSupported()
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            return DeviceCapabilities.hasFrequencyModulation;
#else
return false;
#endif
        }

        public static void ToggleHaptics(bool enabled)
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            HapticController.hapticsEnabled = enabled;
#endif
        }

        public static void GlobalIntensity(float intensity)
        {
#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
            HapticController.outputLevel = intensity;
#endif
        }
    }
}