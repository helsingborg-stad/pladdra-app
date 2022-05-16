using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Dialogs;
using UnityEngine;
using Utility;
using Workspace;

namespace Pipelines
{
    public class Pipeline
    {
        public event Action<string> OnTaskStarted;
        public event Action<string> OnTaskDone;
        public event Action<string, int> OnTaskProgress;
        
        public Func<IDialogProjectRepository> CreateDialogProjectRepository { get; set; }

        public Func<IWebResourceManager> CreateWebResourceManager { get; set; }

        public Func<DialogResource, GameObject, IWorkspaceResource> CreateWorkspaceResource { get; set; }

        public Func<DialogProject, IEnumerable<IWorkspaceResource>, IWorkspaceResourceCollection> CreateWorkspaceResourceCollection { get; set; }

        public Pipeline()
        {
            CreateDialogProjectRepository = () => new SampleDialogProjectRepository(Application.temporaryCachePath);
            CreateWebResourceManager = () => new WebResourceManager(Application.temporaryCachePath);
            CreateWorkspaceResource = (resource, prefab) => new WorkspaceResource { Prefab = prefab, ResourceID = resource.Url };
            CreateWorkspaceResourceCollection = (project, items) => new WorkspaceResourceCollection { Resources = items };

            OnTaskStarted += label => Debug.Log($"[pipline] {label}...");
            OnTaskDone += label => Debug.Log($"[pipline] done {label}");
            OnTaskProgress += (label, step) => Debug.Log($"[pipline] {"................".Substring(0, step)}");
        }

        private CustomYieldInstruction WrapEnumerator<T>(string label, Func<T> factory) where T : CustomYieldInstruction
        {
            return new LoggingInstruction(this, label, factory());
        }

        private class LoggingInstruction : CustomYieldInstruction
        {
            public Pipeline Pipeline { get; }
            public string Label { get; }
            public CustomYieldInstruction Inner { get; }
            
            private int Step { get; set; }

            public LoggingInstruction(Pipeline pipeline, string label, CustomYieldInstruction inner)
            {
                Pipeline = pipeline;
                Label = label;
                Inner = inner;
                Pipeline.OnTaskStarted(label);
            }
            public override bool keepWaiting {
                get
                {
                    var w = Inner.keepWaiting;
                    if (!w)
                    {
                        Pipeline.OnTaskProgress(Label, ++Step);
                    }
                    Pipeline.OnTaskDone(Label);
                    return w;
                }
            }
        }

        private T Wrap<T>(string label, Func<T> factory)
        {
            OnTaskStarted(label);
            var result = factory();
            OnTaskDone(label);
            return result;
        }
        
        public IEnumerator LoadWorkspace(Action<WorkspaceConfiguration> callback)
        {
            
            var repo = Wrap("loading external project", () => CreateDialogProjectRepository());

            DialogProject project = null;
            yield return WrapEnumerator("loading external project", () => new LoadExternalProject(repo, p => project = p));

            yield return new WaitForSeconds(4);
            
            var wrm = CreateWebResourceManager();
            Dictionary<string, string> url2path = null;
            yield return WrapEnumerator("mapping external resources to local", () => new MapExternalResourceToLocalPaths(wrm, project, p => url2path = p));

            var path2model = new Dictionary<string, GameObject>();
            foreach (var path in url2path.Values)
            {
                yield return WrapEnumerator($"loading 3d model {path}", () => new Load3dModel(path, go =>
                {
                    path2model[path] = go;
                }));
            }

            var modelItems = Wrap("creating model items", () => project.Resources
                    .Where(resource => resource.Type == "model")
                    .Select(resource => new { resource, gameObject = path2model.TryGet(url2path.TryGet(resource.Url)) })
                    .Where(o => o.gameObject != null)
                    .Select(o => CreateWorkspaceResource(o.resource, o.gameObject))
                    .Where(item => item != null)
                    .ToList());

            var markerItems = Wrap("creating marker items", () => project.Resources
                    .Where(resource => resource.Type == "marker")
                    .Select(resource => new { resource, gameObject = path2model.TryGet(url2path.TryGet(resource.Url)) })
                    .Where(o => o.gameObject != null)
                    .Select(o => CreateWorkspaceResource(o.resource, o.gameObject))
                    .Where(item => item != null)
                    .ToList());


            var allResources = modelItems.Concat(markerItems);

            var cosmos = Wrap("faking cosmos", () => new WorkspaceCosmos
            {
                SpaceItems = /* fake */ allResources
                    .Select((resource, index) => new WorkspaceItemInSpace
                    {
                        ResourceId = resource.ResourceID,
                        Position = new Vector3(index, 0, 0),
                        Scale = new Vector3(1, 1, 1),
                        Rotation = new Quaternion()
                    }).ToList()
            });

            var configuration = Wrap("creating workspace configuration", () => new WorkspaceConfiguration
            {
                Origin = new WorkspaceOrigin(),
                Cosmos = cosmos,
                Plane = new WorkspacePlane
                {
                    Width = 6,
                    Height = 12
                },
                ResourceCollection = CreateWorkspaceResourceCollection(project, modelItems.Concat(markerItems)
                    .ToList())
            });

            callback(configuration);
        }
    }
}
