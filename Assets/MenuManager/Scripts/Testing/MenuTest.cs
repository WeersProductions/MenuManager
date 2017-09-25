using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WeersProductions;

namespace WeersProductions
{
    public class MenuTest : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            CreatePopup(0);
        }

        void CreatePopup(int count)
        {
            MCMenu popupMenu = MenuController.GetMenu(MenuController.Menus.SIMPLEPOPUP);

            MCSimplePopupData simplePopupData = new MCSimplePopupData("title " + count, "This is another popup.", new UnityAction[]
            {
                () =>
                {
                    CreatePopup(count + 1);
                },
                popupMenu.Hide,
                () =>
                {
                    MenuController.HideMenu(popupMenu.Parent);
                }
            }, new[]
            {
                "New popup",
                "Close this",
                "Close parent"
            });

            MenuController.AddPopup(popupMenu, true, simplePopupData);
        }
    }
}
