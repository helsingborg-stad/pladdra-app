using System;
using System.Collections.Generic;
using System.Linq;
using Data.Dialogs;
using Newtonsoft.Json;
using UnityEngine;
using Utility;
using Pladdra.Data;

namespace Pladdra.Workspace.EditHistory
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

        public bool CanUndo()
        {
            return Head.Count > 0;
        }

        public bool CanRedo()
        {
            return Tail.Count > 0;
        }

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
                PladdraDebug.LogJson(new
                {
                    operation = "save",
                    head = Head.AsEnumerable(),
                    current = Current,
                    tail = Tail.AsEnumerable()
                });
            }
        }

        public void Undo(Action<UserProposal> restore)
        {
            if (CanUndo())
            {
                Slide(Head, Tail, restore);
            }
        }

        public void Redo(Action<UserProposal> restore)
        {
            if (CanRedo())
            {
                Slide(Tail, Head, restore);
            }
        }

        private void Slide(Stack<string> prev, Stack<string> next, Action<UserProposal> restore)
        {
            if (!String.IsNullOrEmpty(Current))
            {
                next.Push(Current);
            }

            Current = prev.Pop();

            PopWhileTopEquals(prev, Current);
            PopWhileTopEquals(next, Current);
            PladdraDebug.LogJson(new
            {
                operation = "restore",
                head = Head.AsEnumerable(),
                current = Current,
                tail = Tail.AsEnumerable()
            });
            restore(Decode(Current));
        }

        private void PopWhileTopEquals(Stack<string> stack, string current)
        {
            string top;
            while (stack.TryPeek(out top) && (top == current))
            {
                stack.Pop();
            }
        }

        private string EncodeScene(IWorkspace workspace)
        {
            return TryReuseString(
                JsonConvert.SerializeObject(workspace.GetSceneDescription()),
                Head.Concat(Tail).Concat(new[] { Current }));
        }

        private UserProposal Decode(string encoding)
        {
            return JsonConvert.DeserializeObject<UserProposal>(encoding);
        }

        private string TryReuseString(string value, IEnumerable<string> existing)
        {
            return existing.FirstOrDefault(s => s == value) ?? value;
        }
    }
}