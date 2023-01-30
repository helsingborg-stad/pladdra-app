using System.Collections;
using System.Collections.Generic;
using Pladdra.DialogueAbility.UX;
using Pladdra.UI;
using Pladdra.UX;
using UnityEngine;

namespace Pladdra.QuizAbility
{
    public class MenuManager_Quiz : Pladdra.UI.MenuManager
    {
        protected QuizManager quizManager { get { return transform.parent.gameObject.GetComponentInChildren<QuizManager>(); } }

        protected override void Start()
        {
            base.Start();
            menuItems.Add(new MenuItem()
            {
                id = "project-list",
                name = "Hem",
                action = () =>
                {
                    quizManager.ClearQuiz();
                    appManager.DisplayRecentProjectList(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
                    });
                    ToggleMenu(false);
                }
            });
        }
    }
}