using System.IO;
using UnityEditor;
using UnityEngine;

namespace WeersProductions
{
    /// <inheritdoc />
    /// <summary>
    /// Contains all settings for in the editor and is used to make them persistant.
    /// </summary>
    public class EditorMenuCreatorSettings : ScriptableObject
    {
        public static string SettingsPathWithAssets
        {
            get { return Path.Combine("assets/", SettingsPath); }
        }

        public static string SettingsPath
        {
            get
            {
                if (!EditorPrefs.HasKey("MenuManager_SettingsPath"))
                {
                    return "MenuManager/Editor/Settings.asset";
                }
                return EditorPrefs.GetString("MenuManager_SettingsPath");
            }
            set
            {
                EditorPrefs.SetString("MenuManager_SettingsPath", value);
            }
        }

        public static string SettingsPathFull
        {
            get { return Path.Combine(Application.dataPath, SettingsPath); }
        }

        public static string SettingsPathDirectories
        {
            get { return Path.GetDirectoryName(SettingsPathFull); }
        }

        public RectTransform MenuParent;
        public static string DefaultPresetPathFull
        {
            get { return Path.Combine(SettingsPathDirectories, "Presets"); }
        }

        public MenuController MenuController;
    }
}