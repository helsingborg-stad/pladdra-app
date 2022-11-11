using System.Collections.Generic;
using Data.Dialogs;
using Pladdra.Data;

namespace Pladdra.Workspace
{
    public class WorkspaceConfiguration
    {
        public WorkspaceOrigin Origin { get; set; }
        public IWorkspaceMarker Marker { get; set; }
        public IWorkspacePlane Plane { get; set; }
        public IWorkspaceResourceCollection ResourceCollection { get; set; }
        
        public DialogScene Scene { get; set; }
        public List<UserProposal> UserProposals { get; set; }
    }
}