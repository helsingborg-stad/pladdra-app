using System;
using UnityEngine;
using Pladdra.Data;

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
    public class DialogueResource : Resource
    {
        public ResourceDisplayRules displayRule;
        public GameObject gameObject;
        public Texture2D thumbnail;
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public float scale = 0;
        public (string url, float width) marker;
    }
}
