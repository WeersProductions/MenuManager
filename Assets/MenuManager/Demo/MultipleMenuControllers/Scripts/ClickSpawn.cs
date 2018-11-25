using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSpawn : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField]
	private GameObject _prefab;

    public void OnPointerClick(PointerEventData eventData)
    {
		Instantiate(_prefab, eventData.pointerCurrentRaycast.worldPosition, Quaternion.identity);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
