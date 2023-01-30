using UnityEngine.SceneManagement;

namespace Pladdra
{
    [System.Serializable]
    public class Ability
    {
        public string name;
        public string description;
        [Scene]
        public string scene;
    }
}