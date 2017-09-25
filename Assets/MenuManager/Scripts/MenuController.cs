using System.Collections.Generic;
using UnityEngine;

namespace WeersProductions
{
    public class MenuController : MonoBehaviour
    {
        /// <summary>
        /// Static references to the ids of all the menus and popups. Makes it easy to change ids.
        /// </summary>
        public enum Menus
        {
            UNDEFINED = -2,
            NONE = -1,
            SIMPLEPOPUP = 0,
            SIMPLETOOLTIP = 1
        }

        /// <summary>
        /// Singleton reference to the MenuController instance.
        /// </summary>
        private static MenuController _instance;

        /// <summary>
        /// Used for editor only 
        /// </summary>
        [SerializeField] private MCMenu[] _mcMenus;

        /// <summary>
        /// Used for faster lookup during runtime of menus with specified ids.
        /// </summary>
        private readonly Dictionary<Menus, MCMenu> _menus = new Dictionary<Menus, MCMenu>();

        /// <summary>
        /// A list of the active menus on screen.
        /// </summary>
        private readonly List<MCMenu> _activeMenus = new List<MCMenu>();

        /// <summary>
        /// A set of pooling objects from menus.
        /// </summary>
        private readonly Dictionary<Menus, Queue<MCMenu>> _menuPool = new Dictionary<Menus, Queue<MCMenu>>();

        /// <summary>
        /// A queue of menus that want to be popupped up when another menu is closed.
        /// This queue contains copy of the original menus, so you don't have to duplicate them anymore.
        /// </summary>
        private readonly Queue<MenuQueueItem> _menuQueue = new Queue<MenuQueueItem>();

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
        /// <param name="mcMenuData"></param>
        public static MCMenu ShowMenu(Menus id, object mcMenuData = null)
        {
             return ShowMenu(GetPoolObject(id), mcMenuData);
        }

        /// <summary>
        /// Will return a duplicate of the original menu so it can be used in the method <see cref="ShowMenu(WeersProductions.MCMenu,object)"/>
        /// </summary>
        /// <param name="id">The TYPE of the menu</param>
        /// <returns></returns>
        public static MCMenu GetMenu(Menus id)
        {
            return GetPoolObject(id);
        }

        /// <summary>
        /// Show a specific menu.
        /// If this menu needs fullscreen and the current active menus allow it to override them, it will close the current menus.
        /// </summary>
        /// <param name="mcMenu">The new menu. (this is a copy of the original)</param>
        /// <param name="mcMenuData"></param>
        public static MCMenu ShowMenu(MCMenu mcMenu, object mcMenuData = null)
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
                        _instance._menuQueue.Enqueue(new MenuQueueItem(mcMenu, mcMenuData));
                    }
                    return mcMenu;
                }
            }

            // We can show this one.
            mcMenu.Show(mcMenuData);
            _instance._activeMenus.Add(mcMenu);
            return mcMenu;
        }

        /// <summary>
        /// Hide all menus.
        /// </summary>
        public static void HideAllMenus()
        {
            List<MCMenu> copy = new List<MCMenu>(_instance._activeMenus);
            for (int i = 0; i < copy.Count; i++)
            {
                HideMenu(copy[i]);
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
            if (_instance._activeMenus.Contains(mcMenu))
            {
                _instance._activeMenus.Remove(mcMenu);
                // Only check the menu when we actually removed something.
                CheckMenuQueue();
            }

            if (mcMenu.ShouldBePooled)
            {
                // It should be pooled, add it.
                AddPoolObject(mcMenu);
            }
            else
            {
                // Since it should not be pooled, we destroy it.
                Destroy(mcMenu.gameObject);
            }
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
                    MenuQueueItem menuQueueItem = _instance._menuQueue.Dequeue();
                    ShowMenu(menuQueueItem.Menu, menuQueueItem.Data);
                }
            }
        }

        /// <summary>
        /// Will add a menu popup for the current visible menu (first one that was visible).
        /// </summary>
        /// <param name="mcMenu"></param>
        /// <param name="createWhenNoMenu">If true this popup will be made an active window in the case of no active windows at the time of calling this.
        /// If false, no popup will be shown if there is no active window.</param>
        /// <param name="data"></param>
        public static void AddPopup(MCMenu mcMenu, bool createWhenNoMenu, object data = null)
        {
            if (_instance._activeMenus.Count > 0)
            {
                _instance._activeMenus[0].AddPopup(mcMenu, data);
            }
            else
            {
                if (createWhenNoMenu)
                {
                    ShowMenu(mcMenu, data);
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
        /// <param name="data"></param>
        public static void AddPopup(Menus id, bool createWhenNoMenu, object data = null)
        {
            if (id == Menus.NONE)
            {
                return;
            }
            AddPopup(GetPoolObject(id), createWhenNoMenu, data);
        }

        /// <summary>
        /// Returns a poolObject if available, otherwise it will instantiate a new menu.
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        private static MCMenu GetPoolObject(Menus menu)
        {
            Queue<MCMenu> menus;
            if (_instance._menuPool.TryGetValue(menu, out menus))
            {
                if (menus.Count > 0)
                {
                    MCMenu dequeue = menus.Dequeue();
                    dequeue.PrepareForPool();
                    return dequeue;
                }
            }

            // We could not find a poolObject, instantiate a new object.
            return Instantiate(_instance._menus[menu], _instance.transform);
        }

        /// <summary>
        /// Add a poolObject to the pool for a certain menu.
        /// Only does data managing, the object should already be ready for pooling (E.G. inactive).
        /// </summary>
        /// <param name="menu"></param>
        private static void AddPoolObject(MCMenu menu)
        {
            Queue<MCMenu> menus;
            if (!_instance._menuPool.TryGetValue(menu.Id, out menus))
            {
                menus = new Queue<MCMenu>();
                _instance._menuPool.Add(menu.Id, menus);
            }

            menus.Enqueue(menu);
        }
    }

    struct MenuQueueItem
    {
        private readonly MCMenu _mcMenu;
        private readonly object _data;

        public MenuQueueItem(MCMenu mcMenu, object data)
        {
            _mcMenu = mcMenu;
            _data = data;
        }

        public MCMenu Menu
        {
            get { return _mcMenu; }
        }

        public object Data
        {
            get { return _data; }
        }
    }
}