using UnityEngine;

namespace WeersProductions
{
    /// <inheritdoc />
    /// <summary>
    /// Simple class used for prototyping.
    /// </summary>
    public class AutoStartMenu : MonoBehaviour
    {
        /// <summary>
        /// If true it will disable all other menus that are still active when starting the scene.
        /// </summary>
        [SerializeField]
        private bool _disableOthers;

        /// <summary>
        /// This menu will be shown at the start of the scene.
        /// </summary>
        [SerializeField]
        private MenuController.Menus _startMenu;

        /// <summary>
        /// If set, this MenuController is used to show the menu. Otherwise, Global is used.
        /// </summary>
        [SerializeField]
        private MenuController _menuController;

        private void Start()
        {
            if (_disableOthers)
            {
                if (_menuController) {
                    _menuController.HideAllMenus(true);
                } else {
                    MenuController.HideAllMenusGlobal(true);
                }
            }

            if (_startMenu != MenuController.Menus.NONE && _startMenu != MenuController.Menus.UNDEFINED)
            {
                if (_menuController) {
                    _menuController.ShowMenu(_startMenu);
                } else {
                    MenuController.ShowMenuGlobal(_startMenu);
                }
            }
        }
    }

}
