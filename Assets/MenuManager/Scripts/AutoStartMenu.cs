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

        private void Start()
        {
            if (_disableOthers)
            {
                MenuController.HideAllMenus(true);
            }

            if (_startMenu != MenuController.Menus.NONE && _startMenu != MenuController.Menus.UNDEFINED)
            {
                MenuController.ShowMenu(_startMenu);
            }
        }
    }

}
