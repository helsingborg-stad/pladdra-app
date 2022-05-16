using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;
using Workspace;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class WorkspaceScreen : Screen
    {
        public WorkspaceConfiguration configuration { get; private set; }

        private void Start()
        {
        }

        public void SetWorkspaceConfiguration(WorkspaceConfiguration wc)
        {
            configuration = wc;
        }

        protected override void BeforeActivateScreen()
        {
        }

        protected override void AfterActivateScreen()
        {
            FindObjectOfType<WorkspaceManager>()
                .Activate(configuration);
        }
    }
}