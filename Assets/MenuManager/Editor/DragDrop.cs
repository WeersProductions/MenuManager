using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DragDrop
{

    public delegate void OnDrop(Object[] dropObjects);

    public static void DrawDragDrop(string label, OnDrop onDrop)
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, label);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    onDrop(DragAndDrop.objectReferences);
                }
                break;
        }
    }
}
