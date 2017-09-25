using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeersProductions
{
    public interface IDraggableMenu
    {
        void OnBeginDrag(PointerEventData eventData);
        void OnDrag(PointerEventData eventData);
        void OnEndDrag(PointerEventData eventData);
    }
}
