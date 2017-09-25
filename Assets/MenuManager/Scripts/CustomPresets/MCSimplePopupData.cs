using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MCSimplePopupData
{
    private string _title;
    private string _description;
    private UnityAction[] _buttonActions;
    private string[] _buttonStrings;
    private Sprite[] _buttonSprites;

    public MCSimplePopupData(string title, string description)
    {
        _title = title;
        _description = description;
    }

    public MCSimplePopupData(string title, string description, UnityAction[] buttonActions, string[] buttonStrings) : this(title, description)
    {
        _buttonActions = buttonActions;
        _buttonStrings = buttonStrings;
    }

    public MCSimplePopupData(string title, string description, UnityAction[] buttonActions, Sprite[] buttonSprites) : this(
        title, description)
    {
        _buttonActions = buttonActions;
        _buttonSprites = buttonSprites;
    }


    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public UnityAction[] ButtonActions
    {
        get { return _buttonActions; }
        set { _buttonActions = value; }
    }

    public string[] ButtonStrings
    {
        get { return _buttonStrings; }
        set { _buttonStrings = value; }
    }

    public Sprite[] ButtonSprites
    {
        get { return _buttonSprites; }
        set { _buttonSprites = value; }
    }
}
