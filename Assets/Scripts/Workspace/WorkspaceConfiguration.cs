using Data.Dialogs;

namespace Workspace
{
    public class WorkspaceConfiguration
    {
        public WorkspaceOrigin Origin { get; set; }
        public IWorkspacePlane Plane { get; set; }
        public IWorkspaceResourceCollection ResourceCollection { get; set; }
        
        public DialogScene Scene { get; set; } 
    }
}