using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Dialogs;
using Pipelines;
using Repository;
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


            var wrm = CreateWebResourceManager();
            Dictionary<string, string> url2path = null;
            yield return LogTask("Vi tankar ner alla modeller för att det ska gå snabbare sen!",
                () => new MapExternalResourceToLocalPaths(wrm, project, p => url2path = p));

            var path2model = new Dictionary<string, GameObject>();
            foreach (var path in url2path.Values)
            {
                yield return LogTask(
                    $"Nu läser vi in en 3d modell som heter {Path.GetFileNameWithoutExtension(path).Split('.').Last()} i minnet!",
                    () => new Load3dModel(path, go => { path2model[path] = go; }));
            }

            var modelItems = LogAction("Nu skapar vi modeller!", () => project.Resources
                .Where(resource => resource.Type == "model")
                .Select(resource => new { resource, gameObject = path2model.TryGet(url2path.TryGet(resource.Url)) })
                .Where(o => o.gameObject != null)
                .Select(o => CreateWorkspaceResource(o.resource, o.gameObject))
                .Where(item => item != null)
                .ToList());

            var markerItems = LogAction("Nu skapar vi markörer", () => project.Resources
                .Where(resource => resource.Type == "marker")
                .Select(resource => new { resource, gameObject = path2model.TryGet(url2path.TryGet(resource.Url)) })
                .Where(o => o.gameObject != null)
                .Select(o => CreateWorkspaceResource(o.resource, o.gameObject))
                .Where(item => item != null)
                .ToList());


            var allResources = modelItems.Concat(markerItems);

            var configuration = LogAction("Här skapas det en konfiguration minsann!", () => new WorkspaceConfiguration
            {
                Origin = new WorkspaceOrigin(),
                Plane = new WorkspacePlane()
                {
                    Width = project.Plane?.Width ?? 4,
                    Height = project.Plane?.Height ?? 4
                },
                ResourceCollection = CreateWorkspaceResourceCollection(project, allResources.ToList()),
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