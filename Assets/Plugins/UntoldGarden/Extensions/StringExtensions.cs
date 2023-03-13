using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden.Utils
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmptyOrFalse(this string s)
        {
            if (string.IsNullOrEmpty(s) || s == "false" || s == "False" || s == "FALSE" || s == "0" || s == "0.0" || s == "0.00" || s == "failed" || s == "Failed" || s == "FAILED" || s == "failed/" || s == "error")
            {
                return true;
            }
            return false;
        }

        public static bool TryConvertToBoolean(this string s, string errorRef = "")
        {
            bool b = false;
            try
            {
                b = (s != "") ? Convert.ToBoolean(s) : false;
            }
            catch (Exception e)
            {
                Debug.Log($"Error converting to bool for {errorRef}, Error " + e.Message);
            }
            return b;
        }

        public static double TryConvertToDouble(this string s, string errorRef = "")
        {
            double b = 0;
            try
            {
                b = (s != "") ? Convert.ToDouble(s) : 0;
            }
            catch (Exception e)
            {
                Debug.Log($"Error converting to bool for {errorRef}, Error " + e.Message);
            }
            return b;
        }

        public static float TryConvertToSingle(this string s, string errorRef = "")
        {
            float b = 0;
            try
            {
                b = (s != "") ? Convert.ToSingle(s) : 0;
            }
            catch (Exception e)
            {
                Debug.Log($"Error converting to bool for {errorRef}, Error " + e.Message);
            }
            return b;
        }   
    }
}