using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UntoldGarden.Utils;
using UnityEngine.Android;
using System;

namespace Pladdra
{
    /// <summary>
    /// Manages raycasts. Contains a list with layerMasks to raycast against.
    /// Not ability specific.
    /// </summary>
    public class RaycastManager : MonoBehaviour
    {
        #region Public
        [Header("Touch Settings")]
        [Tooltip("Draws a line between the screen touch point and the world touch point")]
        [SerializeField] bool visualizeTouch = true;
        public bool VisualiseTouch { get { return visualizeTouch; } }
        [Tooltip("Length of tap in ms")]
        [SerializeField] float tapLength;
        [Tooltip("Mapped to touch.y from bottom")]
        [SerializeField] bool mapTouch = false;
        [SerializeField] float mapTouch_Bottom;
        [Tooltip("Mapped to touch.y from top")]
        [SerializeField] float mapTouch_Top;
        [Tooltip("Screen Y limit below which touch is not recognized.")]
        public float minTouchLimit = 100;
        public float maxTouchLimit = 100;
        [SerializeField] Camera cam;

        [Header("Raycast Settings")]
        [Tooltip("Layer mask for raycast")]
        [SerializeField] List<NamedLayerMask> namedLayerMasks = new List<NamedLayerMask>();
        [GrayOut] public string currentLayerMask = "default";

        [Header("Events")]
        public UnityEvent<Vector3> OnHitPoint;
        public UnityEvent<GameObject> OnHitObject;
        public UnityEvent<Vector3, GameObject> OnHitPointAndObject;
        public UnityEvent<Touch> OnTouch;
        public UnityEvent<Touch[], RaycastHit[], RaycastHit> OnTouches;
        public UnityEvent<Vector3, GameObject> OnTap;
        public UnityEvent OnSecondFingerEnd;
        public UnityEvent<Touch, Touch> OnTwoFingerTouch;
        public UnityEvent<Vector2> OnTwoFingerDrag;
        public UnityEvent OnEndTouch;
        public bool isBlockedByUIElement = false;
        #endregion Public

        #region Scene References
        LineRenderer lineRenderer;
        // ProjectManager projectManager { get { return transform.parent.GetComponentInChildren<ProjectManager>(); } }
        #endregion Scene References

        #region Private
        Dictionary<string, LayerMask> layerMasks = new Dictionary<string, LayerMask>();
        string pastLayerMask;
        public LayerMask LayerMask { get { return layerMasks[currentLayerMask]; } }
        float screenHeight;
        private Touch[] touches;
        private Touch? fakeEndTouch;
        private Touch? pastTouch;
        GameObject hitObject;
        private Dictionary<int, int> touchList = new Dictionary<int, int>();
        bool enableTouch = true;
        List<Action> layersToRemove = new List<Action>();
        float t = 0;
        #endregion Private

        #region Monobehaviour 
        public void Start()
        {
            if (!cam) cam = Camera.main;

            foreach (NamedLayerMask namedLayerMask in namedLayerMasks)
            {
                layerMasks.Add(namedLayerMask.name, namedLayerMask.layerMask);
            }

            lineRenderer = GetComponent<LineRenderer>();
            if (!lineRenderer)
                visualizeTouch = false;

            screenHeight = Screen.height;
        }

        void Update()
        {
            if (!cam || !enableTouch)
                return;

            if (GetTouch(out touches))
            {
                t += Time.deltaTime;
                if (t < tapLength)
                {
                    pastTouch = touches[0];
                    return;
                }

                Vector2 screenPoint = touches[0].position;

                if (mapTouch)
                {
                    float y = screenPoint.y;
                    y = y.Map(0, screenHeight, mapTouch_Bottom, screenHeight - mapTouch_Top);
                    screenPoint = new Vector2(screenPoint.x, y);
                }

                // Debug.Log($"Y is {screenPoint.y}, y mapped is {y}, screenHeight {screenHeight}, fromTarget {touchMap}, toTarget {screenHeight - touchMap} ");
                //AR.Logger.Log("Touches: " + touches.Length);
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(screenPoint);

                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray.origin, ray.direction, 100f, layerMasks[currentLayerMask]);
                // Ray rayOrigin = cam.ScreenPointToRay(screenPoint); //This one is just for the viz LineRenderer
                if (Physics.Raycast(ray, out hit, 100, layerMasks[currentLayerMask]))
                {
                    hitObject = hit.transform.gameObject;

                    // if (visualizeTouch)
                    // {
                    //     lineRenderer.SetPosition(0, rayOrigin.origin);
                    //     lineRenderer.SetPosition(1, hit.point);
                    // }
                    OnHitPoint.Invoke(hit.point);
                    OnHitObject.Invoke(hitObject);
                    OnHitPointAndObject.Invoke(hit.point, hitObject);
                    // Debug.Log("Hit object " + hitObject.name);
                }

                OnTouch.Invoke(touches[0]);
                OnTouches.Invoke(touches, hits, hit);

                if (touches.Length > 1)
                {
                    OnTwoFingerTouch.Invoke(touches[0], touches[1]);
                    if (touches[1].phase == TouchPhase.Canceled || touches[1].phase == TouchPhase.Ended)
                    {
                        // AR.Logger.Log("Second finger tap");
                        OnSecondFingerEnd.Invoke();
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // AR.Logger.Log("Second finger tap");
                    OnSecondFingerEnd.Invoke();
                }

                if (touches[0].phase == TouchPhase.Ended || touches[0].phase == TouchPhase.Canceled)
                {
                    OnEndTouch.Invoke();
                }

                pastTouch = touches[0];
                hitObject = null;
                // fakeEndTouch = null;
            }
            else if (t > 0)
            {
                if (t < tapLength && pastTouch != null)
                {
                    Vector2 screenPoint = pastTouch.Value.position;
                    RaycastHit hit;
                    Ray ray = cam.ScreenPointToRay(screenPoint);

                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        hitObject = hit.transform.gameObject;
                    }
                    OnTap.Invoke(hit.point, hitObject);
                    hitObject = null;
                    pastTouch = null;
                }
                // Debug.Log($"t is {t}");
                t = 0;
                if (visualizeTouch)
                {
                    lineRenderer.SetPosition(0, Vector3.zero);
                    lineRenderer.SetPosition(1, Vector3.zero);
                }
            }
        }
        #endregion Monobehaviour

