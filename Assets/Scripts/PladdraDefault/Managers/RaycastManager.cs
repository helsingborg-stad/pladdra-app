using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UntoldGarden.Utils;


namespace Pladdra
{
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
        [SerializeField] Camera cam;

        [Header("Raycast Settings")]
        [Tooltip("Layer mask for raycast")]
        [SerializeField] LayerMask layerMask;
        public LayerMask LayerMask { get { return layerMask; } set { layerMask = value; } }

        [Header("Events")]
        public UnityEvent<Vector3> OnHitPoint;
        public UnityEvent<GameObject> OnHitObject;
        public UnityEvent<Touch[], Vector3, GameObject> OnTouch;
        public UnityEvent<Vector3, GameObject> OnTap;
        public UnityEvent OnSecondFingerEnd;
        public UnityEvent<Touch, Touch> OnTwoFingerTouch;
        public UnityEvent<Vector2> OnTwoFingerDrag;
        public UnityEvent OnEndTouch;

        #endregion Public

        #region Private
        float screenHeight;
        private Touch[] touches;
        private Touch? fakeEndTouch;
        private Touch? pastTouch;
        GameObject hitObject;
        private Dictionary<int, int> touchList = new Dictionary<int, int>();
        LineRenderer lineRenderer;
        bool enableTouch = true;
        #endregion Private

        public void Start()
        {
            if (!cam) cam = Camera.main;

            lineRenderer = GetComponent<LineRenderer>();
            if (!lineRenderer)
                visualizeTouch = false;

            screenHeight = Screen.height;
        }

        public void SetEnableTouch(bool enable)
        {
            enableTouch = enable;
        }
        float t = 0;

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
                // Ray rayOrigin = cam.ScreenPointToRay(screenPoint); //This one is just for the viz LineRenderer

                if (Physics.Raycast(ray, out hit, 100, layerMask))
                {
                    hitObject = hit.transform.gameObject;

                    // if (visualizeTouch)
                    // {
                    //     lineRenderer.SetPosition(0, rayOrigin.origin);
                    //     lineRenderer.SetPosition(1, hit.point);
                    // }
                    OnHitPoint.Invoke(hit.point);
                    OnHitObject.Invoke(hitObject);
                }

                OnTouch.Invoke(touches, hit.point, hitObject);

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
                fakeEndTouch = null;
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
        //TODO Check if it's a tap or persistent before
        private bool GetTouch(out Touch[] touches)
        {
            bool gotTouch = false;

            touches = new Touch[1];

            if (Input.touches.Length > 0)
            {
                if (Input.touches[0].position.y > minTouchLimit)
                {
                    touches = Input.touches;
                    gotTouch = true;
                    fakeEndTouch = touches[0];
                }
                else if (fakeEndTouch != null)
                {
                    //Fakes an end touch if we go down into tool area to make sure we send a TouchPhase.Ended 
                    touches = new Touch[1] { new Touch { position = fakeEndTouch.Value.position, phase = TouchPhase.Ended } };
                    gotTouch = true;
                    fakeEndTouch = null;
                }
            }
            return gotTouch;
        }

        public void EnableTouchVisualisation(bool enable)
        {
            visualizeTouch = enable;
            lineRenderer.enabled = enable;

            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
        }
    }
}