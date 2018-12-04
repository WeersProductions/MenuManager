using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WeersProductions
{
    public class MCSimplePopupData : MCSimpleTooltipData
    {
        public MCSimplePopupData(string title, string description, MCButtonData[] buttonDatas)
            : base(title, description)
        {
            ButtonDatas = buttonDatas;
        }

        public MCButtonData[] ButtonDatas;
    }
}