using System;
using System.Collections.Generic;
using System.Linq;
using Pipelines;
using Repository;
using Screens;
using UnityEngine.UIElements;
using Workspace.Hud;
using Screen = Screens.Screen;

namespace ExampleScreens
{

    public class LoadProjectsScreen : Screen
    {
        private void Start()
        {
            // this action is updated to map to a label in our HUD
            Action<string> setLabelText = s => { };
            Action updateUI = () => { };
            var actions = new List<string>();
            
            // show progress HUD
            FindObjectOfType<HudManager>().UseHud("app-is-loading-project-hud", root =>
            {
                var labelElement = root.Q<Label>("label");
                setLabelText = s => labelElement.text = s;
                updateUI = () => labelElement.text = string.Join("\r\n", actions);
            });

            
            // configure pipeline
            var pipeline = new Pipeline()
            {
                CreateDialogProjectRepository = FindObjectOfType<RepositoryManager> // () => new SampleDialogProjectRepository(Application.temporaryCachePath)

            };
            pipeline.OnTaskStarted += label =>
            {
                actions.Add(label);
                updateUI();
            };
            // pipeline.OnTaskDone += label => { setLabelText("") };
            // pipeline.OnTaskProgress += (label, step) => setLabelText($"{label} {new String('.', step)}");

            // run pipeline, and when done: clear hud, transition to another screen
            StartCoroutine(pipeline.LoadWorkspace((configuration) =>
            {
                updateUI = () => { };
                setLabelText = s => { };
                FindObjectOfType<HudManager>().ClearHud();
                GetComponentInParent<ScreenManager>().SetActiveScreen<WorkspaceScreen>(
                    beforeActivate: screen => screen.SetWorkspaceConfiguration(configuration)
                );
            }));
        }
    }
}