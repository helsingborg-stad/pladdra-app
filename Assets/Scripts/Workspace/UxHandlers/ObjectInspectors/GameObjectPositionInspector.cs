using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Workspace.UxHandlers.ObjectInspectors
{
    public class GameObjectPositionInspector : AbstractGameObjectInspector<Vector3>, IGameObjectPositionInspector
    {
        public GameObjectPositionInspector(GameObject gameObject, VisualElement root) : base(gameObject, root)
        {
            AddTextField("position_x", p => p.x, (v , p) => new Vector3(v, p.y, p.z));
            AddTextField("position_y", p => p.y, (v , p) => new Vector3(p.x, v, p.z));
            AddTextField("position_z", p => p.z, (v , p) => new Vector3(p.x, p.y, v));
        }

        protected override Vector3 GetModel(GameObject gameObject)
        {
            return gameObject.transform.localPosition;
        }

        protected override void SetModel(GameObject gameObject, Vector3 model)
        {
            gameObject.transform.localPosition = model;
        }

        public void OnPositionChanged(Vector3 position)
        {
            UpdateUI(position);
            
        }
    }
}