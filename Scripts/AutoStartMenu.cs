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
        private string _startMenu;

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

#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(_startMenu))
            {
                Debug.LogError("You are trying to use AutoStartMenu, but without any menu. Please set the menu that should be started.");
                return;
            }
#endif
           
            if (_menuController) {
                _menuController.ShowMenu(_startMenu);
            } else {
                MenuController.ShowMenuGlobal(_startMenu);
            }
        }
    }

}
