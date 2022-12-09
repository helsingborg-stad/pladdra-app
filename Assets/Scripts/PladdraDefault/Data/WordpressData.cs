using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.DefaultAbility.Data
{
    /// <summary>
    /// Helper object for deserializing json from wordpress.
    /// </summary>
    [System.Serializable]
    public class WordpressData
    {
        public string id;
        public Title title;
        public Acf acf;

        /// <summary>
        /// Creates a Project object from the data in the wordpress JSON.
        /// </summary>
        /// <returns>Project</returns>
        public Project MakeProject()
        {
            Project project = new Project()
            {
                id = id,
                name = acf.name,
                description = acf.description,
                startScale = (acf.scale != "") ? float.Parse(acf.scale) : 1,
                markerURL = acf.marker,
            };

            // Add our groundplane if we have one
            if (acf.groundplane != "")
            {
                project.groundPlane = new PladdraResource()
                {
                    name = "groundplane",
                    modelURL = acf.groundplane,
                };
            }
            // Add our static resources if we have any
            if (acf.staticResources != null)
            {
                project.staticResources = new List<PladdraResource>();
                foreach (var item in acf.staticResources)
                {
                    project.staticResources.Add(new PladdraResource()
                    {
                        name = item.name,
                        modelURL = item.source,
                        scale = (item.scale != "") ? float.Parse(item.scale) : 1,
                        markerURL = item.marker,
                    });
                }
            }
            // Add our resources if we have any
            if (acf.resources != null)
            {
                project.resources = new List<PladdraResource>();
                foreach (var item in acf.resources)
                {
                    project.resources.Add(new PladdraResource()
                    {
                        name = item.name,
                        modelURL = item.source,
                    });
                }
            }
            return project;
        }
    }

    [System.Serializable]
    public class Title
    {
        public string rendered;
    }

    [System.Serializable]
    public class Acf
    {
        public string name;
        public string description;
        public string marker;
        public string scale;
        public string groundplane;
        public Model[] resources;
        public StaticModel[] staticResources;
    }

    [System.Serializable]
    public class Model
    {
        public string name;
        public string source;
    }

    [System.Serializable]
    public class StaticModel
    {
        public string name;
        public string source;
        public string marker;
        public string scale;

    }
}