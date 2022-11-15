using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Dialogs;
using Pipelines;
using Unity.VisualScripting;
using UnityEngine;
using Utility;
using Pladdra.Workspace;
using Pladdra.Data;

namespace Abilities.ARRoomAbility
{
    public class ArRoomWorkspaceLoader: AbstractPipeline
    {
        public event Action<string> OnTaskStarted;
        public event Action<string> OnTaskDone;
        public event Action<string, int> OnTaskProgress;

        private IDialogProjectRepository Repository { get; }
        public ArRoomWorkspaceLoader(IDialogProjectRepository repository)
        {
            Repository = repository;
            OnTaskStarted += label => Debug.Log($"[pipline] {label}...");
            OnTaskDone += label => Debug.Log($"[pipline] done {label}");
            OnTaskProgress += (label, step) => { };
        }
        
        public IEnumerator LoadWorkspace(Action<WorkspaceConfiguration, Project> callback)
        {
            // DialogProject project = null;
            Project project = null;
            
            yield return LogTask("Nu hämtar vi definitioner från servern!",
                () => new LoadExternalProject(Repository, p => project = p));

            var ModelURLs = project.Resources.Select(resource => resource.ModelURL)
                .Concat(project.Resources.Select(resource => resource.ModelIconURL))
                .Where(s => !string.IsNullOrEmpty(s));
            
            var markerImageUrls = new[] { project?.Marker?.Image }
                .Where(s => !string.IsNullOrEmpty(s));

            var wrm = CreateWebResourceManager();
            Dictionary<string, string> url2path = null;
            yield return LogTask("Vi tankar ner alla modeller för att det ska gå snabbare sen!",
                () => new MapExternalResourceToLocalPaths(
                    wrm,
                    ModelURLs.Concat(markerImageUrls),
                    p => url2path = p));

            var path2model = new Dictionary<string, GameObject>();
            foreach (var path in ModelURLs.Select(url => url2path[url]).DistinctBy(path => path))
            {
                yield return LogTask(
                    $"Nu läser vi in en 3d modell som heter {Path.GetFileNameWithoutExtension(path).Split('.').Last()} i minnet!",
                    () => new Load3dModel(path, go =>
                    {
                        // TODO: Decide if this is the best place for this stuff
                        if (go.GetComponent<Animation>())
                            go.GetComponent<Animation>().playAutomatically = true;

                            // TODO Add gameobject to pladdra resource
                        
                        path2model[path] = go;
                    }));
            }

            // TODO Load video
            // TODO Load audio
            // TODO Load asset bundles
            
            var path2Preview = new Dictionary<string, Texture2D>();
            foreach (var path in ModelURLs.Select(url => url2path[url]).DistinctBy(path => path))
            {
                yield return LogTask(
                    $"Nu tar vi en bild på en 3d modell som heter {Path.GetFileNameWithoutExtension(path).Split('.').Last()}!",
                    () => new LoadPreview(path2model[path], texture => path2Preview[path] = texture));
            }

            var modelItems = LogAction("Nu skapar vi modeller!", () => project.Resources
                .Where(resource => resource.Type == "model")
                .Select(resource => new
                {
                    resource,
                    model = path2model.TryGet(url2path.TryGet(resource.ModelURL)),
                    marker = path2model.TryGet(url2path.TryGet(resource.ModelIconURL)),
                    thumbnail = path2Preview.TryGet(url2path.TryGet(resource.ModelURL)),
                })
                .Where(o => o.model != null)
                .Where(o => o.marker != null)
                .Where(o => o.thumbnail != null)
                .Select(o => CreateWorkspaceResource(o.resource, o.thumbnail, o.marker, o.model))
                .Where(item => item != null)
                .ToList());
            
            Texture2D markerImageTexture = null;
            if (project?.Marker?.Image != null)
            {
                yield return LogTask("Nu laddar vi in en markörbild!", () => 
                new Load2dTexture(url2path[project.Marker.Image], t =>  
                {
                    markerImageTexture = t;
                }));
            }

            var allResources = modelItems;

            var configuration = LogAction("Här skapas det en konfiguration minsann!", () => new WorkspaceConfiguration
            {
                Marker = markerImageTexture ? new WorkspaceMarker()
                {
                    Image = markerImageTexture,
                    Width = project.Marker.Width,
                    Height = project.Marker.Height
                } : null,
                Origin = new WorkspaceOrigin(),
                // Plane = new WorkspacePlane()
                // {
                //     Width = project.Plane?.Width ?? 4,
                //     Height = project.Plane?.Height ?? 4
                // },
                ResourceCollection = CreateWorkspaceResourceCollection(project, allResources.ToList()),
                UserProposals = project.UserProposals ?? new List<UserProposal>()
            });

            callback(configuration, project);
        }

        protected override IPipelineTaskEvents CreateEvents(string label)
        {
            return new PipelineTaskEvents(
                () => OnTaskStarted?.Invoke(label),
                step => OnTaskProgress?.Invoke(label, step),
                () => OnTaskDone?.Invoke(label));
        }
    }
}