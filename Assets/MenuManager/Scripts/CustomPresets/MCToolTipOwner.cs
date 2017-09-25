using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeersProductions
{
    /// <inheritdoc cref="Component" />
    /// <summary>
    /// A very simple class that is used to 'own' tooltips during run-time.
    /// Everytime a tooltip is spawned this component is used to determine whether the mouse is still selecting the correct object.
    /// </summary>
    public class MCToolTipOwner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private IMouseInsideListener _insideListener;
        private bool _inside;

        public IMouseInsideListener InsideListener
        {
            get { return _insideListener; }
            set { _insideListener = value; }
        }

        public bool Inside
        {
            get { return _inside; }
            set { _inside = value; }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _inside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_inside)
            {
                // The mouse is leaving this object, we should remove the tooltip.
                _insideListener.MouseLeaves(this);
            }
        }
    }
}
