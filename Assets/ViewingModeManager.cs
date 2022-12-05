using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.DefaultAbility
{
    public class ViewingModeManager : MonoBehaviour
    {
        public GameObject whiteSphere;

        void Start()
        {
            whiteSphere.SetActive(false);
        }

        public void ToggleWhiteSphere()
        {
            whiteSphere.SetActive(!whiteSphere.activeSelf);
        }
    }
}