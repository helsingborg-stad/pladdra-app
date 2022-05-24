using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Data.Dialogs;
using UnityEngine;
using Workspace;

namespace Pipelines
{
    public abstract class AbstractPipeline
    {

        protected abstract IPipelineTaskEvents CreateEvents(string label);

        protected virtual IWebResourceManager CreateWebResourceManager(string cacheSubFolder = "resources") =>
            new WebResourceManager(Path.Combine(Application.temporaryCachePath, cacheSubFolder));
        protected virtual IWorkspaceResourceCollection CreateWorkspaceResourceCollection(DialogProject project, IEnumerable<IWorkspaceResource> resources) => new WorkspaceResourceCollection { Resources = resources.ToList() };

        protected virtual IWorkspaceResource CreateWorkspaceResource(DialogResource resource, params GameObject[] prefabs) =>
            new WorkspaceResource() { Prefabs = prefabs, ResourceID = resource.Id };

        protected virtual CustomYieldInstruction LogTask(string label, Func<CustomYieldInstruction> factory) => LogTask(CreateEvents(label), factory);

        protected virtual CustomYieldInstruction LogTask(IPipelineTaskEvents events, Func<CustomYieldInstruction> factory)
        {
            return new LoggingYieldInstruction(events, factory());
        }

        protected virtual T LogAction<T>(string label, Func<T> factory) => LogAction(CreateEvents(label), factory);

        protected virtual T LogAction<T>(IPipelineTaskEvents events, Func<T> factory)
        {
            events?.TaskStarted();
            var result = factory();
            events?.TaskFinished();
            return result;
        }

    }
}