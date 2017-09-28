using UnityEngine;

namespace WeersProductions
{
    /// <summary>
    /// A simple script that can be attached to any gameobject to create a popup with some buttons.
    /// </summary>
    public class MenuTest : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            CreatePopup(0);
        }

        private void CreatePopup(int count)
        {
            // Get the Menu object, we need this because we have a button that closes the menu.
            MCMenu popupMenu = MenuController.GetMenu(MenuController.Menus.SIMPLEPOPUP);

            // Create the data for the menu.
            MCSimplePopupData simplePopupData = new MCSimplePopupData("title " + count, "This is another popup.",
                new MCSimplePopupData.ButtonClick[]
                {
                    button => { CreatePopup(count + 1); },
                    button => popupMenu.Hide(),
                    button => { MenuController.HideMenu(popupMenu.Parent); },
                    button =>
                    {
                        MCSimpleTooltipData simpleTooltipData = new MCSimpleTooltipData("Tooltip", "More text",
                            button.GetComponent<RectTransform>()) {AutoRemove = true};
                        MenuController.AddPopup(MenuController.Menus.SIMPLETOOLTIP, false, simpleTooltipData);
                    }
                }, new[]
                {
                    "New popup",
                    "Close this",
                    "Close parent",
                    "Tooltip"
                });

            // Add the popup to the screen, when there is nothing on the screen it will be added as a menu instead of a popup. 
            MenuController.AddPopup(popupMenu, true, simplePopupData);
        }
    }
}