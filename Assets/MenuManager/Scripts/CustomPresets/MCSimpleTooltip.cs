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

        /// <summary>
        /// Offset from the mouse. 
        /// </summary>
        private readonly Vector3 _offset = new Vector3(-10, 0, 0);

        private Coroutine _updatePosition;

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

            DisableIfNoTitle.SetActive(!string.IsNullOrEmpty(simplePopupData.Title));
            TitleText.text = simplePopupData.Title;

            DisableIfNoDescription.SetActive(!string.IsNullOrEmpty(simplePopupData.Description));
            DescriptionText.text = simplePopupData.Description;

            if (simplePopupData.HasCustomPosition)
            {
                RectTransform rectTransform = this.GetComponent<RectTransform>();
                if (!rectTransform)
                {
                    throw new Exception("No RectTransform could be found on this tooltip!");
                }

                if(_updatePosition == null) 
                {
                    _updatePosition = StartCoroutine(UpdatePosition(rectTransform));
                }
                
                if (simplePopupData.AutoRemove)
                {
                    // 'Register' us to the target component, by adding a component to the object.
                    _toolTipOwner = simplePopupData.ToolTipOwner.gameObject.AddComponent<MCToolTipOwner>();
                    _toolTipOwner.InsideListener = this;
                    _toolTipOwner.Inside = true;
                }
            }

            base.Show(data);
        }

        private IEnumerator UpdatePosition(RectTransform rectTransform)
        {
            while(true) 
            {
                rectTransform.position = Input.mousePosition - new Vector3(rectTransform.rect.width * 0.5f, 0, 0) + _offset;
                yield return null;
            }
        }

        public void MouseLeaves(MCToolTipOwner owner)
        {
            // Stop updating our position.
            if (_updatePosition != null) 
            {
                StopCoroutine(_updatePosition);
                _updatePosition = null;
            }

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

