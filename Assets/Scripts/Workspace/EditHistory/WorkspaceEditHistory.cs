using System;
using System.Collections.Generic;
using Data.Dialogs;
using Newtonsoft.Json;
using UnityEngine;

namespace Workspace.EditHistory
{
    public class WorkspaceEditHistory: IWorkspaceEditHistory
    {
        private Stack<string> Head { get; set; }
        private string Current { get; set; }
        private Stack<string> Tail { get; set; }
        
        public WorkspaceEditHistory()
        {
            Head = new Stack<string>();
            Tail = new Stack<string>();
        }

        public bool CanUndo() => Head.Count > 0;
        public bool CanRedo() => Tail.Count > 0;

        public void SaveSnapshot(IWorkspace workspace)
        {
            string encoding = EncodeScene(workspace);
            if (encoding != Current)
            {
                if (!string.IsNullOrEmpty(Current))
                {
                    Head.Push(Current);
                }

                Current = encoding;
                Tail = new Stack<string>();
            }
        }

        public void Undo(Action<DialogScene> restore)
        {
            if (CanUndo())
            {
                Slide(Head, Tail, restore);
            }
        }

        public void Redo(Action<DialogScene> restore)
        {
            if (CanRedo())
            {
                Slide(Tail, Head, restore);
            }
        }

        private void Slide(Stack<string> prev, Stack<string> next, Action<DialogScene> restore)
        {
            if (!String.IsNullOrEmpty(Current))
            {
                next.Push(Current);
            }

            Current = prev.Pop();
            restore(JsonConvert.DeserializeObject<DialogScene>(Current));
        }
        
        private string EncodeScene(IWorkspace workspace)
        {
            return JsonConvert.SerializeObject(workspace.GetSceneDescription());
        }
    }
}