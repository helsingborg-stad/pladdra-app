using System;
using UnityEngine;

namespace Pladdra.DefaultAbility.Data
{
    [System.Serializable]
    public class PladdraResource
    {
        public string id;
        public string name;
        public string type;
        public string modelURL; // TODO this is the ID
        public string modelIconURL;
        public GameObject model;
        public Texture2D thumbnail;
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public float scale = 0;
        public bool disable;
        public string markerURL;
        public Texture2D marker;
    }
}
