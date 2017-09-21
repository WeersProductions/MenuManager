using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MCSimplePopupData
{
    private readonly string _title;
    private readonly string _description;
    private readonly UnityAction[] _buttonActions;
    private readonly string[] _buttonStrings;
    private readonly Sprite[] _buttonSprites;

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
    }

    public string Description
    {
        get { return _description; }
    }

    public UnityAction[] ButtonActions
    {
        get { return _buttonActions; }
    }

    public string[] ButtonStrings
    {
        get { return _buttonStrings; }
    }

    public Sprite[] ButtonSprites
    {
        get { return _buttonSprites; }
    }
}
