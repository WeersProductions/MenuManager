using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeersProductions;

namespace WeersProductions
{
    /// <inheritdoc cref="MCSimpleTooltip" />
    /// <inheritdoc cref="IDraggableMenu"/>
    /// <summary>
    /// Adds buttons to the tooltip and makes it draggable.
    /// </summary>
    public class MCSimplePopup : MCSimpleTooltip, IDraggableMenu
    {
        [SerializeField]
        private Button _buttonPrefab;

        /// <summary>
        /// The transform that is used as a parent for all the parents.
        /// </summary>
        [SerializeField]
        private RectTransform _buttonParent;
        [SerializeField]
        private GameObject _disableIfNoButtons;

        public override void Show(object data)
        {
            if (data == null)
            {
                throw new Exception("Trying to start a simple popup with a data that is equal to null!");
            }

            base.Show(data);

            MCSimplePopupData simplePopupData = data as MCSimplePopupData;
            if (simplePopupData == null)
            {
                throw new Exception("Trying to show a simple popup, but using the wrong data type: " + data.GetType());
            }
            
            bool hasButtons = simplePopupData.ButtonDatas !=  null && simplePopupData.ButtonDatas.Length > 0;
            _disableIfNoButtons.SetActive(hasButtons);
            for (int i = 0; i < simplePopupData.ButtonDatas.Length; i++)
            {   
                Button newButton = Instantiate(_buttonPrefab, _buttonParent);
                newButton.onClick.RemoveAllListeners();
                MCButtonData buttonData = simplePopupData.ButtonDatas[i];
                if (buttonData.ButtonClick != null) 
                {
                    newButton.onClick.AddListener(() => buttonData.ButtonClick(newButton));
                }

                // Use the icon if one is present, use text otherwise.
                if (buttonData.Icon != null)
                {
                    Image imageChild = newButton.GetComponentInChildren<Image>();
                    imageChild.sprite = buttonData.Icon;
                }
                else
                {
                    Text textChild = newButton.GetComponentInChildren<Text>();
                    textChild.text = buttonData.Text;
                }

                // Create the tooltip component and add the OnHover components to the object if it wants a tooltip.
                if (buttonData.Tooltip) 
                {
                    MCSimpleTooltipData simpleTooltipData = new MCSimpleTooltipData("Tooltip", buttonData.TooltipText,
                    newButton.GetComponent<RectTransform>()) {AutoRemove = true};

                    OnHover onHover = newButton.GetComponent<OnHover>();
                    if(!onHover) 
                    {
                        onHover = newButton.gameObject.AddComponent<OnHover>();
                    }
                    onHover.Delay = 1;
                    onHover.onPointerDelay += () => {
                        this.MenuController.AddPopup("SIMPLETOOLTIP", false, simpleTooltipData);
                    };
                }

                newButton.gameObject.SetActive(true);
            }

            _buttonPrefab.gameObject.SetActive(false);

            gameObject.SetActive(true);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Show that we are dragging.
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Stop showing that we are dragging.
        }

        /// <inheritdoc />
        public override void PrepareForPool()
        {
            base.PrepareForPool();
            Button[] buttons = _buttonParent.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != _buttonPrefab)
                {
                    Destroy(buttons[i].gameObject);
                }
            }
            _buttonPrefab.gameObject.SetActive(true);
        }

        public override void OnClickOutside()
        {
            Hide();
        }
    }
}