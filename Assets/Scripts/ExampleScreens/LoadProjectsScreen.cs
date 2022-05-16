using System;
using Data.Dialogs;
using DefaultNamespace;
using Pipelines;
using Screens;
using UnityEngine;
using UnityEngine.UIElements;
using Screen = Screens.Screen;

namespace ExampleScreens
{

    public class LoadProjectsScreen : Screen
    {
        private void Start()
        {
            // this action is updated to map to a label in our HUD
            Action<string> setLabelText = s => { };
            
            // show progress HUD
            FindObjectOfType<HudManager>().UseHud("app-is-loading-project-hud", root =>
            {
                var labelElement = root.Q<Label>();
                setLabelText = s => labelElement.text = s;
            });

            // configure pipeline
            var pipeline = new Pipeline()
            {
                CreateDialogProjectRepository = () => new SampleDialogProjectRepository(Application.temporaryCachePath)

            };
            pipeline.OnTaskStarted += label => setLabelText(label);
            pipeline.OnTaskDone += label => setLabelText("");
            pipeline.OnTaskProgress += (label, step) => setLabelText($"{label} {"............".Substring(0, step)}");

            // run pipeline, and when done: clear hud, transition to another screen
            StartCoroutine(pipeline.LoadWorkspace((configuration) =>
            {
                setLabelText = s => { };
                FindObjectOfType<HudManager>().ClearHud();
                GetComponentInParent<ScreenManager>().SetActiveScreen<WorkspaceScreen>(
                    beforeActivate: screen => screen.SetWorkspaceConfiguration(configuration)
                );
            }));
        }
    }
}