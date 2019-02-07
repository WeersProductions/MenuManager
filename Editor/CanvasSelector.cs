using UnityEngine;
using UnityEditor;

namespace WeersProductions
{
	public class CanvasSelector : PopupWindowContent
	{
		private Canvas[] _canvasses;

		public delegate void SelectEvent(Canvas canvas);

		private SelectEvent _onSelected;

		private Vector2 _scrollviewPosition;

		public CanvasSelector(Canvas[] canvasses, SelectEvent onSelected) {
			_canvasses = canvasses;
			_onSelected = onSelected;
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(200, 150);
		}

		public override void OnGUI(Rect rect)
		{
			GUILayout.Label("Select a Canvas", EditorStyles.boldLabel);
			_scrollviewPosition = GUILayout.BeginScrollView(_scrollviewPosition);
			
			for(int i = 0; i < _canvasses.Length; i++) {
				int index = i;
				string buttonName = _canvasses[i].name;

				MenuController _menuController = _canvasses[i].GetComponent<MenuController>();
				if(_menuController) {
					// This Canvas already contains a MenuController.
					buttonName += " (already has a MenuController)";
				}
				if(GUILayout.Button(buttonName)) {
					editorWindow.Close();
					_onSelected(_canvasses[index]);
				}
			}
			
			GUILayout.EndScrollView();
			EditorGUILayout.Separator();
			if(GUILayout.Button("Create new")) {
				editorWindow.Close();
				_onSelected(null);
			}
		}
	}
}
