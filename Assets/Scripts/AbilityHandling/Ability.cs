using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pladdra
{
    [System.Serializable]
    public class Ability
    {
        public string name;
        public string description;
        [Tooltip("Unique identifiers for this ability. Any deeplink specifying these identifiers will open this ability. In most cases these will correspond to a wordpress post slug")]
        public List<string> deepLinkIdentifiers = new List<string>();
        [Tooltip("If false, this ability will not be shown in the list of abilities that is shown on app start.")]
        public bool showInList = true;
        [Tooltip("The start scene of this ability.")]
        [Scene]
        public string scene;
    }
}