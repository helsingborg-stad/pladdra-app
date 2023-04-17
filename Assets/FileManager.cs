using System.Collections;
using System.Collections.Generic;
using Pladdra.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pladdra.ARSandbox
{
    public class FileManager : MonoBehaviour
    {
        //TODO: This class is a stub. Model loading from Project.cs should be moved here.
        protected MenuManager menuManager { get { return transform.parent.gameObject.GetComponentInChildren<MenuManager>(); } }
        protected UIManager uiManager { get { return transform.parent.gameObject.GetComponentInChildren<UIManager>(); } }
        string fileFolder = "Files";
        void Start()
        {

            StartCoroutine(AddMenuItems());
        }

        // Workaround to place the menu item at the bottom of the menu
        IEnumerator AddMenuItems()
        {
            yield return new WaitForSeconds(1);
            menuManager.AddMenuItem(new MenuItem()
            {
                id = "delete-all-files",
                name = "Radera modeller",
                action = () =>
                {
                    uiManager.DisplayUI("warning-with-text-and-options", root =>
                    {
                        root.Q<Label>("warning").text = "Radera alla modeller?";
                        root.Q<Label>("text").text = "Vill du radera alla nedladdade modeller? Appen behöver startas om för att genomföra raderingen.";
                        root.Q<Button>("option-one").clicked += () =>
                        {
                            uiManager.DisplayPreviousUI();
                        };
                        root.Q<Button>("option-one").text = "Avbryt";
                        root.Q<Button>("option-two").clicked += () =>
                        {
                            DeleteAllLocalFilesAndQuit();
                        };
                        root.Q<Button>("option-two").text = "Radera";
                    });
                }
            });
        }

        void DeleteAllLocalFilesAndQuit()
        {
            string[] files = System.IO.Directory.GetFiles(FilePath());
            foreach (string file in files)
            {
                System.IO.File.Delete(file);
            }
            Application.Quit();
        }

        public string FilePath()
        {
            return Application.persistentDataPath + "/" + fileFolder + "/";
        }
    }
}