
using UnityEditor;
using UnityEngine;

namespace WeersProductions
{
    public static class EditorUtils
    {
        public static void Collapse(GameObject go, bool collapse)
        {
            // bail out immediately if the go doesn't have children
            if (go.transform.childCount == 0) return;
            // get a reference to the hierarchy window
            var hierarchy = GetFocusedWindow("Hierarchy");
            // select our go
            SelectObject(go);
            // create a new key event (RightArrow for collapsing, LeftArrow for folding)
            var key = new Event { keyCode = collapse ? KeyCode.RightArrow : KeyCode.LeftArrow, type = EventType.KeyDown };
            // finally, send the window the event
            hierarchy.SendEvent(key);
        }

        private static void SelectObject(Object obj)
        {
            Selection.activeObject = obj;
        }

        private static EditorWindow GetFocusedWindow(string window)
        {
            FocusOnWindow(window);
            return EditorWindow.focusedWindow;
        }

        private static void FocusOnWindow(string window)
        {
            EditorApplication.ExecuteMenuItem("Window/" + window);
        }
    }
}
