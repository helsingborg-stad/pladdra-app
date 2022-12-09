using System.Collections;
using UnityEngine;

namespace Pladdra
{
    public class LoadingIndicator : MonoBehaviour
    {
        public GameObject loadingIndicator;
        public GameObject followPoint;

        public AnimationCurve spinCurve;
        public AnimationCurve scaleCurve;

        Coroutine spin;
        void Start()
        {
            loadingIndicator.SetActive(false);
        }
        public void StartLoadingIndicator()
        {
            loadingIndicator.SetActive(true);
            spin = StartCoroutine(Spin());
        }

        public void StopLoadingIndicator()
        {
            loadingIndicator.SetActive(false);
            if (spin != null)
                StopCoroutine(spin);
        }

        IEnumerator Spin()
        {
            float f = 0;
            Vector3 originScale = loadingIndicator.transform.localScale;
            while (true)
            {
                loadingIndicator.transform.position = followPoint.transform.position;
                loadingIndicator.transform.Rotate(0, spinCurve.Evaluate(f),0);
                loadingIndicator.transform.localScale = originScale * scaleCurve.Evaluate(f);
                f += Time.deltaTime;
                if (f > spinCurve.keys[spinCurve.length - 1].time)
                    f = 0;

                yield return new WaitForEndOfFrame();
            }
        }
    }
}