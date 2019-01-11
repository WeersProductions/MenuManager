
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace WeersProductions
{
    public static class EditorUtils
    {
        public static void Collapse(GameObject go, bool collapse)
        {
            // bail out immediately if the go doesn't have children
            if (go.transform.childCount == 0) return;
            // get a reference to the hierarchy window
            var hierarchy = GetFocusedWindow("General/Hierarchy");
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The center of the screen if no mouseposition could be found.</returns>
        public static Vector2 GetMousePosition() 
        {
            if(Event.current != null) {
                return GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            }

            Vector2 coordinates = new Vector2(Screen.width / 2, Screen.height / 2);
            FieldInfo field = typeof ( Event ).GetField ( "s_Current", BindingFlags.Static | BindingFlags.NonPublic );
            if ( field != null )
            {
                Event current = field.GetValue ( null ) as Event;
                if ( current != null )
                {
                    coordinates = current.mousePosition;
                }
            }
            Debug.Log(coordinates);
            return GUIUtility.GUIToScreenPoint(coordinates);
        }
    }
}
