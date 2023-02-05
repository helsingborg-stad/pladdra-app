using System.Collections;
using System.Collections.Generic;
using Pladdra.ARSandbox.Dialogues.UX;
using Pladdra.UI;
using UnityEngine;

public class TestAnchor : MonoBehaviour
{
    public DialoguesUXManager uxManager;
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