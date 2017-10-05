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

        /// <summary>
        /// If true, when creating new menus they will be instantiated in the scene.
        /// This can be usefull if your game only has a few menus and they're often used.
        /// 
        /// If you have a game with a lot of menus, you should turn this off.
        /// </summary>
        public bool SpawnInScene = true;
    }
}