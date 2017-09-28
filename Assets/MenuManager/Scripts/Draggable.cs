using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WeersProductions
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private IDraggableMenu _owner;

        private void Awake()
        {
            // Find our owner.
            _owner = FindInParents<IDraggableMenu>(gameObject);
            if (_owner == null)
            {
                Debug.Log("Could not find the correct IDraggableMenu as parent.");
                Destroy(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _owner.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _owner.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _owner.OnEndDrag(eventData);
        }

        /// <summary>
        /// Find a component in one of the parents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go">Starting node.</param>
        /// <returns>Null if no component is found of type T</returns>
        public static T FindInParents<T>(GameObject go)
        {
            if (go == null) return default(T);
            var comp = go.GetComponent<T>();

            if (comp != null)
                return comp;

            Transform t = go.transform.parent;
            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent<T>();
                t = t.parent;
            }
            return comp;
        }
    }

}
