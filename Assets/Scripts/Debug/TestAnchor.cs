using System.Collections;
using System.Collections.Generic;
using Pladdra.UI;
using Pladdra.UX;
using UnityEngine;

public class TestAnchor : MonoBehaviour
{
    public UXManager uxManager;
    public MenuManager menuManager;
    public GameObject anchor;

    void Start()
    {
        uxManager.UIManager.MenuManager.AddMenuItem(new MenuItem()
        {
            id = "alignToGeoAnchor",
            name = "Test anchor",
            action = () =>
            {
                uxManager.Project.SetGeoAnchor(anchor);
            }
        });
    }
}