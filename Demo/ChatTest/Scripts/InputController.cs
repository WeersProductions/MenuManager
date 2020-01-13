using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeersProductions;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private MenuController _menuController;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _menuController.ToggleMenu("CHATBOX");
        }
    }
}
