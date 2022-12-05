using System.Collections.Generic;
using Data.Dialogs;

namespace Pladdra.Workspace
{
    public class WorkspaceConfiguration
    {
        public WorkspaceOrigin Origin { get; set; }
        public IWorkspaceMarker Marker { get; set; }
        public IWorkspacePlane Plane { get; set; }
        public IWorkspaceResourceCollection ResourceCollection { get; set; }
        
        public DialogScene Scene { get; set; }
        public List<DialogScene> FeaturedScenes { get; set; }
    }
}