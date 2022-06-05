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
using Workspace;

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
        
        public IEnumerator LoadWorkspace(Action<WorkspaceConfiguration> callback)
        {
            DialogProject project = null;
            yield return LogTask("Nu hämtar vi definitioner från servern!",
                () => new LoadExternalProject(Repository, p => project = p));

            var modelUrls = project.Resources.Select(resource => resource.ModelUrl)
                .Concat(project.Resources.Select(resource => resource.MarkerModelUrl))
                .Where(s => !string.IsNullOrEmpty(s));
            
            var markerImageUrls = new[] { project?.Marker?.Image }
                .Where(s => !string.IsNullOrEmpty(s));

            var wrm = CreateWebResourceManager();
            Dictionary<string, string> url2path = null;
            yield return LogTask("Vi tankar ner alla modeller för att det ska gå snabbare sen!",
                () => new MapExternalResourceToLocalPaths(
                    wrm,
                    modelUrls.Concat(markerImageUrls),
                    p => url2path = p));

            var path2model = new Dictionary<string, GameObject>();
            foreach (var path in modelUrls.Select(url => url2path[url]).DistinctBy(path => path))
            {
                yield return LogTask(
                    $"Nu läser vi in en 3d modell som heter {Path.GetFileNameWithoutExtension(path).Split('.').Last()} i minnet!",
                    () => new Load3dModel(path, go => { path2model[path] = go; }));
            }
            
            var path2Thumbnail = new Dictionary<string, Texture2D>();
            foreach (var path in modelUrls.Select(url => url2path[url]).DistinctBy(path => path))
            {
                yield return LogTask(
                    $"Nu skapar vi thumbnail för {Path.GetFileNameWithoutExtension(path).Split('.').Last()}!",
                    () => new GenerateThumbnail(path2model[path], thumbnail => path2Thumbnail[path] = thumbnail));
            }

            var modelItems = LogAction("Nu skapar vi modeller!", () => project.Resources
                .Where(resource => resource.Type == "model")
                .Select(resource => new
                {
                    resource,
                    model = path2model.TryGet(url2path.TryGet(resource.ModelUrl)),
                    marker = path2model.TryGet(url2path.TryGet(resource.MarkerModelUrl)),
                    thumbnail = path2Thumbnail.TryGet(url2path.TryGet(resource.ModelUrl)),
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
                Plane = new WorkspacePlane()
                {
                    Width = project.Plane?.Width ?? 4,
                    Height = project.Plane?.Height ?? 4
                },
                ResourceCollection = CreateWorkspaceResourceCollection(project, allResources.ToList()),
                FeaturedScenes = project.FeaturedScenes ?? new List<DialogScene>()
            });

            callback(configuration);
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