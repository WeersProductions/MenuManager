using UnityEngine;

namespace WeersProductions
{
    public class EditorMenuCreatorSettings : ScriptableObject
    {
        public const string SettingsPath = "assets/MenuManager/Editor/Settings.asset";

        public RectTransform MenuParent;
        public string DefaultPresetPath = "/MenuManager/Presets/";
        public MenuController MenuController;
    }
}