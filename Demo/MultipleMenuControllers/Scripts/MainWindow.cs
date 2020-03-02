using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeersProductions
{
	public class MainWindow : MCMenu {
		/// <summary>
		/// Called when clicking on the first button.
		/// </summary>
		public void ShowMenu1()
		{
			// Create our data-class.
			GeneralWindowData data = new GeneralWindowData("This is menu 1", Color.gray, new string[]{"button 1", "button 2", "button3"}, new string[]{"Pressed 1", "Pressed 2", "Pressed 3"});
			// Pass it to the specific menu.
			this.MenuController.ShowMenu("GENERALWINDOW", data);
		}

		/// <summary>
		/// Called when clicking on the second button.
		/// </summary>
		public void ShowMenu2()
		{
			GeneralWindowData data = new GeneralWindowData("This is menu 2", Color.blue, new string[]{"1", "2", "3"}, new string[]{"1", "2", "3"});
			this.MenuController.ShowMenu("GENERALWINDOW", data);
		}

		/// <summary>
		/// Called when clicking on the third button.
		/// </summary>
		public void ShowMenu3()
		{
			GeneralWindowData data = new GeneralWindowData("This is menu 3", Color.cyan, new string[]{"Yes", "No", "Maybe"}, new string[]{"Yes", "No", "Maybe"});
			this.MenuController.ShowMenu("GENERALWINDOW", data);
		}
	}
}
