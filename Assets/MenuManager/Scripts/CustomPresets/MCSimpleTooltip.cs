using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeersProductions;

namespace WeersProductions
{
    /// <inheritdoc cref="MCMenu" />
    /// <inheritdoc cref="IMouseInsideListener"/>
    /// <summary>
    /// A simple tooltip that can be controlled by any gameobject that has a <see cref="T:WeersProductions.MCToolTipOwner" />
    /// </summary>
    public class MCSimpleTooltip : MCMenu, IMouseInsideListener
    {
        [SerializeField]
        protected Text TitleText;
        [SerializeField]
        protected GameObject DisableIfNoTitle;

        [SerializeField]
        protected GameObject DisableIfNoDescription;

        [SerializeField]
        protected Text DescriptionText;

        private MCToolTipOwner _toolTipOwner;

        public override void Show(object data)
        {
            if (data == null)
            {
                throw new Exception("Trying to start a simple popup with a data that is equal to null!");
            }

            MCSimpleTooltipData simplePopupData = data as MCSimpleTooltipData;
            if (simplePopupData == null)
            {
                throw new Exception("Trying to show a simple tooltip, but using the wrong data type: " + data.GetType());
            }

            if (simplePopupData.HasCustomPosition)
            {
                RectTransform rectTransform = this.GetComponent<RectTransform>();
                if (!rectTransform)
                {
                    throw new Exception("No RectTransform could be found on this tooltip!");
                }
                
                rectTransform.position = simplePopupData.Position;

                if (simplePopupData.AutoRemove)
                {
                    // 'Register' us to the target component, by adding a component to the object.
                    _toolTipOwner = simplePopupData.ToolTipOwner.gameObject.AddComponent<MCToolTipOwner>();
                    _toolTipOwner.InsideListener = this;
                    _toolTipOwner.Inside = true;
                }
            }

            DisableIfNoTitle.SetActive(!string.IsNullOrEmpty(simplePopupData.Title));
            TitleText.text = simplePopupData.Title;

            DisableIfNoDescription.SetActive(!string.IsNullOrEmpty(simplePopupData.Description));
            DescriptionText.text = simplePopupData.Description;

            base.Show(data);
        }

        public void MouseLeaves(MCToolTipOwner owner)
        {
            // 'Unregister' us.
            Destroy(owner);

            // Hide the tooltip.
            Hide();
        }

        /// <inheritdoc />
        protected override void OnHide(UnityAction afterHidden)
        {
            if (_toolTipOwner)
            {
                Destroy(_toolTipOwner);
            }
            base.OnHide(afterHidden);
        }
    }
}

