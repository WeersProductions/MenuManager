using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WeersProductions;

namespace WeersProductions
{
    [CustomEditor(typeof(MCMenu), true)]
    public class MCMenuInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (target is IDraggableMenu draggableMenu)
            {
                EditorGUILayout.HelpBox("Will set the owner variable of all Draggable child components to this menu.", MessageType.Info);
                if (GUILayout.Button("Initialize draggables"))
                {
                    InitializeDraggables(draggableMenu);
                }
            }
        }

        private void InitializeDraggables(IDraggableMenu draggableMenu)
        {
            MCMenu mcMenu = (MCMenu) target;
            Draggable[] draggables = mcMenu.GetComponentsInChildren<Draggable>();
            
            for (int i = 0; i < draggables.Length; i++)
            {
                SerializedObject so = new SerializedObject(draggables[i]);
                so.Update();
                so.FindProperty("Owner").objectReferenceValue = mcMenu;
                so.ApplyModifiedProperties();
            }
        }
    }
}
