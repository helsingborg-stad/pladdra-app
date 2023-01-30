using System;
using UnityEngine;

namespace Pladdra.DialogueAbility.Data
{
    public enum ResourceDisplayRules
    {
        Static,
        Interactive,
        Marker,
        Library,
    }

    [System.Serializable]
    public class DialogueResource
    {
        public string name;
        public string type;
        public string url; // This doubles as the local storage ID
        public ResourceDisplayRules displayRule;
        public GameObject gameObject;
        public Texture2D thumbnail;
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public float scale = 0;
        public bool disable;
        public (string url, float width) marker;
    }
}
