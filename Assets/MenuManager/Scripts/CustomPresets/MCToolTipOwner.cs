using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeersProductions
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary>
    /// A very simple class that is used to 'own' tooltips during run-time.
    /// Everytime a tooltip is spawned this component is used to determine whether the mouse is still selecting the correct object.
    /// </summary>
    public class MCToolTipOwner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private IMouseInsideListener _insideListener;
        private bool _inside;

        /// <summary>
        /// An object that wants to know when the mouse is leaving this UI object.
        /// </summary>
        /// <example>Used for tooltips that should be destroyed when the mouse is not hoovering over an object anymore.</example>
        public IMouseInsideListener InsideListener
        {
            get { return _insideListener; }
            set { _insideListener = value; }
        }

        /// <summary>
        /// Used to 'remember' whether the mouse is inside this UI object.
        /// If set to false, the listener won't be notified when the mouse leaves this object.
        /// </summary>
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
