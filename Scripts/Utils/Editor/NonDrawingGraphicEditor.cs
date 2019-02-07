using UnityEditor.UI;
using UnityEditor;
using UnityEngine;

namespace WeersProductions
{
    /// <inheritdoc />
    /// <remarks>http://answers.unity3d.com/answers/1157876/view.html</remarks>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NonDrawingGraphic), false)]
    public class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Script);
            // skipping AppearanceControlsGUI
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
