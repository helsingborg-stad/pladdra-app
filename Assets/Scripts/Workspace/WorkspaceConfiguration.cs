namespace Workspace
{
    public class WorkspaceConfiguration
    {
        public WorkspaceOrigin Origin { get; set; }
        public IWorkspacePlane Plane { get; set; }
        public IWorkspaceCosmos Cosmos { get; set; }
        public IWorkspaceResourceCollection ResourceCollection { get; set; }
    }
}