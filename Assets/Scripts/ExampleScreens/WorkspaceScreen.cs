using Abilities;
using UnityEngine;
using Pladdra.Workspace;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class WorkspaceScreen : Screen
    {
        private IAbility Ability { get; set; }

        private WorkspaceConfiguration Configuration { get; set; }

        public GameObject OriginPrefab;

        public void Configure(IAbility ability, WorkspaceConfiguration wc)
        {
            Ability = ability;
            Configuration = wc;
        }


        protected override void BeforeActivateScreen()
        {
        }

        protected override void AfterActivateScreen()
        {
            Configuration.Origin.go = Configuration.Origin.go ?? Instantiate(OriginPrefab);
            FindObjectOfType<WorkspaceManager>()
                .Activate(Ability, Configuration);
        }

        protected override void AfterDeactivateScreen()
        {
            FindObjectOfType<WorkspaceManager>().Deactivate();
        }
    }
}