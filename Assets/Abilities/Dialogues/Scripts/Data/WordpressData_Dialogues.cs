using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pladdra.Data;
using UntoldGarden.Utils;

namespace Pladdra.ARSandbox.Dialogues.Data
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

            float startScale = acf.settings.scale.TryConvertToSingle($"project {name}");
            double lat = acf.settings.geolocation.latitude.TryConvertToDouble($"project {name}");
            double lon = acf.settings.geolocation.longitude.TryConvertToDouble($"project {name}");
            float rotation = acf.settings.geolocation.rotation.TryConvertToSingle($"project {name}");
            bool markerRequired = acf.settings.marker.required.TryConvertToBoolean("marker required in " + name);
            float markerWidth = acf.settings.marker.width.TryConvertToSingle($"project {name}");

            Project project = new Project()
            {
                id = id,
                name = name,
                description = acf.settings.description,
                startScale = startScale,
                marker = (acf.settings.marker.image, markerRequired, markerWidth, null),
                location = (lat, lon, rotation),
            };

            project.resources = new List<DialogueResource>();

            // Add our groundplane if we have one
            if (!acf.settings.groundplane.IsNullOrEmptyOrFalse())
            {
                project.resources.Add(new DialogueResource()
                {
                    name = "groundplane",
                    url = acf.settings.groundplane,
                });
            }

            // Add resources
            if (acf.models != null)
            {

                foreach (var item in acf.models)
                {
                    if (item.source == "" || item.source == "false")
                        continue;

                    float scale = item.transform.scale.TryConvertToSingle($"model {item.name}");
                    float posx = item.transform.position.x.TryConvertToSingle($"model {item.name}");
                    float posy = item.transform.position.y.TryConvertToSingle($"model {item.name}");
                    float posz = item.transform.position.z.TryConvertToSingle($"model {item.name}");
                    float rotx = item.transform.rotation.x.TryConvertToSingle($"model {item.name}");
                    float roty = item.transform.rotation.y.TryConvertToSingle($"model {item.name}");
                    float rotz = item.transform.rotation.z.TryConvertToSingle($"model {item.name}");


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

                    float width = item.marker.width.TryConvertToSingle($"model {item.name}");

                    project.resources.Add(new DialogueResource()
                    {
                        name = item.name,
                        url = item.source,
                        displayRule = rule,
                        scale = scale,
                        position = new Vector3(posx, posy, posz),
                        rotation = new Vector3(rotx, roty, rotz),
                        marker = (item.marker.image, width),
                    });
                }
            }

            // Add our proposals if we have any
            if (acf.proposals != null)
            {
                project.proposals = new List<Proposal>();
                foreach (var item in acf.proposals)
                {
                    project.proposals.Add(JsonUtility.FromJson<Proposal>(item.json));
                }
            }
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
        public string width;
    }

    [System.Serializable]
    public class Geolocation
    {
        public string latitude;
        public string longitude;
        public string rotation;
    }

    [System.Serializable]
    public class Model
    {
        public string name;
        public string source;
        public string rules;
        public Marker marker;
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