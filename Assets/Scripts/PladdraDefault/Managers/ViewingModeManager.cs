using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pladdra.DefaultAbility
{
    public class ViewingModeManager : MonoBehaviour
    {
        public Camera mainCamera;
        public GameObject whiteSphere;
        public Camera fadeCamera;
        public RenderTexture fadeTexture;
        public RawImage fadeImage;
        public LayerMask cullingMask;
        private int cullingMaskEverything;
        

        void Start()
        {
            //White Mode
            whiteSphere.SetActive(false);

            //Fade Mode
            fadeCamera.gameObject.SetActive(false);
            fadeImage.transform.parent.gameObject.SetActive(false);
            fadeTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            fadeCamera.targetTexture =  fadeTexture;
            fadeImage.texture = fadeTexture;
        }

        public void ToggleWhiteSphere()
        {
            whiteSphere.SetActive(!whiteSphere.activeSelf);
        }
        
        public void ToggleFadeMode()
        {
            fadeCamera.gameObject.SetActive(!fadeCamera.gameObject.activeSelf);
            fadeImage.transform.parent.gameObject.SetActive(!fadeImage.transform.parent.gameObject.activeSelf);
            if(fadeCamera.gameObject.activeSelf)
            {
                cullingMaskEverything = mainCamera.cullingMask;
                mainCamera.cullingMask = cullingMask;
            }
            else
            {
                mainCamera.cullingMask = cullingMaskEverything;
            }
        }
    }
}