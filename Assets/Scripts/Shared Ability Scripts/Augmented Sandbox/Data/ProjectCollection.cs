using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pladdra.Data
{
    /// <summary>
    /// Client side representation of a project collection.
    /// </summary>
    [System.Serializable]
    public class ProjectCollection
    {
        public ProjectCollection(string name, string description, Texture2D image, List<int> projectIds)
        {
            this.name = name;
            this.description = description;
            this.image = image;
            this.projectIds = projectIds;
        }
        public string name;
        public string description;
        public Texture2D image;
        public List<int> projectIds;
    }

    /// <summary>
    /// Wordpress side representation of a project collection.
    /// </summary>
    [System.Serializable]
    public class WordpressData_ProjectCollections
    {
        public WordpressData_ProjectCollection[] collections;

        public List<ProjectCollection> MakeProjectCollections()
        {
            List<ProjectCollection> dialogueCollections = new List<ProjectCollection>();

            foreach (WordpressData_ProjectCollection dialogueCollection in collections)
            {
                List<int> projectIds = new List<int>();
                foreach (DialogueCollection_Projects project in dialogueCollection.acf.projects)
                {
                    try
                    {
                        projectIds.Add(Int32.Parse(project.id));
                    }
                    catch (FormatException e)
                    {
                        Debug.Log($"Could not parse {project.id} to int, error: {e.Message}");
                    }
                }
                dialogueCollections.Add(new ProjectCollection(dialogueCollection.title.rendered, dialogueCollection.acf.description, null, projectIds));
            }
            return dialogueCollections;
        }
    }

    [System.Serializable]
    public class WordpressData_ProjectCollection
    {
        public Title title;
        public DialogueCollection_Acf acf;

        public ProjectCollection MakeProjectCollection()
        {
            List<int> projectIds = new List<int>();
            foreach (DialogueCollection_Projects project in acf.projects)
            {
                try
                {
                    projectIds.Add(Int32.Parse(project.id));
                }
                catch (FormatException e)
                {
                    Debug.Log($"Could not parse {project.id} to int, error: {e.Message}");
                }
            }
            return new ProjectCollection(title.rendered, acf.description, null, projectIds);
        }
    }

    [System.Serializable]
    public class DialogueCollection_Acf
    {
        public string description;
        public string image;
        public DialogueCollection_Projects[] projects;
    }

    [System.Serializable]
    public class DialogueCollection_Projects
    {
        public string id;
    }
}