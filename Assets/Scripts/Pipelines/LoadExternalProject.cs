using System;
using Abilities.ARRoomAbility;
using Pladdra.Data;

namespace Pipelines
{
    public class LoadExternalProject : TaskYieldInstruction<Project>
    {
        public LoadExternalProject(IDialogProjectRepository repository, Action<Project> callback) : base(() => repository.Load(), callback) { }
    }
}
