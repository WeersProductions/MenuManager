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
        [SerializeField]
        private MCMenu[] _mcMenus;

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
        /// This queue contains copy of the original menus, so you don't have to duplicate (instantiate) them anymore.
        /// </summary>
        private readonly Queue<MenuQueueItem> _menuQueue = new Queue<MenuQueueItem>();

        /// <summary>
        /// A reference to the objec that checks clicking outside the menu.
        /// </summary>
        private NonDrawingGraphic _clickOutsideMenu;

        private void Awake()
        {
            // Set the singleton reference.
            _instance = this;

            // For faster referencing, convert the array to a dictionary for run-time use.
            for (int i = 0; i < _mcMenus.Length; i++)
            {
                _menus.Add(_mcMenus[i].Id, _mcMenus[i]);
            }

            // Create the object that checks clicking outside of the current menu.
            _clickOutsideMenu = CreateOutsideMenuObject();
        }

        /// <summary>
        /// Create the object that checks clicking outside of the current menu and disable it.
        /// </summary>
        /// <returns>The new component that is added to control it.</returns>
        private NonDrawingGraphic CreateOutsideMenuObject()
        {
            GameObject outsideMenuGameObject = new GameObject("OutsideMenuClick", typeof(RectTransform));
            RectTransform outsideMenuRect = outsideMenuGameObject.GetComponent<RectTransform>();
            outsideMenuRect.SetParent(transform);
            outsideMenuRect.anchoredPosition = Vector2.zero;
            outsideMenuRect.sizeDelta = Vector2.zero;
            outsideMenuRect.anchorMin = Vector2.zero;
            outsideMenuRect.anchorMax = Vector2.one;
            outsideMenuGameObject.SetActive(false);
            return outsideMenuGameObject.AddComponent<NonDrawingGraphic>();
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

            _instance.InitializeOutsideClick(mcMenu);
            // We can show this one.
            mcMenu.Show(mcMenuData);
            _instance._activeMenus.Add(mcMenu);
            return mcMenu;
        }

        /// <summary>
        /// Hide all menus.
        /// </summary>
        /// <param name="hideAll">If true it will set all children as inactive (it will not <see cref="HideMenu"/> them!) of the MenuController, even those that are not marked as active.</param>
        public static void HideAllMenus(bool hideAll = false)
        {
            List<MCMenu> copy = new List<MCMenu>(_instance._activeMenus);
            for (int i = 0; i < copy.Count; i++)
            {
                HideMenu(copy[i]);
            }

            if (hideAll)
            {
                MCMenu[] allMenus = _instance.GetComponentsInChildren<MCMenu>();
                for (int i = 0; i < allMenus.Length; i++)
                {
                    allMenus[i].gameObject.SetActive(false);
                }
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
                _instance._clickOutsideMenu.gameObject.SetActive(false);

                _instance._activeMenus.Remove(mcMenu);
                // Only check the menu when we actually removed something.
                if (!CheckMenuQueue())
                {
                    // We did not activate a menu in the queue.
                    _instance.InitializeOutsideClick();
                }
            }
            else
            {
                if (mcMenu.Parent)
                {
                    _instance.InitializeOutsideClick(mcMenu.Parent);
                }
                else
                {
                    _instance.InitializeOutsideClick();
                }
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
        /// <returns>True if a new menu is activated that was in the queue.</returns>
        private static bool CheckMenuQueue()
        {
            if (_instance._activeMenus.Count <= 0)
            {
                // There are no menus active, check the queue!
                if (_instance._menuQueue.Count > 0)
                {
                    // There is a menu in the queue, let show it.
                    MenuQueueItem menuQueueItem = _instance._menuQueue.Dequeue();
                    ShowMenu(menuQueueItem.Menu, menuQueueItem.Data);
                    return true;
                }
            }
            return false;
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
                _instance.InitializeOutsideClick(mcMenu);
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
        /// A wrapper for <see cref="AddPopup(MCMenu,bool, object)"/>
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
        /// Called when a popup is added directly to the McMenu and will make sure the OutsideClick is correctly configured.
        /// </summary>
        /// <param name="mcMenu"></param>
        public static void OnMenuAdded(MCMenu mcMenu)
        {
            _instance.InitializeOutsideClick(mcMenu);
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

        /// <summary>
        /// Prepare the OutsideClick object and put it in the right position in the heiarchy.
        /// </summary>
        /// <param name="mcMenu"></param>
        private void InitializeOutsideClick(MCMenu mcMenu)
        {
            int siblingIndex = mcMenu.transform.GetSiblingIndex();
            if (_clickOutsideMenu.transform.GetSiblingIndex() < siblingIndex)
            {
                siblingIndex--;
            }
            _clickOutsideMenu.transform.SetSiblingIndex(siblingIndex);
            _clickOutsideMenu.OnClick = mcMenu.OnClickOutside;

            _clickOutsideMenu.raycastTarget = mcMenu.BlockOutsideRaycasting;

            _clickOutsideMenu.gameObject.SetActive(true);
        }

        /// <summary>
        /// Will Initalize the outside click for the last activated menu that is still active.
        /// </summary>
        private void InitializeOutsideClick()
        {
            MCMenu lastActive = GetLastActive();
            if (lastActive)
            {
                InitializeOutsideClick(lastActive);
            }
        }

        /// <summary>
        /// Returns the menu that was activated last and is still active now.
        /// </summary>
        private MCMenu GetLastActive()
        {
            if (_activeMenus.Count > 0)
            {
                return _activeMenus[_activeMenus.Count - 1];
            }
            return null;
        }

        /// <summary>
        /// Checks if any menus are currently active.
        /// </summary>
        /// <returns>True if any menu is active, false if no menu is active.</returns>
        public static bool AnyMenuActive()
        {
            return _instance._activeMenus.Count > 0;
        }
    }

    /// <summary>
    /// Used when a menu needs to be put in queue, because it cannot be shown at the moment.
    /// </summary>
    struct MenuQueueItem
    {
        /// <summary>
        /// The menu that is going to be shown.
        /// </summary>
        private readonly MCMenu _mcMenu;
        /// <summary>
        /// Extra data that is passed to the Menu when it is activated.
        /// </summary>
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