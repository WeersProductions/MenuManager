using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A simple script that ius used to listen to click events on an object and spawn a prefab.
/// Used in the demo to spawn menus in the world.
/// </summary>
public class ClickSpawn : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// The prefab that should be spawned.
    /// </summary>
    [SerializeField]
    private GameObject _prefab;

    /// <summary>
    /// Instantiate the prefab whenever a click has been made.
    /// </summary>
    /// <param name="eventData"></param>
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
