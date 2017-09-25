using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeersProductions;

namespace WeersProductions
{
    /// <summary>
    /// Adds buttons to the tooltip and makes it draggable.
    /// </summary>
    public class MCSimplePopup : MCSimpleTooltip, IDraggableMenu
    {
        [SerializeField]
        private Button _buttonPrefab;

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
            
            bool hasButtons = simplePopupData.ButtonActions !=  null && simplePopupData.ButtonActions.Length > 0;
            _disableIfNoButtons.SetActive(hasButtons);
            if (hasButtons)
            {
                for (int i = 0; i < simplePopupData.ButtonActions.Length; i++)
                {
                    Button newButton = Instantiate(_buttonPrefab, _buttonParent);
                    newButton.onClick.RemoveAllListeners();
                    var numberIndex = i;
                    newButton.onClick.AddListener(() => simplePopupData.ButtonActions[numberIndex](newButton));

                    if (simplePopupData.ButtonSprites != null && simplePopupData.ButtonSprites.Length > i)
                    {
                        Image imageChild = newButton.GetComponentInChildren<Image>();
                        imageChild.sprite = simplePopupData.ButtonSprites[i];
                    }
                    else
                    {
                        Text textChild = newButton.GetComponentInChildren<Text>();
                        textChild.text = simplePopupData.ButtonStrings[i];
                    }

                    newButton.gameObject.SetActive(true);
                }
            }

            _buttonPrefab.gameObject.SetActive(false);

            gameObject.SetActive(true);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Show something visual");
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("Stop the special visual");
        }

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
    }
}