using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Data.Dialogs;
using Repository;
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
            CreateDialogProjectRepository = () =>
            {
                throw new ApplicationException("No factory for Dialog Project Repository is specified");
            };
            CreateWebResourceManager = () => new WebResourceManager(Path.Combine(Application.temporaryCachePath, "resources"));
            CreateWorkspaceResource = (resource, prefab) => new WorkspaceResource { Prefab = prefab, ResourceID = resource.Url };
            CreateWorkspaceResourceCollection = (project, items) => new WorkspaceResourceCollection { Resources = items };

            OnTaskStarted += label => Debug.Log($"[pipline] {label}...");
            OnTaskDone += label => Debug.Log($"[pipline] done {label}");
            OnTaskProgress += (label, step) => { };
            // OnTaskProgress += (label, step) => Debug.Log($"[pipline] {new String('.', step)}");
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
                    if (w)
                    {
                        Pipeline.OnTaskProgress(Label, ++Step);
                    }
                    else
                    {
                        Pipeline.OnTaskDone(Label);
                    }
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
            
            var repo = Wrap("Nu kör vi!", () => CreateDialogProjectRepository());

            DialogProject project = null;
            yield return WrapEnumerator("Nu hämtar vi definitioner från servern!", () => new LoadExternalProject(repo, p => project = p));

            yield return new WaitForSeconds(4);
            
            var wrm = CreateWebResourceManager();
            Dictionary<string, string> url2path = null;
            yield return WrapEnumerator("Vi tankar ner alla modeller för att det ska gå snabbare sen!", () => new MapExternalResourceToLocalPaths(wrm, project, p => url2path = p));

            var path2model = new Dictionary<string, GameObject>();
            foreach (var path in url2path.Values)
            {
                yield return WrapEnumerator($"Nu läser vi in en 3d modell som heter {Path.GetFileNameWithoutExtension(path).Split('.').Last()} i minnet!", () => new Load3dModel(path, go =>
                {
                    path2model[path] = go;
                }));
            }

            var modelItems = Wrap("Nu skapar vi modeller!", () => project.Resources
                    .Where(resource => resource.Type == "model")
                    .Select(resource => new { resource, gameObject = path2model.TryGet(url2path.TryGet(resource.Url)) })
                    .Where(o => o.gameObject != null)
                    .Select(o => CreateWorkspaceResource(o.resource, o.gameObject))
                    .Where(item => item != null)
                    .ToList());

            var markerItems = Wrap("Nu skapar vi markörer", () => project.Resources
                    .Where(resource => resource.Type == "marker")
                    .Select(resource => new { resource, gameObject = path2model.TryGet(url2path.TryGet(resource.Url)) })
                    .Where(o => o.gameObject != null)
                    .Select(o => CreateWorkspaceResource(o.resource, o.gameObject))
                    .Where(item => item != null)
                    .ToList());


            var allResources = modelItems.Concat(markerItems);
            
            var configuration = Wrap("Här skapas det en konfiguration minsann!", () => new WorkspaceConfiguration
            {
                Origin = new WorkspaceOrigin(),
                Plane = new WorkspacePlane
                {
                    Width = 6,
                    Height = 12
                },
                ResourceCollection = CreateWorkspaceResourceCollection(project, allResources.ToList()),
            });

            callback(configuration);
        }
    }
}
