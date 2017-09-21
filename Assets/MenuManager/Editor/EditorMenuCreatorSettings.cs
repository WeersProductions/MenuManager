using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMenuCreatorSettings : ScriptableObject
{
    public const string SettingsPath = "assets/MenuManager/Editor/Settings.asset";

    public RectTransform MenuParent;
    public string DefaultPresetPath = "/MenuManager/Presets/";
    public MenuController MenuController;
}
