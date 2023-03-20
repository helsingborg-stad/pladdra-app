using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Linq;

/// <summary>
/// Unity and C# extension scripts
/// </summary>

namespace UntoldGarden.Utils
{
    public static class GameObjectExtensions
    {
        // Checks if gameobject is seen by camera
        public static bool IsSeenByCam(this GameObject go, Camera cam = null)
        {
            if (cam == null)
                cam = Camera.main;
            Vector3 pos = cam.WorldToViewportPoint(go.transform.position);
            bool onScreen = pos.z > 0 && pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1;
            return onScreen;
        }

        public static bool IsCloseToCam(this GameObject go, Camera cam, float distance)
        {
            float realDist = Vector3.Distance(go.transform.position, cam.transform.position);
            bool isClose = realDist < distance;
            return isClose;
        }

        // Finds a grandchild with a specific tag - use sparingly, not optimized
        public static Transform FindDeepChildWithTag(this GameObject parent, string tag)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent.transform);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.CompareTag(tag))
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        public static GameObject[] FindDeepChildrenWithTag(this GameObject parent, string tag)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent.transform);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.CompareTag(tag))
                    gameObjects.Add(c.gameObject);
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }

            if (gameObjects.Count > 0)
                return gameObjects.ToArray();
            else
                return null;
        }

        // Finds a grandchild with a specific tag - use sparingly, not optimized
        public static Transform FindDeepChildByName(this GameObject parent, string name)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent.transform);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == name)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        public static Transform FindDeepChildWithComponent<T>(this GameObject parent) where T : Component
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent.transform);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.GetComponent<T>() != null)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        public static GameObject[] FindDeepChildrenWithComponent<T>(this GameObject parent) where T : Component
        {
            List<GameObject> gameObjects = new List<GameObject>();
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent.transform);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.GetComponent<T>() != null)
                    gameObjects.Add(c.gameObject);
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }

            if (gameObjects.Count > 0)
                return gameObjects.ToArray();
            else
                return null;
        }

        public static T FindDeepComponent<T>(this GameObject parent) where T : Component
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent.transform);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.GetComponent<T>() != null)
                    return c.GetComponent<T>();
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        public static T[] FindDeepComponents<T>(this GameObject parent) where T : Component
        {
            List<T> components = new List<T>();
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent.transform);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.GetComponent<T>() != null)
                    components.Add(c.gameObject.GetComponent<T>());
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }

            if (components.Count > 0)
                return components.ToArray();
            else
                return null;
        }
        public static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }

        public static GameObject CreateObjectBetweenTwoPoints(this Vector3 pointA, Vector3 pointB, PrimitiveType type = PrimitiveType.Plane, float width = 1, float? y = null)
        {
            if (y.HasValue)
            {
                pointA.y = y.Value;
                pointB.y = y.Value;
            }
            if (Vector3.Distance(pointA, pointB) > 200)
                return null;

            GameObject a = new GameObject();
            a.transform.position = pointA;

            GameObject b = new GameObject();
            b.transform.position = pointB;

            Vector3 midPoint = (a.transform.position + b.transform.position) / 2;
            float length = Vector3.Distance(a.transform.position, b.transform.position);

            GameObject go = GameObject.CreatePrimitive(type);
            go.transform.position = midPoint;
            if (type == PrimitiveType.Plane)
                go.transform.localScale = new Vector3(width * .1f, width * .1f, length * .1f);
            else if (type == PrimitiveType.Cylinder)
                go.transform.localScale = new Vector3(width, length / 2, width);
            else if (type == PrimitiveType.Cube)
                go.transform.localScale = new Vector3(width, length, width);


            go.transform.LookAt(a.transform);

            if (type == PrimitiveType.Cylinder || type == PrimitiveType.Cube)
                go.transform.rotation *= Quaternion.Euler(-90, 0, 0);

            Object.Destroy(a);
            Object.Destroy(b);
            return go;
        }

        public static void MoveToBoundsCenter(this GameObject go)
        {
            Bounds b = go.GetBounds();

            if (go.transform.parent != null)
                go.transform.localPosition = go.transform.parent.position - b.center;
            else
                go.transform.position = -b.center;
        }

        /// <summary>
        /// Sets the layer of the gameobject and all its children
        /// </summary>
        /// <param name="go">GameObject to set layer for</param>
        /// <param name="layer">Layer to set</param>
        public static void SetAllChildLayers(this GameObject go, string layer, bool setParent = true)
        {
            if (setParent)
                go.layer = LayerMask.NameToLayer(layer);
            foreach (Transform child in go.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(layer); ;
                child.gameObject.SetAllChildLayers(layer);
            }
        }

    }

}
