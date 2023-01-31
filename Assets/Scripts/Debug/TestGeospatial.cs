using System;
using System.Collections;
using System.Collections.Generic;
using Pladdra.UI;
using UnityEngine;
using Pladdra;
public class TestGeospatial : MonoBehaviour
{
    public GeospatialManager geospatialManager;
    public MenuManager menuManager;
    public double lat;
    public double lon;
    // Start is called before the first frame update

    void Start()
    {
        menuManager.AddMenuItem(new MenuItem()
        {
            id = "test-geospatial",
            name = "Test Geospatial",
            action = () =>
            {
                Test();
            }
        });
    }
    public void Test()
    {
        Debug.Log("Testing geospatial");
        //create action to be called when object is placed
        Action<bool, string, GameObject> placedObject = PlacedObject;
        geospatialManager.PlaceGeoAnchorAtLocation("id", lat, lon, Quaternion.identity, placedObject);
    }

    void PlacedObject(bool b, string s, GameObject g)
    {
        Debug.Log("Placed Object, b: " + b + " s: " + s + " g: " + g);
    }
}
