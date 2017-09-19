using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    /// <summary>
    /// Static references to the ids of all the menus and popups. Makes it easy to change ids.
    /// </summary>
    public enum Menus
    {
        NONE = -1
    }

    private static MenuController _instance;

    /// <summary>
    /// Used for editor only 
    /// </summary>
    [SerializeField]
    private MCMenu[] _mcMenus;

    /// <summary>
    /// Used for faster lookup during runtime of menus with specified ids.
    /// </summary>
    private readonly Dictionary<int, MCMenu> _menus = new Dictionary<int, MCMenu>();

    /// <summary>
    /// A list of the active menus on screen.
    /// </summary>
    private readonly List<MCMenu> _activeMenus = new List<MCMenu>();

    /// <summary>
    /// A queue of menus that want to be popupped up when another menu is closed.
    /// </summary>
    private readonly Queue<MCMenu> _menuQueue = new Queue<MCMenu>();

    private void Awake()
    {
        _instance = this;

        for (int i = 0; i < _mcMenus.Length; i++)
        {
            _menus.Add(_mcMenus[i].Id, _mcMenus[i]);
        }
    }

    /// <summary>
    /// Show a menu with a specific id.
    /// </summary>
    /// <param name="id">The unique id of a menu.</param>
    public static void ShowMenu(int id)
    {
        if (id == (int)Menus.NONE)
        {
            HideAllMenus();
        }
        else
        {
            ShowMenu(_instance._menus[id]);
        }
    }

    /// <summary>
    /// Show a specific menu.
    /// If this menu needs fullscreen and the current active menus allow it to override them, it will close the current menus.
    /// </summary>
    /// <param name="mcMenu">The new menu.</param>
    public static void ShowMenu(MCMenu mcMenu)
    {
        if (mcMenu.Fullscreen)
        {
            bool canShow = true;
            for (int i = 0; i < _instance._activeMenus.Count; i++)
            {
                if (!_instance._activeMenus[i].CanBeClosed)
                {
                    canShow = false;
                    break;
                }
            }
            if (canShow)
            {
                // Hide the current menu's and show this one.
                HideAllMenus();
            }
            else
            {
                if (mcMenu.ShouldBeQueued)
                {
                    _instance._menuQueue.Enqueue(mcMenu);
                }
                return;
            }
        }

        // We can show this one.
        mcMenu.Show();
        _instance._activeMenus.Add(mcMenu);
    }

    /// <summary>
    /// Hide all menus.
    /// </summary>
    public static void HideAllMenus()
    {
        for (int i = 0; i < _instance._activeMenus.Count; i++)
        {
            HideMenu(_instance._activeMenus[i]);
        }
    }

    /// <summary>
    /// Hide a specific menu.
    /// </summary>
    /// <param name="mcMenu"></param>
    public static void HideMenu(MCMenu mcMenu)
    {
        mcMenu.Hide(() => OnHideMenu(mcMenu));
    }

    /// <summary>
    /// Called when a menu is ready to be actually hidden internally (from the data structure).
    /// </summary>
    /// <param name="mcMenu"></param>
    private static void OnHideMenu(MCMenu mcMenu)
    {
        _instance._activeMenus.Remove(mcMenu);
        CheckMenuQueue();
    }

    /// <summary>
    /// Checks the queue of menus waiting to be activated and will activate them if possible.
    /// </summary>
    private static void CheckMenuQueue()
    {
        if (_instance._activeMenus.Count <= 0)
        {
            // There are no menus active, check the queue!
            if (_instance._menuQueue.Count > 0)
            {
                // There is a menu in the queue, let show it.
                ShowMenu(_instance._menuQueue.Dequeue());
            }
        }
    }

    /// <summary>
    /// Will add a menu popup for the current visible menu (first one that was visible).
    /// </summary>
    /// <param name="mcMenu"></param>
    /// <param name="createWhenNoMenu">If true this popup will be made an active window in the case of no active windows at the time of calling this.
    /// If false, no popup will be shown if there is no active window.</param>
    public static void AddPopup(MCMenu mcMenu, bool createWhenNoMenu)
    {
        if (_instance._activeMenus.Count > 0)
        {
            _instance._activeMenus[0].AddPopup(mcMenu);
        }
        else
        {
            if (createWhenNoMenu)
            {
                ShowMenu(mcMenu);
            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// A wrapper for <see cref="AddPopup(MCMenu,bool)"/>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="createWhenNoMenu"></param>
    public static void AddPopup(int id, bool createWhenNoMenu)
    {
        if (id < 0)
        {
            return;
        }
        AddPopup(_instance._menus[id], createWhenNoMenu);
    }
}
