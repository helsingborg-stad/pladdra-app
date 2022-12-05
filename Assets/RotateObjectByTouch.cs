using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden
{
    public class RotateObjectByTouch : MonoBehaviour
    {
        Vector3 startPosition;
        Vector3 currentPosition;
        Vector3 diffPosition;
        Vector3 currentRotation;
        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        startPosition = Input.mousePosition;
                        currentRotation = new Vector3(gameObject.transform.rotation.eulerAngles.x, 0, gameObject.transform.rotation.eulerAngles.z);
                        break;
                    case TouchPhase.Stationary:
                    case TouchPhase.Moved:
                        currentPosition = Input.mousePosition;
                        diffPosition = currentPosition - startPosition;
                        float xRotation = currentRotation.x + diffPosition.y / 2;
                        float zRotation = currentRotation.z + diffPosition.x / 2;
                        gameObject.transform.rotation = Quaternion.Euler(xRotation, 0, zRotation);
                        break;
                    case TouchPhase.Ended:
                        break;
                }
            }
        }
    }
}