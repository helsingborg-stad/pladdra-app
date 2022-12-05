using System;
using Abilities.ARRoomAbility;
using Data.Dialogs;

namespace Pipelines
{
    public class LoadExternalProject : TaskYieldInstruction<DialogProject>
    {
        public LoadExternalProject(IDialogProjectRepository repository, Action<DialogProject> callback) : base(() => repository.Load(), callback) { }
    }
}