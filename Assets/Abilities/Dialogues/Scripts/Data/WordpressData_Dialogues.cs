using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.Data;

namespace Pladdra.DialogueAbility.Data
{

    /// <summary>
    /// Helper object for deserializing json from wordpress for dialogue ability.
    /// </summary>
    [System.Serializable]
    public class WordpressData_Dialogues : WordpressData
    {
        public Acf_Dialogues acf;
        /// <summary>
        /// Creates a Project object from the data in the wordpress JSON.
        /// </summary>
        /// <returns>Project</returns>
        internal Project MakeProject()
        {
            name = title.rendered;
            Debug.Log("Making project from wordpress data: " + name);

            float startScale = 1;
            try { startScale = (acf.settings.scale != "") ? (float)Convert.ToSingle(acf.settings.scale) : 1; }
            catch (Exception e) { Debug.Log("Error converting scale to float: " + e.Message); }

            (double, double) location = (0, 0);
            try { location.Item1 = (acf.settings.geolocation.latitude != "") ? Convert.ToDouble(acf.settings.geolocation.latitude) : 0; }
            catch (Exception e) { Debug.Log("Error converting location lat to double: " + e.Message); }
            try { location.Item2 = (acf.settings.geolocation.longitude != "") ? Convert.ToDouble(acf.settings.geolocation.longitude) : 0; }
            catch (Exception e) { Debug.Log("Error converting location lng to double: " + e.Message); }

            bool markerRequired = false;
            try { markerRequired = (acf.settings.marker.required != "") ? Convert.ToBoolean(acf.settings.marker.required) : false; }
            catch (Exception e) { Debug.Log("Error converting marker required to bool: " + e.Message); }

            Project project = new Project()
            {
                id = id,
                name = name,
                description = acf.settings.description,
                startScale = startScale,
                markerURL = acf.settings.marker.image,
                markerRequired = markerRequired,
                location = location,
            };

            Debug.Log("Adding groundplane");

            // Add our groundplane if we have one
            if (acf.settings.groundplane != "")
            {
                project.groundPlane = new DialogueResource()
                {
                    name = "groundplane",
                    url = acf.settings.groundplane,
                };
            }
            Debug.Log("Adding resources");

            if (acf.models != null)
            {
                project.resources = new List<DialogueResource>();

                foreach (var item in acf.models)
                {
                    if (item.source == "" || item.source == "false")
                        continue;

                    //TODO There is a better way to do this
                    float scale = 1;
                    float posx = 0;
                    float posy = 0;
                    float posz = 0;
                    float rotx = 0;
                    float roty = 0;
                    float rotz = 0;
                    try { scale = (item.transform.scale != "") ? (float)Convert.ToSingle(item.transform.scale) : 1; }
                    catch (Exception e) { Debug.Log($"Error converting scale to float for model {item.name}, error {e.Message}"); }
                    try { posx = (item.transform.position.x != "") ? (float)Convert.ToSingle(item.transform.position.x) : 0; }
                    catch (Exception e) { Debug.Log($"Error converting position x to float for model {item.name}, error {e.Message}"); }
                    try { posy = (item.transform.position.y != "") ? (float)Convert.ToSingle(item.transform.position.y) : 0; }
                    catch (Exception e) { Debug.Log($"Error converting position y to float for model {item.name}, error {e.Message}"); }
                    try { posz = (item.transform.position.z != "") ? (float)Convert.ToSingle(item.transform.position.z) : 0; }
                    catch (Exception e) { Debug.Log($"Error converting position z to float for model {item.name}, error {e.Message}"); }
                    try { rotx = (item.transform.rotation.x != "") ? (float)Convert.ToSingle(item.transform.rotation.x) : 0; }
                    catch (Exception e) { Debug.Log($"Error converting rotation x to float for model {item.name}, error {e.Message}"); }
                    try { roty = (item.transform.rotation.y != "") ? (float)Convert.ToSingle(item.transform.rotation.y) : 0; }
                    catch (Exception e) { Debug.Log($"Error converting rotation y to float for model {item.name}, error {e.Message}"); }
                    try { rotz = (item.transform.rotation.z != "") ? (float)Convert.ToSingle(item.transform.rotation.z) : 0; }
                    catch (Exception e) { Debug.Log($"Error converting rotation z to float for model {item.name}, error {e.Message}"); }

                    ResourceDisplayRules rule = ResourceDisplayRules.Static;
                    switch (item.rules)
                    {
                        case "interactive":
                            rule = ResourceDisplayRules.Interactive;
                            break;
                        case "marker":
                            rule = ResourceDisplayRules.Marker;
                            break;
                        case "library":
                            rule = ResourceDisplayRules.Library;
                            break;
                        default:
                            rule = ResourceDisplayRules.Static;
                            break;
                    }

                    project.resources.Add(new DialogueResource()
                    {
                        name = item.name,
                        url = item.source,
                        displayRule = rule,
                        scale = scale,
                        position = new Vector3(posx, posy, posz),
                        rotation = new Vector3(rotx, roty, rotz),
                        markerURL = item.marker,
                    });
                }
            }

            Debug.Log("Adding proposals");

            // Add our proposals if we have any
            if (acf.proposals != null)
            {
                project.proposals = new List<Proposal>();
                foreach (var item in acf.proposals)
                {
                    project.proposals.Add(JsonUtility.FromJson<Proposal>(item.json));
                }
            }
            Debug.Log("Created project from wordpress data: " + project.name);
            return project;
        }
    }

    [System.Serializable]
    public class Acf_Dialogues : Acf
    {
        public DialogueSettings settings;
        public Model[] models;
        public ProposalJSON[] proposals;
    }

    [System.Serializable]
    public class DialogueSettings
    {
        public string description;
        public string image;
        public string scale;
        public string groundplane;
        public Marker marker;
        public Geolocation geolocation;
    }

    [System.Serializable]
    public class Marker
    {
        public string image;
        public string required;
    }

    [System.Serializable]
    public class Geolocation
    {
        public string latitude;
        public string longitude;
    }

    [System.Serializable]
    public class Model
    {
        public string name;
        public string source;
        public string rules;
        public string marker;
        public ModelTransform transform;
    }

    [System.Serializable]
    public class ModelTransform
    {
        public string scale;
        public ModelPosition position;
        public ModelRotation rotation;
    }

    [System.Serializable]
    public class ModelPosition
    {
        public string x;
        public string y;
        public string z;
    }
    [System.Serializable]
    public class ModelRotation
    {
        public string x;
        public string y;
        public string z;
    }

    [System.Serializable]
    public class ProposalJSON
    {
        public string name;
        public string json;
    }
}