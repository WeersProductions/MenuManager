using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeersProductions
{
	public class MainWindow : MCMenu {
		public void ShowMenu1()
		{
			this.MenuController.ShowMenu(MenuController.Menus.GAMEWINDOW, "This is menu 1");
		}

		public void ShowMenu2()
		{
			this.MenuController.ShowMenu(MenuController.Menus.GAMEWINDOW, "This is menu 2");
		}

		public void ShowMenu3()
		{
			this.MenuController.ShowMenu(MenuController.Menus.GAMEWINDOW, "This is menu 3");
		}
	}
}
