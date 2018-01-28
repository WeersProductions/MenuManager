﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WeersProductions
{
    /// <inheritdoc />
    /// <summary>
    /// The base class for all menus/popups/tooltips. Should be used whenever creating a new one.
    /// </summary>
    public class MCMenu : MonoBehaviour
    {
        /// <summary>
        /// A unique id for this menu.
        /// </summary>
        [SerializeField]
        private MenuController.Menus _id;

        /// <summary>
        /// If true clicking outside of the window will be blocked.
        /// </summary>
        [SerializeField]
        private bool _blockOutsideRaycasting = true;

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
        /// If true, this screen will always be on top of the other screens.
        /// Undefined behaviour if multiple screens have this true and are active at the same time.
        /// </summary>
        [SerializeField]
        private bool _alwaysOnTop;

        //TODO: Add pool amount if it should be pooled (E.G. max 5 objects in pool).

        /// <summary>
        /// A list of popups that this menu owns.
        /// These will be closed when this menu will be closed.
        /// </summary>
        private readonly List<MCMenu> _popupMenus = new List<MCMenu>();

        /// <summary>
        /// If this is a popup menu, or a menu owned by another menu, the parent is not null.
        /// </summary>
        private MCMenu _parent;

        /// <summary>
        /// If not null, called when the Menu is being hidden.
        /// </summary>
        private UnityAction _callBackHide;

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
        /// If true, a new menu will not be able to be shown unless this one is hidden.
        /// </summary>
        public bool ShouldBlockNewMenu
        {
            get { return !CanBeClosed && Fullscreen; }
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
        /// If true clicking outside of the window will be blocked.
        /// </summary>
        public bool BlockOutsideRaycasting
        {
            get { return _blockOutsideRaycasting; }
        }

        /// <summary>
        /// If true, this screen will always be on top of the other screens.
        /// Undefined behaviour if multiple screens have this true and are active at the same time.
        /// </summary>
        public bool AlwaysOnTop
        {
            get { return _alwaysOnTop; }
        }

        /// <summary>
        /// Called when this menu should be shown, can be used for animations.
        /// </summary>
        /// <param name="data">Can be used to send extra data to the menu.</param>
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
            if (_callBackHide != null)
            {
                afterHidden += _callBackHide;
            }

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
        /// Will add a callback when the screen is hidden and call it.
        /// </summary>
        /// <param name="action">The action that should be called when the menu is hidden.</param>
        public void HideAndCall(UnityAction action)
        {
            _callBackHide = action;
            Hide();
            _callBackHide = null;
        }

        /// <summary>
        /// Called when the Menu is being hidden.
        /// Only this menu will be hidden here, no popups or child menus are called in here.
        /// </summary>
        /// <param name="afterHidden">Should be called when the menu is actually hidden. 
        /// This means that when you have fade out animations, you should call this when the fade out is complete.</param>
        protected virtual void OnHide(UnityAction afterHidden)
        {
            gameObject.SetActive(false);
            afterHidden();
        }

        /// <summary>
        /// Add a popup to this menu and assign this menu as the parent for the popup.
        /// </summary>
        /// <param name="mcMenu">The new popup object.</param>
        /// <param name="data">Data that is passed to the popup that is created.</param>
        public MCMenu AddPopup(MCMenu mcMenu, object data = null)
        {
            PopupMenus.Add(mcMenu);
            mcMenu.Parent = this;
            mcMenu.Show(data);

            MenuController.OnMenuAdded(mcMenu);
            return mcMenu;
        }

        /// <summary>
        /// Add a popup to this menu.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data">Data that is passed to the popup that is created.</param>
        /// <returns>The new popup object.</returns>
        public MCMenu AddPopup(MenuController.Menus id, object data = null)
        {
            return MenuController.AddPopup(id, this, data);
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
        /// Remove all popups with a certain Id from this menu.
        /// </summary>
        /// <param name="id"></param>
        public void RemovePopup(MenuController.Menus id)
        {
            List<MCMenu> copy = new List<MCMenu>(PopupMenus);
            for (int i = 0; i < copy.Count; i++)
            {
                if (copy[i].Id == id)
                {
                    RemovePopup(copy[i]);
                }
            }
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
        /// After calling this, the object should be ready for reuse as if it was just instantiated.
        /// </summary>
        public virtual void PrepareForPool()
        {

        }

        /// <summary>
        /// Called when the user presses outside this window.
        /// </summary>
        public virtual void OnClickOutside()
        {
            
        }

#if UNITY_EDITOR
        /// <summary>
        /// Used by the editor script, normally you do not want to set the id and therefore it's not available during run-time.
        /// </summary>
        /// <param name="menus"></param>
        public void SetId(MenuController.Menus menus)
        {
            _id = menus;
        }
#endif
    }
}