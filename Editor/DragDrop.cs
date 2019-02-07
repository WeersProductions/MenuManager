using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DragDrop
{

    public delegate void OnDrop(Object[] dropObjects);

    public static void DrawDragDrop(string label, OnDrop onDrop, float height = 50.0f)
    {
        Event evt = Event.current;
        GUIStyle guiStyle = GUI.skin.GetStyle("Box");
        guiStyle.alignment = TextAnchor.MiddleCenter;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, label, guiStyle);

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
