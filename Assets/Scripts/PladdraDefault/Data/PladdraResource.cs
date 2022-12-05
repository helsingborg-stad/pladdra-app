using System;
using UnityEngine;

namespace Pladdra.DefaultAbility.Data
{
    [System.Serializable]
    public class PladdraResource
    {
        public string Id;
        public string Name;
        public string Type;
        public string ModelURL; // TODO this is the ID
        public string ModelIconURL; 
        public GameObject Model;
        public Texture2D Thumbnail;
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Scale = Vector3.zero;
        public bool Disable;
        public ARMarker Marker;
    }
}