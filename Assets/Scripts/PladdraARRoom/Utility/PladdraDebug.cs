using Newtonsoft.Json;
using UnityEngine;

namespace Utility
{
    public static class PladdraDebug
    {
        public static void LogJson(object entry)
        {
            Debug.Log(JsonConvert.SerializeObject(entry, Formatting.Indented));
        }
    }
}