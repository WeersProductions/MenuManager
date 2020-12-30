using System.Collections.Generic;
using UnityEngine;

namespace WeersProductions
{
    /// <summary>
    /// TODO: Allow names for MenuControllers with string look-up
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        /// <summary>
        /// Singleton reference to the global MenuController instance.
        /// </summary>
        private static MenuController _instance;

        /// <summary>
        /// If true, this will overwrite the static instance variable.
        /// </summary>
        [SerializeField]
        private bool _global;

        /// <summary>
        /// Used for editor only 
        /// </summary>
        [SerializeField]
        private MCMenu[] _mcMenus;

        /// <summary>
        /// Used for faster lookup during runtime of menus with specified ids.
        /// </summary>
        private readonly Dictionary<string, MCMenu> _menus = new Dictionary<string, MCMenu>();

        /// <summary>
        /// A list of the active menus on screen.
        /// </summary>
        private readonly List<MCMenu> _activeMenus = new List<MCMenu>();

        /// <summary>
        /// A set of pooling objects from menus.
        /// </summary>
        private readonly Dictionary<string, Queue<MCMenu>> _menuPool = new Dictionary<string, Queue<MCMenu>>();

        /// <summary>
        /// A queue of menus that want to be popupped up when another menu is closed.
        /// This queue contains copy of the original menus, so you don't have to duplicate (instantiate) them anymore.
        /// </summary>
        private readonly Queue<MenuQueueItem> _menuQueue = new Queue<MenuQueueItem>();

        /// <summary>
        /// A reference to the objec that checks clicking outside the menu.
        /// </summary>
        private NonDrawingGraphic _clickOutsideMenu;

#region Unity methods
        private void Awake()
        {
            // Set the singleton reference.
            if (_instance && _global) {
                if (_instance._global) {
                    Debug.Log("Some MenuManager is defined as global and is currently set as the instance reference. Are multiple static MenuManager in the scene?");
                } else {
                    // The current instance is not defined as global and can thus be overwritten.
                    _instance = this;
                }
            } else {
                _instance = this;
            }

            // For faster referencing, convert the array to a dictionary for run-time use.
            for (int i = 0; i < _mcMenus.Length; i++)
            {
                if(_menus.ContainsKey(_mcMenus[i].Id)) {
                    Debug.LogError(string.Format("This menu could not be added, since this canvas ({0}) already contains a key for {1}", this.name, _mcMenus[i].Id));
                    continue;
                }
                _menus.Add(_mcMenus[i].Id, _mcMenus[i]);
            }

            // Add the shared menus.
            MCMenu[] sharedMenus = MenuControllerSharedProps.GetSharedMenus();
            for(int i = 0; i < sharedMenus.Length; i++) {
                if(_menus.ContainsKey(sharedMenus[i].Id)) {
                    Debug.LogError(string.Format("This shared menu could not be added, since this canvas ({0}) already contains a key for {1}", this.name, sharedMenus[i].Id));
                    continue;
                }
                _menus.Add(sharedMenus[i].Id, sharedMenus[i]);
            }

            // Create the object that checks clicking outside of the current menu.
            _clickOutsideMenu = CreateOutsideMenuObject();
        }
#endregion

#region Instance methods
        /// <summary>
        /// Create the object that checks clicking outside of the current menu and disable it.
        /// </summary>
        /// <returns>The new component that is added to control it.</returns>
        private NonDrawingGraphic CreateOutsideMenuObject()
        {
            GameObject outsideMenuGameObject = new GameObject("OutsideMenuClick", typeof(RectTransform));
            RectTransform outsideMenuRect = outsideMenuGameObject.GetComponent<RectTransform>();
            outsideMenuRect.SetParent(transform, false);
            outsideMenuRect.sizeDelta = Vector2.zero;
            outsideMenuRect.anchorMin = Vector2.zero;
            outsideMenuRect.anchorMax = Vector2.one;
            outsideMenuGameObject.SetActive(false);
            outsideMenuGameObject.AddComponent<CanvasRenderer>();
            return outsideMenuGameObject.AddComponent<NonDrawingGraphic>();
        }

