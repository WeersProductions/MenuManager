using UnityEngine;
using UnityEditor;

namespace WeersProductions
{
	public class MenuControllerSelector : PopupWindowContent
	{
		bool toggle1 = true;
		bool toggle2 = true;
		bool toggle3 = true;

		private MenuController[] _menuControllers;

		public delegate void SelectEvent(MenuController menuController);

		private SelectEvent _onSelected;

		private Vector2 _scrollviewPosition;

		public MenuControllerSelector(MenuController[] menuControllers, SelectEvent onSelected) {
			_menuControllers = menuControllers;
			_onSelected = onSelected;
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(200, 150);
		}

		public override void OnGUI(Rect rect)
		{
			GUILayout.Label("Select a MenuController", EditorStyles.boldLabel);
			_scrollviewPosition = GUILayout.BeginScrollView(_scrollviewPosition);
			
			for(int i = 0; i < _menuControllers.Length; i++) {
				int index = i;
				if(GUILayout.Button(_menuControllers[i].name)) {
					editorWindow.Close();
					_onSelected(_menuControllers[index]);
				}
			}
			
			GUILayout.EndScrollView();
		}

		public override void OnOpen()
		{
			Debug.Log("Popup opened: " + this);
		}

		public override void OnClose()
		{
			Debug.Log("Popup closed: " + this);
		}
	}
}
