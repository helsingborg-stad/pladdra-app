using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.Data
{
    public class PladdraResource : MonoBehaviour
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ModelUrl { get; set; } // TODO REMOVE
        public string ModelIconUrl { get; set; } // TODO REMOVE
        public GameObject Model { get; set; }
        public Texture2D ModelIcon { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public bool Disable { get; set; }
    }
}
