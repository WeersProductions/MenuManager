using UnityEngine;

namespace WeersProductions
{
    /// <inheritdoc />
    /// <summary>
    /// Contains all settings for in the editor and is used to make them persistant.
    /// </summary>
    public class EditorMenuCreatorSettings : ScriptableObject
    {
        public const string SettingsPath = "assets/MenuManager/Editor/Settings.asset";

        public RectTransform MenuParent;
        public string DefaultPresetPath = "/MenuManager/Presets/";
        public MenuController MenuController;
    }
}