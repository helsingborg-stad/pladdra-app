using System;
using System.Collections.Generic;
using Abilities;
using Abilities.ARRoomAbility;
using Screens;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;
using Workspace.Hud;
using Screen = Screens.Screen;

namespace ExampleScreens
{
    public class LoadProjectsScreen : Screen
    {
        public bool UseARSceneAfterLoad;
        public IAbility Ability { get; set; }

        private void Start()
        {
            // this action is updated to map to a label in our HUD
            Action<string> setLabelText = s => { };
            Action updateUI = () => { };
            var actions = new List<string>();
            
            // show progress HUD
            HudManager.UseHud("app-is-loading-project-hud", root =>
            {
                var labelElement = root.Q<Label>("label");
                setLabelText = s => labelElement.text = s;
                updateUI = () => labelElement.text = string.Join("\r\n", actions);
            });

            
            var pipeline = new ArRoomWorkspaceLoader(Ability.Repository);
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
                HudManager.ClearHud();

                var screenHandlers = new Dictionary<string, Action>()
                {
                    {
                        "Default", () => ScreenManager.SetActiveScreen<WorkspaceScreen>(
                            beforeActivate: screen => screen.Configure(Ability, configuration)
                        )
                    },
                    {
                        "AR", () => ScreenManager.SetActiveScreen<ARDetectionScreen>(
                            beforeActivate: screen => screen.Configure(Ability, configuration)
                        )
                    }
                };

                screenHandlers[UseARSceneAfterLoad ? "AR" : "Default"]();
            }).Catch(e =>
            {
                ScreenManager.SetActiveScreen<ErrorScreen>(
                    beforeActivate: screen => screen.Configure(e));
            }));
        }
    }
}