        /// <summary>
        /// Show a menu with a specific id.
        /// </summary>
        /// <param name="id">The unique id of a menu.</param>
        /// <param name="mcMenuData"></param>
        public MCMenu ShowMenu(string id, object mcMenuData = null) 
        {
            return ShowMenu(GetPoolObject(id), mcMenuData);
        }

        /// <summary>
        /// Show a specific menu.
        /// If this menu needs fullscreen and the current active menus allow it to override them, it will close the current menus.
        /// </summary>
        /// <param name="mcMenu">The new menu. (this is a copy of the original)</param>
        /// <param name="mcMenuData"></param>
        public MCMenu ShowMenu(MCMenu mcMenu, object mcMenuData = null)
        {
            if (mcMenu.Fullscreen)
            {
                bool canShow = true;
                for (int i = 0; i < _activeMenus.Count; i++)
                {
                    if (_activeMenus[i].ShouldBlockNewMenu)
                    {
                        canShow = false;
                        break;
                    }
                }
                if (canShow)
                {
                    // Hide the current menu's and show this one.
                    HideAllBlockingMenus();
                }
                else
                {
                    if (mcMenu.ShouldBeQueued)
                    {
                        _menuQueue.Enqueue(new MenuQueueItem(mcMenu, mcMenuData));
                    }
                    return mcMenu;
                }
            }

            InitializeOutsideClick(mcMenu);
            // Add ourselves as the owner of the menu.
            mcMenu.SetMenuController(this);
            // We can show this one.
            mcMenu.Show(mcMenuData);
            _activeMenus.Add(mcMenu);
            return mcMenu;
        }

