using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden.Utils
{
public static class StringExtensions 
{
    public static bool IsNullOrEmptyOrFalse(this string s)
    {
        if (string.IsNullOrEmpty(s) || s == "false")
        {
            return true;
        }
        return false;
    }
}
}