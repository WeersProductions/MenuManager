# MenuManager
A MenuManager is a manager for menus inside UGUI, pretty straight forward right?

With just a small amount of code you can create relations between different menus. Think of popups, tooltips but also any regular menu.

## Adding a new menu
This system makes use of Unity Prefabs for different menus. Each menu (or popup/tooltip) is a prefab with a ```MCMenu``` component (or a subclass), which is registered in the ```MenuController``` class.

First you create your menu like you would usally do in the scene editor as a GameObject. This GameObject should have a ```MCMenu``` component or a subclass of it.
Then you create a preset using the custom editor and assign the MCMenu component to it. The preset is used for own organization in the editor, it is not used during run-time.

To create the actual menu and automatically assign it to the MenuController, simply head over to the ```Create Menu``` tab of the editor and select the new menu.

![Create menu tab](https://user-images.githubusercontent.com/22612711/32515498-97c4bd38-c400-11e7-8395-b50037c65118.png)

Depending on your settings in the ```Options``` tab, a new menu object is instantiated in the scene.

To easily have a reference to a type of menu, add your menu to the ```Menus``` enum, with the value being the same as the ```ID``` of the ```MCMenu``` component.
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
MenuController.ShowMenu(Menus.SIMPLETOOLTIP);
```
To show the new menu.


## Editor
Using a custom editor, it is very easy to add or edit so called presets. These presets are the base of a menu, which then can be visually customized.
 
![Add/Edit preset settings](https://user-images.githubusercontent.com/22612711/32513908-d169272c-c3fb-11e7-8189-a80b093d1b9a.png)

This is the menu to edit the preset 'tooltip'. MCSimpleToolTip is a class that extends the base class McMenu, which is used to control all menus. Every variable is documented in code.


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
    MCMenu popupMenu = MenuController.GetMenu(MenuController.Menus.SIMPLEPOPUP);

    // Create the data for the menu.
    MCSimplePopupData simplePopupData = new MCSimplePopupData("title " + count, "This is another popup.",
        new MCSimplePopupData.ButtonClick[]
        {
            button => { CreatePopup(count + 1, popupMenu); },
            button => popupMenu.Hide(),
            button => { MenuController.HideMenu(popupMenu.Parent); },
            button =>
            {
                MCSimpleTooltipData simpleTooltipData = new MCSimpleTooltipData("Tooltip", "More text",
                    button.GetComponent<RectTransform>()) {AutoRemove = true};
                MenuController.AddPopup(MenuController.Menus.SIMPLETOOLTIP, false, simpleTooltipData);
            }
        }, new[]
        {
            "New popup",
            "Close this",
            "Close parent",
            "Tooltip"
        });

    if (parent)
    {
        parent.AddPopup(popupMenu, simplePopupData);
    }
    else
    {
        // Add the popup to the screen, when there is nothing on the screen it will be added as a menu instead of a popup. 
        MenuController.AddPopup(popupMenu, true, simplePopupData);
    }
}
```

Take a look at the example code and the custom MCMenu classes, ```MCSimplePopup``` and ```MCSimpleToolTip```. These get additional data when they are shown, ```MCSimplePopupData``` and ```MCSimpleTooltipData```. 

If you have any questions of how to use this, or any suggestions to add, don't hesitate to send me a message!