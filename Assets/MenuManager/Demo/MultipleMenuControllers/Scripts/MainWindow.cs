using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeersProductions
{
	public class MainWindow : MCMenu {
		public void ShowMenu1()
		{
			GeneralWindowData data = new GeneralWindowData("This is menu 1", Color.gray, new string[]{"button 1", "button 2", "button3"}, new string[]{"Pressed 1", "Pressed 2", "Pressed 3"});
			this.MenuController.ShowMenu(MenuController.Menus.GENERALWINDOW, data);
		}

		public void ShowMenu2()
		{
			GeneralWindowData data = new GeneralWindowData("This is menu 2", Color.blue, new string[]{"1", "2", "3"}, new string[]{"1", "2", "3"});
			this.MenuController.ShowMenu(MenuController.Menus.GENERALWINDOW, data);
		}

		public void ShowMenu3()
		{
			GeneralWindowData data = new GeneralWindowData("This is menu 3", Color.cyan, new string[]{"Yes", "No", "Maybe"}, new string[]{"Yes", "No", "Maybe"});
			this.MenuController.ShowMenu(MenuController.Menus.GENERALWINDOW, data);
		}
	}
}
