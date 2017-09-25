using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WeersProductions
{
    public class MCSimplePopupData : MCSimpleTooltipData
    {
        public delegate void ButtonClick(Button button);

        public MCSimplePopupData(string title, string description, ButtonClick[] buttonActions, string[] buttonStrings)
            : base(title, description)
        {
            ButtonActions = buttonActions;
            ButtonStrings = buttonStrings;
        }

        public MCSimplePopupData(string title, string description, ButtonClick[] buttonActions,
            Sprite[] buttonSprites) : base(
            title, description)
        {
            ButtonActions = buttonActions;
            ButtonSprites = buttonSprites;
        }

        public ButtonClick[] ButtonActions { get; set; }

        public string[] ButtonStrings { get; set; }

        public Sprite[] ButtonSprites { get; set; }
    }
}