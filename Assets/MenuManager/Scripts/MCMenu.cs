using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WeersProductions
{
    public class MCMenu : MonoBehaviour
    {
        /// <summary>
        /// A unique id for this menu.
        /// </summary>
        [SerializeField]
        private MenuController.Menus _id;

        /// <summary>
        /// If true this menu wants the whole screen and thus cannot be shown next to another menu.
        /// </summary>
        [SerializeField]
        private bool _fullscreen;

        /// <summary>
        /// If true, when another menu wants to show up, it will close this menu.
        /// If false, the new menu will be put in a queue for when this menu gets closed.
        /// </summary>
        [SerializeField]
        private bool _canBeClosed = true;

        /// <summary>
        /// When this menu cannot be shown immediatly, it should be queued for when there is space on screen to show it.
        /// </summary>
        [SerializeField]
        private bool _shouldBeQueued;

        /// <summary>
        /// If true this menu will be pooled and thus will not be destroyed after use.
        /// </summary>
        [SerializeField]
        private bool _shouldBePooled = true;

        /// <summary>
        /// TODO: implement this so that a menu can be always on top.
        /// </summary>
        [SerializeField]
        private bool _alwaysOnTop;

        //TODO: Add pool amount if it should be pooled (E.G. max 5 objects).

        /// <summary>
        /// A list of popups that this menu owns.
        /// These will be closed when this menu will be closed.
        /// </summary>
        private readonly List<MCMenu> _popupMenus = new List<MCMenu>();

        /// <summary>
        /// If this is a popup menu, or a menu owned by another menu, the parent is not null.
        /// </summary>
        private MCMenu _parent;

        public MenuController.Menus Id
        {
            get { return _id; }
        }

        public bool Fullscreen
        {
            get { return _fullscreen; }
        }

        /// <summary>
        /// If true, when another menu wants to show up, it will close this menu.
        /// If false, the new menu will be put in a queue for when this menu gets closed.
        /// </summary>
        public bool CanBeClosed
        {
            get { return _canBeClosed; }
        }

        /// <summary>
        /// If this is a popup menu, or a menu owned by another menu, the parent is not null.
        /// </summary>
        public MCMenu Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// A list of popups that this menu owns.
        /// These will be closed when this menu will be closed.
        /// </summary>
        public List<MCMenu> PopupMenus
        {
            get { return _popupMenus; }
        }

        /// <summary>
        /// When this menu cannot be shown immediatly, it should be queued for when there is space on screen to show it.
        /// </summary>
        public bool ShouldBeQueued
        {
            get { return _shouldBeQueued; }
        }

        /// <summary>
        /// If true this menu will be pooled and thus will not be destroyed after use.
        /// </summary>
        public bool ShouldBePooled
        {
            get { return _shouldBePooled; }
        }

        /// <summary>
        /// Called when this menu should be shown, can be used for animations.
        /// </summary>
        public virtual void Show(object data)
        {
            gameObject.SetActive(true);
            gameObject.transform.SetAsLastSibling();
        }

        /// <summary>
        /// A simple wrapper that will call <see cref="MenuController.HideMenu"/> for itself.
        /// </summary>
        public void Hide()
        {
            MenuController.HideMenu(this);
        }

        /// <summary>
        /// Called when this menu should be hidden.
        /// </summary>
        /// <param name="afterHidden">A function that should be called when this menu is hidden.</param>
        public void Hide(UnityAction afterHidden)
        {
            // Remove all null values for popupMenus that are destroyed.
            PopupMenus.RemoveAll(menu => !menu);

            if (PopupMenus.Count <= 0)
            {
                // There are no popups to take care of, increase performance and skip those.
                OnHide(afterHidden);
            }
            else
            {
                RemoveAllPopups(() =>
                {
                // There are no popupMenus left, we can actually hide the parent menu.
                OnHide(afterHidden);
                });
            }
        }

        /// <summary>
        /// Called when the Menu is being hidden.
        /// Only this menu will be hidden here, no popups or child menus are called in here.
        /// </summary>
        /// <param name="afterHidden"></param>
        protected virtual void OnHide(UnityAction afterHidden)
        {
            gameObject.SetActive(false);
            afterHidden();
        }

        /// <summary>
        /// Add a popup to this menu and assign this menu as the parent for the popup.
        /// </summary>
        /// <param name="mcMenu"></param>
        public void AddPopup(MCMenu mcMenu, object data = null)
        {
            PopupMenus.Add(mcMenu);
            mcMenu.Parent = this;
            mcMenu.Show(data);
        }

        /// <summary>
        /// Remove a specific popup menu.
        /// </summary>
        /// <param name="mcMenu"></param>
        public void RemovePopup(MCMenu mcMenu)
        {
            PopupMenus.Remove(mcMenu);
            mcMenu.Parent = null;
            mcMenu.Hide();
        }

        /// <summary>
        /// Removes all popups from this menu.
        /// </summary>
        /// <param name="afterHidden"></param>
        public void RemoveAllPopups(UnityAction afterHidden)
        {
            // Create a copy to not edit the list while looping through it.
            List<MCMenu> popupMenuCopy = new List<MCMenu>(PopupMenus);
            for (int i = 0; i < popupMenuCopy.Count; i++)
            {
                int currentIndex = i;

                // Hide each popupMenu.
                popupMenuCopy[i].Hide(() =>
                {
                // Called when this popup menu is closed.
                PopupMenus.Remove(popupMenuCopy[currentIndex]);

                    if (PopupMenus.Count <= 0)
                    {
                        if (afterHidden != null)
                        {
                            afterHidden();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// After calling this, the object should be ready for reuse as if it was instantiated.
        /// </summary>
        public virtual void PrepareForPool()
        {
            
        }

#if UNITY_EDITOR
        public void SetId(MenuController.Menus menus)
        {
            _id = menus;
        }
#endif
    }
}