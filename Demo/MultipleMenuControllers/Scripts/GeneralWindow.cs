using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WeersProductions
{
	/// <summary>
	/// A simple window with x buttons and a text in the center.
	/// Background color can be changed using properties in the constructor.
	/// </summary>
	public class GeneralWindow : MCMenu {

		/// <summary>
		/// Main text in the center of the window.
		/// </summary>
		[SerializeField]
		private Text _text;
		/// <summary>
		/// The background image which color is changed using properties.
		/// </summary>
		[SerializeField]
		private Image _background;

		[SerializeField]
		private Button[] _buttons;
		public override void Show(object data) 
		{
			GeneralWindowData windowData = data as GeneralWindowData;
			
			if (windowData == null) {
				Debug.LogError("Trying to show a GeneralWindow without the apprioate data!");
				return;
			} 

			_text.text = windowData.text;
			_background.color = windowData.BackgroundColor;
			for(int i = 0; i < _buttons.Length; i++) {
				// Store, since we use it in a lambda expression
				int index = i;
				_buttons[i].GetComponentInChildren<Text>().text = windowData.ButtonsTexts[index];
				_buttons[i].onClick.RemoveAllListeners();
				_buttons[i].onClick.AddListener(() => {
					SetText(windowData.ButtonFunctionTexts[index]);
				});
			}
			base.Show(data);
		}

		private void SetText(string text) 
		{
			this._text.text = text;
		}
	}
}