        /// <summary>
        /// Get a menu from the active menus.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>Uses linear search.</remarks>
        /// <returns>Null if no active menu of this type exists.</returns>
        public MCMenu GetMenuActive(string id)
        {
            for (int i = 0; i < _activeMenus.Count; i++)
            {
                if (_activeMenus[i].Id == id)
                {
                    return _activeMenus[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Try to get a menu of type from the active menus.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mcMenu">Set to null if no menu of this type is found. Otherwise is the menu that that is active with type 'id'.</param>
        /// <remarks>Uses linear search.</remarks>
        /// <returns>False if no menu of this type is found. </returns>
        public bool TryGetMenuActive(string id, out MCMenu mcMenu)
        {
            for (int i = 0; i < _activeMenus.Count; i++)
            {
                if (_activeMenus[i].Id == id)
                {
                    mcMenu = _activeMenus[i];
                    return true;
                }
            }

            mcMenu = null;
            return false;
        }

        /// <summary>
        /// Gets all active menus.
        /// </summary>
        /// <remarks>Converts internal list to array, as the internal list should not be modified.</remarks>
        /// <returns>Array of active MCMenus objects.</returns>
        public MCMenu[] GetActiveMenus()
        {
            return _activeMenus.ToArray();
        }
        
        /// <summary>
        /// Will return a duplicate of the original menu in the global MenuController so it can be used in the method <see cref="ShowMenu(WeersProductions.MCMenu,object)"/>
        /// </summary>
        /// <param name="id">The TYPE of the menu</param>
        /// <returns></returns>
        public MCMenu GetMenu(string id)
        {
            return GetPoolObject(id);
        }

        /// <summary>
        /// Toggles the visibility of a menu. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="menuData">Any data that needs to be passed to the menu if it gets shown.</param>
        /// <returns></returns>
        public MCMenu ToggleMenu(string id, object menuData = null)
        {
            if (TryGetMenuActive(id, out MCMenu menu))
            {
                HideMenu(menu);
            }
            else
            {
                menu = GetMenu(id);
                ShowMenu(menu, menuData);
            }

            return menu;
        }

        /// <summary>
        /// Hide all menus.
        /// </summary>
        /// <param name="hideAll">If true it will set all children as inactive (it will not <see cref="HideMenu"/> them!) of the MenuController, even those that are not marked as active.</param>
        public void HideAllMenus(bool hideAll = false)
        {
            List<MCMenu> copy = new List<MCMenu>(_activeMenus);
            for (int i = 0; i < copy.Count; i++)
            {
                HideMenu(copy[i]);
            }

            if (hideAll)
            {
                MCMenu[] allMenus = GetComponentsInChildren<MCMenu>();
                for (int i = 0; i < allMenus.Length; i++)
                {
                    allMenus[i].SetState(false);
                }
            }
        }

        /// <summary>
        /// Will hide all menus that are blocking other menus from showing up.
        /// </summary>
        public void HideAllBlockingMenus()
        {
            List<MCMenu> copy = new List<MCMenu>(_activeMenus);
            for (int i = 0; i < copy.Count; i++)
            {
                if (copy[i].ShouldBlockNewMenu)
                {
                    HideMenu(copy[i]);
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
        public MCMenu AddPopup(MCMenu mcMenu, bool createWhenNoMenu, object data = null)
        {
            if (_activeMenus.Count > 0)
            {
                InitializeOutsideClick(mcMenu);
                _activeMenus[0].AddPopup(mcMenu, data);
            }
            else
            {
                if (createWhenNoMenu)
                {
                    ShowMenu(mcMenu, data);
                }
            }
            return mcMenu;
        }       

        /// <summary>
        /// A wrapper for <see cref="AddPopup(MCMenu,bool, object)"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createWhenNoMenu"></param>
        /// <param name="data"></param>
        public MCMenu AddPopup(string id, bool createWhenNoMenu, object data = null)
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(id))
            {
                Debug.LogError("You are trying to add a popup, but the id is empty. This is probably a mistake.");
            }
#endif
            return AddPopup(GetPoolObject(id), createWhenNoMenu, data);
        }

        /// <summary>
        /// Add a popup with an id to an existing menu.
        /// </summary>
        /// <param name="id">The id of the new popup.</param>
        /// <param name="parent">The parent object that already exists.</param>
        /// <param name="data">Data that should be passed on to the popup that is created.</param>
        /// <returns></returns>
        public MCMenu AddPopup(string id, MCMenu parent, object data = null)
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(id))
            {
                Debug.LogError("You are trying to add a popup, but the id is empty. This is probably a mistake.");
            }
#endif
            return parent.AddPopup(GetPoolObject(id), data);
        } 

        /// <summary>
        /// Returns a poolObject from the global MenuManager if available, otherwise it will instantiate a new menu.
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        private MCMenu GetPoolObject(string menu)
        {
            Queue<MCMenu> menus;
            if (_menuPool.TryGetValue(menu, out menus))
            {
                if (menus.Count > 0)
                {
                    MCMenu dequeue = menus.Dequeue();
                    dequeue.PrepareForPool();
                    return dequeue;
                }
            }

            // We could not find a poolObject, instantiate a new object.
            MCMenu menuObject;
            if(!_menus.TryGetValue(menu, out menuObject))
            {
                Debug.LogError("Trying to show a menu that is not added to this MenuController: " + menu);
                return null;
            }
            MCMenu mcMenu = Instantiate(menuObject, transform);
            mcMenu.SetState(false);
            return mcMenu;
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
        /// Hide a specific menu.
        /// </summary>
        /// <param name="mcMenu"></param>
        public void HideMenu(MCMenu mcMenu)
        {
            mcMenu.Hide(() => OnHideMenu(mcMenu));
        }

        /// <summary>
        /// Checks the queue of menus waiting to be activated and will activate them if possible.
        /// </summary>
        /// <returns>True if a new menu is activated that was in the queue.</returns>
        private bool CheckMenuQueue()
        {
            if (_activeMenus.Count <= 0)
            {
                // There are no menus active, check the queue!
                if (_menuQueue.Count > 0)
                {
                    // There is a menu in the queue, let show it.
                    MenuQueueItem menuQueueItem = _menuQueue.Dequeue();
                    ShowMenu(menuQueueItem.Menu, menuQueueItem.Data);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Called when a menu is ready to be actually hidden internally (from the data structure).
        /// </summary>
        /// <param name="mcMenu"></param>
        private void OnHideMenu(MCMenu mcMenu)
        {
            if (_activeMenus.Contains(mcMenu))
            {
                _clickOutsideMenu.gameObject.SetActive(false);

                _activeMenus.Remove(mcMenu);
                // Only check the menu when we actually removed something.
                if (!CheckMenuQueue())
                {
                    // We did not activate a menu in the queue.
                    InitializeOutsideClick();
                }
            }
            else
            {
                if (mcMenu.Parent)
                {
                    InitializeOutsideClick(mcMenu.Parent);
                }
                else
                {
                    InitializeOutsideClick();
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
        /// Add a poolObject to the pool for a certain menu.
        /// Only does data managing, the object should already be ready for pooling (E.G. inactive).
        /// </summary>
        /// <param name="menu"></param>
        private void AddPoolObject(MCMenu menu)
        {
            Queue<MCMenu> menus;
            if (!_menuPool.TryGetValue(menu.Id, out menus))
            {
                menus = new Queue<MCMenu>();
                _menuPool.Add(menu.Id, menus);
            }

            menus.Enqueue(menu);
        }

        /// <summary>
        /// Checks if any menus are currently active.
        /// </summary>
        /// <returns>True if any menu is active, false if no menu is active.</returns>
        public bool AnyMenuActive()
        {
            return _activeMenus.Count > 0;
        }

        /// <summary>
        /// Called when a popup is added directly to the McMenu and will make sure the OutsideClick is correctly configured.
        /// </summary>
        /// <param name="mcMenu"></param>
        public void OnMenuAdded(MCMenu mcMenu)
        {
            InitializeOutsideClick(mcMenu);
        }
#endregion

#region Static methods
        /// <summary>
        /// Show a menu with a specific id on the global MenuManager.
        /// </summary>
        /// <param name="id">The unique id of a menu.</param>
        /// <param name="mcMenuData"></param>
        public static MCMenu ShowMenuGlobal(string id, object mcMenuData = null)
        {
            return _instance.ShowMenu(id, mcMenuData);
        }

        /// <summary>
        /// Get a menu from the active menus.
        /// </summary>
        /// <param name="id"></param>
        /// <remarks>Uses linear search.</remarks>
        /// <returns>Null if no active menu of this type exists.</returns>
        public static MCMenu GetMenuActiveGlobal(string id)
        {
            return _instance.GetMenuActive(id);
        }

        /// <summary>
        /// Try to get a menu of type from the active menus.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mcMenu">Set to null if no menu of this type is found. Otherwise is the menu that that is active with type 'id'.</param>
        /// <remarks>Uses linear search.</remarks>
        /// <returns>False if no menu of this type is found. </returns>
        public static bool TryGetMenuActiveGlobal(string id, out MCMenu mcMenu)
        {
            return _instance.TryGetMenuActive(id, out mcMenu);
        }

        /// <summary>
        /// Gets all active menus on the global MenuController instance.
        /// </summary>
        /// <remarks>Converts internal list to array, as the internal list should not be modified.
        /// Only returns active menu objects of the global MenuController instance, use <see cref="GetAllActiveMenuControllers"/> in combination with this method to retrieve all active menus on all controllers</remarks>
        /// <returns>Array of active MCMenus objects.</returns>
        public static MCMenu[] GetActiveMenusGlobal()
        {
            return _instance.GetActiveMenus();
        }

        /// <summary>
        /// Will return a duplicate of the original menu in the global MenuController so it can be used in the method <see cref="ShowMenu(WeersProductions.MCMenu,object)"/>
        /// </summary>
        /// <param name="id">The TYPE of the menu</param>
        /// <returns></returns>
        public static MCMenu GetMenuGlobal(string id)
        {
            return _instance.GetMenu(id);
        }

        /// <summary>
        /// Show a specific menu in the global MenuController.
        /// If this menu needs fullscreen and the current active menus allow it to override them, it will close the current menus.
        /// </summary>
        /// <param name="mcMenu">The new menu. (this is a copy of the original)</param>
        /// <param name="mcMenuData"></param>
        public static MCMenu ShowMenuGlobal(MCMenu mcMenu, object mcMenuData = null)
        {
            return _instance.ShowMenu(mcMenu, mcMenuData);
        }

        /// <summary>
        /// Toggle the visibility of a menu in the global MenuController.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mcMenuData">Any data that needs to be passed if this menu gets visible.</param>
        /// <returns></returns>
        public static MCMenu ToggleMenuGlobal(string id, object mcMenuData = null)
        {
            if (TryGetMenuActiveGlobal(id, out MCMenu menu))
            {
                HideMenuGlobal(menu);
            }
            else
            {
                menu = GetMenuGlobal(id);
                ShowMenuGlobal(menu, mcMenuData);
            }
            return menu;
        }

        /// <summary>
        /// Hide all menus for the global MenuController.
        /// </summary>
        /// <param name="hideAll">If true it will set all children as inactive (it will not <see cref="HideMenu"/> them!) of the MenuController, even those that are not marked as active.</param>
        public static void HideAllMenusGlobal(bool hideAll = false)
        {
            _instance.HideAllMenus(hideAll);
        }

        /// <summary>
        /// Will hide all menus for the global MenuController that are blocking other menus from showing up.
        /// </summary>
        public static void HideAllBlockingMenusGlobal()
        {
            _instance.HideAllBlockingMenus();
        }

        /// <summary>
        /// Hide a specific menu for the global MenuController.
        /// </summary>
        /// <param name="mcMenu"></param>
        public static void HideMenuGlobal(MCMenu mcMenu)
        {
            _instance.HideMenu(mcMenu);
        }

        /// <summary>
        /// Checks the queue of menus (of the global MenuController) waiting to be activated and will activate them if possible.
        /// </summary>
        /// <returns>True if a new menu is activated that was in the queue.</returns>
        private static bool CheckMenuQueueGlobal()
        {
            return _instance.CheckMenuQueue();
        }

        /// <summary>
        /// Will add a menu popup for the current visible menu (first one that was visible) for the global MenuController.
        /// </summary>
        /// <param name="mcMenu"></param>
        /// <param name="createWhenNoMenu">If true this popup will be made an active window in the case of no active windows at the time of calling this.
        /// If false, no popup will be shown if there is no active window.</param>
        /// <param name="data"></param>
        public static MCMenu AddPopupGlobal(MCMenu mcMenu, bool createWhenNoMenu, object data = null)
        {
            return _instance.AddPopup(mcMenu, createWhenNoMenu, data);
        }

        /// <summary>
        /// A wrapper for <see cref="AddPopup(MCMenu,bool, object)"/> for the global menu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createWhenNoMenu"></param>
        /// <param name="data"></param>
        public static MCMenu AddPopupGlobal(string id, bool createWhenNoMenu, object data = null)
        {
            return _instance.AddPopup(id, createWhenNoMenu, data);
        }

        /// <summary>
        /// Add a popup with an id to an existing menu to the global MenuController
        /// </summary>
        /// <param name="id">The id of the new popup.</param>
        /// <param name="parent">The parent object that already exists.</param>
        /// <param name="data">Data that should be passed on to the popup that is created.</param>
        /// <returns></returns>
        public static MCMenu AddPopupGlobal(string id, MCMenu parent, object data = null)
        {
            return _instance.AddPopup(id, parent, data);
        }

        /// <summary>
        /// Called when a popup is added directly to the McMenu and will make sure the OutsideClick is correctly configured.
        /// </summary>
        /// <param name="mcMenu"></param>
        public static void OnMenuAddedGlobal(MCMenu mcMenu)
        {
            _instance.InitializeOutsideClick(mcMenu);
        }

        /// <summary>
        /// Add a poolObject to the pool (of the global MenuController) for a certain menu.
        /// Only does data managing, the object should already be ready for pooling (E.G. inactive).
        /// </summary>
        /// <param name="menu"></param>
        private static void AddPoolObjectGlobal(MCMenu menu)
        {
            _instance.AddPoolObject(menu);
        }

        /// <summary>
        /// Checks if any menus are currently active for the global MenuController.
        /// </summary>
        /// <returns>True if any menu is active, false if no menu is active.</returns>
        public static bool AnyMenuActiveGlobal()
        {
            return _instance.AnyMenuActive();
        }

        /// <summary>
        /// Very expensive, store the results instead of calling it every time.
        /// </summary>
        /// <returns></returns>
        public static MenuController[] GetAllActiveMenuControllers()
        {
            return GameObject.FindObjectsOfType(typeof(MenuController)) as MenuController[];
        }
#endregion
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