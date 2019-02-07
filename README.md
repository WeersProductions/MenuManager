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

## Good practice
### Data-flow
The main structure when looking at the data-flow when showing a new menu is as follows:
1. Create a new ```data``` object. This will contain data for the specific menu. Think of an object that contains the following properties:
    - Message
    - Color of the message
    - An icon that should be displayed next to the message

    Example files:
    - [GeneralWindowData](./Demo/MultipleMenuControllers/Scripts/GeneralWindowData.cs)
    - [PopupData](./Scripts/CustomPresets/MCSimplePopupData.cs)
 
 2. This data will be send to your menu class, that inherits from ```McMenu```. This class will use the data from the data object to show specific things on screen. In our example, think of a Message class that will set a UI Text component's text to the Message property, its color to the Color property and sets a UI Image component's Sprite property to the Icon that is sent. 
 
    Example files:
    - [GeneralWindow](./Demo/MultipleMenuControllers/Scripts/GeneralWindow.cs)
    - [Popup](./Scripts/CustomPresets/MCSimplePopup.cs)

 
 You can see a very simple example in [MainWindow](./Demo/MultipleMenuControllers/Scripts/MainWindow.cs) of how to create a data object and use it when showing a menu.

## Demos

Achieve interesting menu set-ups with very little code. Click on the demos to go to the demo readme with more explanation. 

| | |
|:-------------------------:|:-------------------------:|
| <a href="./Demo/MultipleMenuControllers/readme.md"><img width="600" alt="Popups" src="https://user-images.githubusercontent.com/22612711/51031348-975c4980-159c-11e9-80c5-d14a1079f813.png"> Multiple Managers</a> | <a href="./Demo/Popups/readme.md"><img width="600" alt="Multiple managers" src="https://user-images.githubusercontent.com/22612711/51031529-21a4ad80-159d-11e9-8b3e-95b0584f8239.png"> Popups</a> |


If you have any questions of how to use this, or any suggestions on what features are missing, don't hesitate to send me a message!