        #region Touch
        /// <summary>
        /// Enables/disables touch.
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableTouch(bool enable)
        {
            enableTouch = enable;
        }
        //TODO Check if it's a tap or persistent before
        private bool GetTouch(out Touch[] touches)
        {
            bool gotTouch = false;
            touches = new Touch[1];

            if (Input.touches.Length > 0)
            {
                // Debug.Log("Start touch check " + fakeEndTouch.HasValue + ", " + Time.frameCount);
#if UNITY_EDITOR
                Vector2 touchPos = Input.touches[0].position;
#else
                Vector2 touchPos = Input.touches[0].rawPosition;
#endif

                if (touchPos.y > minTouchLimit && touchPos.y < (screenHeight - maxTouchLimit))
                {
                    touches = Input.touches;
                    gotTouch = true;
                    fakeEndTouch = touches[0];
                    // Debug.Log("Touch in bounds " + (fakeEndTouch.HasValue) + ", " + Time.frameCount);
                }
                else if (fakeEndTouch.HasValue)
                {
                    // Debug.Log("Fake end touch");
                    //Fakes an end touch if we go down into tool area to make sure we send a TouchPhase.Ended 
                    touches = new Touch[1] { new Touch { position = fakeEndTouch.Value.position, phase = TouchPhase.Ended } };
                    gotTouch = true;
                    fakeEndTouch = null;
                }
                else
                {
                    // Debug.Log("Touch out of bounds: " + (fakeEndTouch.HasValue) + ", " + Time.frameCount);
                }
            }
            else
            {
                if (fakeEndTouch.HasValue)
                    fakeEndTouch = null;
            }
            return gotTouch;
        }

        /// <summary>
        /// Displays a line between the screen touch point and the raycast hit. 
        /// </summary>
        /// <param name="enable"></param>
        public void EnableTouchVisualisation(bool enable)
        {
            if (lineRenderer == null)
                return;

            visualizeTouch = enable;
            lineRenderer.enabled = enable;

            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
        }

        #endregion Touch

        #region LayerMasks

        /// <summary>
        /// Sets the LayerMask to check raycasts against.
        /// </summary>
        /// <param name="layerMask">The layermask to use.</param>
        public void SetLayerMask(string layerMask)
        {
            pastLayerMask = currentLayerMask;
            currentLayerMask = layerMask;
        }

        /// <summary>
        /// Sets the LayerMask to check raycasts against to the default LayerMask.
        /// </summary>
        public void SetDefaultLayerMask()
        {
            currentLayerMask = "default";
        }

        /// <summary>
        /// Returns a LayerMask from name.
        /// </summary>
        /// <param name="layerMask">Name of the layermask to return.</param>
        /// <returns></returns>
        public LayerMask GetLayerMask(string layerMask)
        {
            return layerMasks[layerMask];
        }

        /// <summary>
        /// Adds a layer to a LayerMask.
        /// An action is added to layersToRemove, so that all added layers can be removed.
        /// </summary>
        /// <param name="layerMask">The LayerMask to add the layer to.</param>
        /// <param name="layer">The layer to add./param>
        internal void AddLayerToLayerMask(string layerMask, string layer)
        {
            layerMasks[layerMask] = layerMasks[layerMask] | (1 << LayerMask.NameToLayer(layer));
            // projectManager.OnOpenProject.AddListener(() => { RemoveLayerFromLayerMask(layerMask, layer); });
            layersToRemove.Add(() => { RemoveLayerFromLayerMask(layerMask, layer); });
        }

        /// <summary>
        /// Removes a layer from a LayerMask.
        /// </summary>
        /// <param name="layerMask">The LayerMask to remove the layer from.</param>
        /// <param name="layer">The layer to remove./param>
        internal void RemoveLayerFromLayerMask(string layerMask, string layer)
        {
            layerMasks[layerMask] = layerMasks[layerMask] & ~(1 << LayerMask.NameToLayer(layer));
        }

        /// <summary>
        /// Removes all layers that have been added to LayerMasks.
        /// </summary>
        internal void CleanLayerMasksFromlayersToRemove()
        {
            foreach (Action action in layersToRemove)
            {
                action();
            }
        }
        #endregion LayerMasks
    }

    /// <summary>
    /// Helper object to display LayerMasks with name in the inspector.
    /// </summary>
    [Serializable]
    public class NamedLayerMask
    {
        public string name;
        public LayerMask layerMask;

        public NamedLayerMask(string name, LayerMask layerMask)
        {
            this.name = name;
            this.layerMask = layerMask;
        }
    }
}