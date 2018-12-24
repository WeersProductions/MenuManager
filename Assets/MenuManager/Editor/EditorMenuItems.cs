using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

namespace WeersProductions
{
	public static class EditorMenuItems {

	[MenuItem("GameObject/WeersProductions/Add menu", false, 0)]
        private static void AddMenu(MenuCommand menuCommand)
        {
            if(Selection.objects.Length > 1) {
                if(menuCommand.context != Selection.objects[0]) {
                    return;
                }
            }
            // Use the selected objects. 
            MenuController[] menuControllers = HierarchyHelper.GetSelectedOrGeneralObjectsOfType<MenuController>();
            if(menuControllers.Length <= 0) {
                if(EditorUtility.DisplayDialog("Adding menu", "Could not find any MenuController in this scene.", "Create MenuController", "Cancel")) {
                    // If the user wanted to create a menucontroller, set the new menucontroller that is created as selected.
                    CreateMenuController((MenuController menuController) => {
                        AddMenuToMenuController(new MenuController[]{menuController});
                    });
                } else {
                    return;
                }
            } else {
                AddMenuToMenuController(menuControllers);
            }
        }

        private static void AddMenuToMenuController(MenuController[] menuControllers) {
            if(menuControllers.Length > 1) {
                // Show a popup so the user can choose what menuController to use.
                try {
                    Vector2 coordinates = EditorUtils.GetMousePosition();
                    // TODO: fix popup location.
                    PopupWindow.Show(new Rect(GUIUtility.ScreenToGUIPoint(coordinates), new Vector2(250, 150)), new MenuControllerSelector(menuControllers, (MenuController menuController) => {
                    AddMenuToTransform(menuController.transform);
                }));
                } catch (UnityEngine.ExitGUIException) {
                    // https://answers.unity.com/questions/385235/editorguilayoutcolorfield-inside-guilayoutwindow-c.html
                }
            } else {
                AddMenuToTransform(menuControllers[0].transform);
            }
            // TODO: Add the new menu to the selected menuController
        }

        private static void AddMenuToTransform(Transform parent) {
            if (!parent)
            {
                return;
            }

            GameObject newMenu = new GameObject("New menu");
            Undo.RegisterCreatedObjectUndo(newMenu, "Create menu");
            Undo.AddComponent<MCMenu>(newMenu);
            Undo.AddComponent<RectTransform>(newMenu);

            if (parent)
            {
                Undo.SetTransformParent(newMenu.GetComponent<Transform>(), parent, "Create menu");
                EditorUtils.Collapse(parent.gameObject, true);
            }
        }

        /// <summary>
        /// Shows popup for the user to choose a Canvas that will be used and adds a MenuController component to it.
        /// </summary>
        private static void CreateMenuController(UnityEngine.Events.UnityAction<MenuController> onFinish) {
            Canvas[] canvasses = HierarchyHelper.GetSelectedOrGeneralObjectsOfType<Canvas>();
            Vector2 coordinates = EditorUtils.GetMousePosition();
            try {
                PopupWindow.Show(new Rect(GUIUtility.ScreenToGUIPoint(coordinates), new Vector2(250, 150)), new CanvasSelector(canvasses, (Canvas canvas) => {
                    if(canvas == null) {
                        // The user wants us to create a canvas
                        EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                        Canvas[] newCanvasses = HierarchyHelper.GetObjectsOfType<Canvas>();
                        canvasses = newCanvasses.Except(canvasses).ToArray();
                        if(canvasses == null || canvasses.Length <= 0) {
                            Debug.LogError("Something went wrong with creating the new canvas. Please try again.");
                            return;
                        }
                        canvas = canvasses[0];
                    }
                    MenuController result = Undo.AddComponent<MenuController>(canvas.gameObject);
                    if(onFinish != null) {
                        onFinish(result);
                    }
                }));
            } catch (UnityEngine.ExitGUIException) {
                // https://answers.unity.com/questions/385235/editorguilayoutcolorfield-inside-guilayoutwindow-c.html
            }
        }
	}
}

