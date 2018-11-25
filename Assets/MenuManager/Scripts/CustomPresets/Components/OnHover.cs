using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace WeersProductions
{
	public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		public delegate void OnPointerEvent(PointerEventData eventData);

		public OnPointerEvent onPointerEnter;
		public OnPointerEvent onPointerExit;
		/// <summary>
		/// Called after <see Delay/> time is over and the mouse is still hovering on the object.
		/// </summary>
		public UnityAction onPointerDelay;

		/// <summary>
		/// If bigger than 0, onPointerDelay will be called after x seconds if the mouse is still on the object. 
		/// Can be used to show a tooltip after some time of hovering.
		/// </summary>
		public float Delay;

		/// <summary>
		/// If true, the mouse is inside this object.
		/// </summary>
		private bool _inside;

		/// <summary>
		/// Called when the pointer enters this object.
		/// If <see onPointerEnter/> has been set, it will be called.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			_inside = true;
			if (onPointerEnter != null)
			{
				onPointerEnter(eventData);
			}
			if (Delay > 0 && onPointerDelay != null) 
			{
				StartCoroutine(InvokeAfter(Delay));
			}
		}

		IEnumerator InvokeAfter(float seconds)
		{
			yield return new WaitForSecondsRealtime(seconds);
			if (_inside)
			{
				onPointerDelay();
			}
		}

		/// <summary>
		/// Called when the pointer leaves this object.
		/// If <see onPointerLeave/> has been set, it will be called.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerExit(PointerEventData eventData)
		{
			_inside = false;
			if (onPointerExit != null)
			{
				onPointerExit(eventData);
			}
		}
	}
}

