# MenuManager
MenuManager is a manager for menus inside UGUI.

With just a small amount of code you can create relations between different menus. Think of popups, tooltips but also any regular menu.

# Getting started
## Set-up
Clone the repository, or download a UnityPackage from the [forums](https://forum.unity.com/threads/wip-open-source-menumanager.503537/). Open te project in Unity. The ```MenuManager``` folder contains all the code required to use the manager.

## Demo
In the Demo folder two demos show you how to use the MenuManager.

| Demo | Shows
--- | ---
| Popups | How to work with popups, tooltips and parent-child relations|
| MultipleMenuControllers | How to use multiple menu controllers with shared menus|

## Create and select MenuController
1. Create and/or open scene
2. Open MenuManager editor (Window -> WeersProductions -> MenuManager)
3. Press 'Create MenuController'
4. Select an existing canvas of your scene, or press 'Create new'
5. Select the newly created MenuController in the MenuManager editor on 
top
<img src="https://user-images.githubusercontent.com/22612711/50911543-63fcac00-1430-11e9-8c62-748314815597.png" alt="drawing" width="400"/>

## Adding a new menu
This system makes use of Unity Prefabs for different menus. Each menu (or popup/tooltip) is a prefab with a ```MCMenu``` component (or a subclass), which is registered in the ```MenuController``` class.

First you create your menu like you would usally do in the scene editor as a GameObject. This GameObject should have a ```MCMenu``` component or a subclass.
To get started you can right click in the Hierarchy panel and click WeersProductions->Menu. This will create a new panel with the ```MCMenu``` component.

Once you have finished your menu, make it prefab and delete the Scene instance. Drag your prefab to the 'Specific menus' drag area. This will add the new menu to the selected MenuController.

Since we need a reference to the menu in code, add your menu to the ```Menus``` enum in the ```MenuController``` file, with the value being the same as the ```ID``` of the ```MCMenu``` component.
```csharp
public enum Menus
        {
            UNDEFINED = -2,
            NONE = -1,
            SIMPLEPOPUP = 0,
            SIMPLETOOLTIP = 1
        }
```
```csharp
MenuController.ShowMenuGlobal(Menus.SIMPLETOOLTIP);
```
To show the new menu.

## Multiple MenuControllers
If you have, for example, a world canvas and a screen-space canvas, you might want to use two MenuControllers. This way they will keep track of their own states and pool their own menus. 
In that case, calling ```ShowMenuGlobal``` does not give enough information about what MenuController you are talking about.

Keep a reference to the MenuControllers and call
```csharp
// Assign this sometime, E.G. in the editor.
MenuController _menuController;
// Call ShowMenu on the instance.
_menuController.ShowMenu(Menus.SIMPLETOOLTIP);
```

If you want a menu that should be possible to shown on every MenuController there is, without having to manually drag the prefab for each MenuController, use the 'Shared menus' drag area. These menus are added to all MenuControllers of your Unity project. 

## Editor
If your project contains a lot of menus, presets might help you organize them. 
 
1. Go to the tab ```Create preset```, enter a name and click ```Create new```. Click on the new preset and set the description field. 
2. Drag your menu prefab to the 'Preset Object' field at the bottom of the editor. 
3. Go to the tab ```Create menu```, select your new preset and click ```Create Menu```. This will instantiate a new object for the current selected MenuController. Edit this, and follow 'Adding a new menu' to use it. 


## Example

![A gif showing the demo](https://user-images.githubusercontent.com/22612711/32513569-d6973cc6-c3fa-11e7-9106-eada8ba07e85.gif)

This is all the code you need to create it!
```csharp
private void Start()
{
    CreatePopup(0, null);
}

private void CreatePopup(int count, MCMenu parent)
{
    // Get the Menu object, we need this because we have a button that closes the menu.
    MCMenu popupMenu = MenuController.GetMenuGlobal(MenuController.Menus.SIMPLEPOPUP);

    // Create the data for the menu.
    MCSimplePopupData simplePopupData = new MCSimplePopupData("title " + count, "This is another popup.",
        new MCButtonData[]
        {
            new MCButtonData("New popup", button => { CreatePopup(count + 1, popupMenu); }, null, true, "Creates a new popup"),
            new MCButtonData("Tooltip", null, null, true, "Simply shows the tooltip working"),
            new MCButtonData("Close parent", button => { MenuController.HideMenuGlobal(popupMenu.Parent); }, null, true, "Closes the parent menu (which will close all children)"),
            new MCButtonData("Close this", button => popupMenu.Hide(), null, true, "Closes this popup")
        });

    if (parent)
    {
        parent.AddPopup(popupMenu, simplePopupData);
    }
    else
    {
        // Add the popup to the screen, when there is nothing on the screen it will be added as a menu instead of a popup. 
        MenuController.AddPopupGlobal(popupMenu, true, simplePopupData);

        // In case you have a specific menucontroller that you want to use:
        // yourMenuController.AddPopup(popupMenu, true, simplePopupData);
    }
}
```

Take a look at the example code and the custom MCMenu classes, ```MCSimplePopup``` and ```MCSimpleToolTip```. These get additional data when they are shown, ```MCSimplePopupData``` and ```MCSimpleTooltipData```. 

If you have any questions of how to use this, or any suggestions to add, don't hesitate to send me a message!