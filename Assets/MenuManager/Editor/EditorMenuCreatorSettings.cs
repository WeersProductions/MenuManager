using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        /// <summary>
        /// Returns the settings object.
        /// If no settings object can be found, a new one is made.
        /// </summary>
        /// <returns></returns>
        public static EditorMenuCreatorSettings GetEditorMenuCreatorSettings()
        {
            EditorMenuCreatorSettings[] findObjectsOfTypeAll = Resources.LoadAll<EditorMenuCreatorSettings>("");
            if (findObjectsOfTypeAll == null || findObjectsOfTypeAll.Length <= 0)
            {
                Debug.LogError("You seem to have deleted the <EditorMenuCreatorSettings> file from the Resources folder. A new one will be generated, you can move this one around as long as its in a <Resources> folder.");

                Directory.CreateDirectory(MenuControllerSharedProps.RESOURCE_PATH);

                string path = Path.Combine(MenuControllerSharedProps.RESOURCE_PATH, "EditorMenuCreatorSettings.asset");

                Debug.Log(string.Format("Creating settingsobject in: {0}", path));

                EditorMenuCreatorSettings settingsAsset = CreateInstance<EditorMenuCreatorSettings>();
                AssetDatabase.CreateAsset(settingsAsset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return settingsAsset;
            }
            if (findObjectsOfTypeAll.Length > 1)
            {
                Debug.LogError("You seem to have multiple <EditorMenuCreatorSettings> files in your Resources folder. The first one found is used, please delete the others.");
                for (int i = 0; i < findObjectsOfTypeAll.Length; i++)
                {
                    Debug.Log("Found one at: " + AssetDatabase.GetAssetPath(findObjectsOfTypeAll[i]));
                }
            }
            return findObjectsOfTypeAll[0];
        }

        public static MenuCreatorPreset[] GetAllPresets()
        {
            EditorMenuCreatorSettings settings = GetEditorMenuCreatorSettings();
            if (settings.HasCustomPresetFolder)
            {
                List<MenuCreatorPreset> result = new List<MenuCreatorPreset>();

                string[] aMaterialFiles = Directory.GetFiles(settings.CustomPresetFolderFull, "*.asset", SearchOption.AllDirectories);
                foreach (string matFile in aMaterialFiles)
                {
                    string assetPath = string.Format("Assets{0}", matFile.Replace(Application.dataPath, "").Replace('\\', '/'));
                    MenuCreatorPreset menuCreatorPreset = AssetDatabase.LoadAssetAtPath<MenuCreatorPreset>(assetPath);
                    result.Add(menuCreatorPreset);
                }
                return result.ToArray();
            }
            else
            {
                return Resources.LoadAll<MenuCreatorPreset>("");
            }
        }

        public static string GetPresetsLocation()
        {
            EditorMenuCreatorSettings settings = GetEditorMenuCreatorSettings();
            string result;
            if (settings.HasCustomPresetFolder)
            {
                result = settings.CustomPresetFolderFull;
            }
            else
            {
                MenuCreatorPreset[] menuCreatorPresets = Resources.LoadAll<MenuCreatorPreset>("");
                if (menuCreatorPresets != null && menuCreatorPresets.Length >= 0)
                {
                    string assetPath = AssetDatabase.GetAssetPath(menuCreatorPresets[0]);
                    // Remove the Assets/ part. This is also included in the dataPath.
                    string subString = assetPath.Substring(7);
                    result = Path.GetDirectoryName(Path.Combine(Application.dataPath, subString));
                }
                else
                {
                    // We don't have any presets and no customPresetFolder is set. Return the default relative folder.
                    result = Path.Combine(MenuControllerSharedProps.RESOURCE_PATH, "Presets");
                    // TODO: check if we should use 'result' here in CreateDirectory or not
                    Directory.CreateDirectory(MenuControllerSharedProps.RESOURCE_PATH);
                }
            }
            return string.Format("Assets{0}", result.Replace(Application.dataPath, "").Replace('\\', '/'));
        }

        public MenuController MenuController;

        /// <summary>
        /// If true, a custom folder is used to keep track of all Presets.
        /// </summary>
        public bool HasCustomPresetFolder;

        /// <summary>
        /// If <see cref="HasCustomPresetFolder"/> is true, this path is used to check all presets.
        /// </summary>
        public string CustomPresetFolder;

        public string CustomPresetFolderFull
        {
            get { return Path.Combine(Application.dataPath, CustomPresetFolder); }
        }
    }
}