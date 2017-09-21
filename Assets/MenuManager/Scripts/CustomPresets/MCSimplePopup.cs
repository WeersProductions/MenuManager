using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeersProductions;

namespace WeersProductions
{
    public class MCSimplePopup : MCMenu
    {
        [SerializeField]
        private Text _titleText;
        [SerializeField]
        private GameObject _disableIfNoTitle;

        [SerializeField]
        private Text _descriptionText;
        [SerializeField]
        private GameObject _disableIfNoDescription;

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


            _disableIfNoTitle.SetActive(!string.IsNullOrEmpty(simplePopupData.Title));
            _titleText.text = simplePopupData.Title;

            _disableIfNoDescription.SetActive(!string.IsNullOrEmpty(simplePopupData.Description));
            _descriptionText.text = simplePopupData.Description;

            bool hasButtons = simplePopupData.ButtonActions !=  null && simplePopupData.ButtonActions.Length > 0;
            _disableIfNoButtons.SetActive(!hasButtons);
            if (hasButtons)
            {
                for (int i = 0; i < simplePopupData.ButtonActions.Length; i++)
                {
                    Button newButton = Instantiate(_buttonPrefab, _buttonParent);
                    newButton.onClick.RemoveAllListeners();
                    newButton.onClick.AddListener(simplePopupData.ButtonActions[i]);

                    
                    if (simplePopupData.ButtonSprites.Length > i)
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
        }
    }
}