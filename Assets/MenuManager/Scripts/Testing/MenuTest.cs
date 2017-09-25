using UnityEngine;

namespace WeersProductions
{
    public class MenuTest : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            CreatePopup(0);
        }

        private void CreatePopup(int count)
        {
            var popupMenu = MenuController.GetMenu(MenuController.Menus.SIMPLEPOPUP);

            var simplePopupData = new MCSimplePopupData("title " + count, "This is another popup.",
                new MCSimplePopupData.ButtonClick[]
                {
                    button => { CreatePopup(count + 1); },
                    button => popupMenu.Hide(),
                    button => { MenuController.HideMenu(popupMenu.Parent); },
                    button =>
                    {
                        var simpleTooltipData = new MCSimpleTooltipData("Tooltip", "More text",
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

            MenuController.AddPopup(popupMenu, true, simplePopupData);
        }
    }
}