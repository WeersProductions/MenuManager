using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WeersProductions
{
    public class EditorMenuCreator : EditorWindow
    {
        #region variables

        private TabsBlock _tabsBlock;

        #region create menu

        private int _selectedPreset;
        private MenuCreatorPreset[] _presets;
        private string[] _presetTitles;
        private string[] _presetDescriptions;

        private SerializedObject _menuController;
        #endregion

        #region create preset

        private string _newFileName = "";
        private MenuCreatorPreset _currentSelectedPreset;
        private Vector2 _scrollPositionPresetList;
        private Vector2 _scrollPositionPresetDetail;
        #endregion

        #region Options

        private EditorMenuCreatorSettings _editorMenuCreatorSettings;
        #endregion

        #endregion

        [MenuItem("WeersProductions/Create menu")]
        private static void Init()
        {
            EditorMenuCreator editorMenuCreator = GetWindow<EditorMenuCreator>();
            editorMenuCreator.Show();
        }

        private void OnEnable()
        {
            // Make sure we have an object to store our settings
            CreateOrLoadSettings();



            _tabsBlock = new TabsBlock(new Dictionary<string, Action>
        {
            {"Initialize", DrawInitialize },
            {"Create menu", DrawCreateMenu },
            {"Create preset", DrawCreatePreset },
            {"Options", DrawOptions }
        });
        }

        void OnGUI()
        {
            _tabsBlock.Draw();
        }

        /// <summary>
        /// Draws the initalize panel.
        /// </summary>
        private void DrawInitialize()
        {
            bool everythingIsPerfect = true;
            if (!_editorMenuCreatorSettings.MenuController)
            {
                EditorGUILayout.HelpBox("The MenuController is not yet set, set it in the options manually or try the button below.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Search for MenuController"))
                {
                    MenuController menuController = GameObject.FindObjectOfType<MenuController>();

                    if (menuController)
                    {
                        _editorMenuCreatorSettings.MenuController = menuController;

                        EditorUtility.SetDirty(_editorMenuCreatorSettings);
                    }
                }
                if (GUILayout.Button("Add MenuController component to first canvas"))
                {
                    Canvas firstCanvas = GameObject.FindObjectOfType<Canvas>();

                    if (firstCanvas)
                    {
                        _editorMenuCreatorSettings.MenuController = firstCanvas.gameObject.AddComponent<MenuController>();

                        EditorUtility.SetDirty(_editorMenuCreatorSettings);
                    }
                }
                EditorGUILayout.EndHorizontal();

                everythingIsPerfect = false;
            }
            if (!_editorMenuCreatorSettings.SpawnInScene && !_editorMenuCreatorSettings.MenuParent)
            {
                EditorGUILayout.HelpBox("The parent for the menus is not yet set, set it in the options manually or try the button below.", MessageType.Warning);
                if (GUILayout.Button("Use first Canvas that's found"))
                {
                    Canvas menuParent = GameObject.FindObjectOfType<Canvas>();

                    if (menuParent)
                    {
                        _editorMenuCreatorSettings.MenuParent = menuParent.GetComponent<RectTransform>();

                        EditorUtility.SetDirty(_editorMenuCreatorSettings);
                    }
                }
                everythingIsPerfect = false;
            }

            if (everythingIsPerfect)
            {
                EditorGUILayout.HelpBox("Seems like everything is fine! Enjoy ;)", MessageType.Info);
            }
        }

        /// <summary>
        /// Draws the create menu panel.
        /// </summary>
        private void DrawCreateMenu()
        {
            if (!CheckPresets())
            {
                EditorGUILayout.HelpBox("Could not find any presets, are you sure you have any scripts of type MenuCreatorPreset?", MessageType.Error);
                return;
            }

            _selectedPreset = EditorGUILayout.Popup(_selectedPreset, _presetTitles);

            if (_selectedPreset < 0)
            {
                EditorGUILayout.HelpBox("Select a preset to continue", MessageType.Error);
                return;
            }

            EditorGUILayout.HelpBox(_presetDescriptions[_selectedPreset], MessageType.Info);

            bool hasPresetObject = _presets[_selectedPreset].PresetObject;
            if (!hasPresetObject)
            {
                EditorGUILayout.HelpBox("This preset doesn't seem to have a Preset Object!", MessageType.Warning);
            }

            using (new EditorGUI.DisabledScope(!hasPresetObject))
            {
                if (GUILayout.Button("Create menu"))
                {
                    CreateMenu(_presets[_selectedPreset]);
                }
            }
        }

        /// <summary>
        /// Called when the user wants to create a new menu.
        /// </summary>
        /// <param name="menuCreatorPreset"></param>
        private void CreateMenu(MenuCreatorPreset menuCreatorPreset)
        {
            GameObject newMenu;
            // Only instantiate an object if the user wants that according to the settings.
            if (_editorMenuCreatorSettings.SpawnInScene)
            {
                newMenu = Instantiate(menuCreatorPreset.PresetObject, _editorMenuCreatorSettings.MenuParent);
            }
            else
            {
                newMenu = menuCreatorPreset.PresetObject;
            }

            MCMenu mcMenu = newMenu.GetComponentInChildren<MCMenu>();
            if (mcMenu)
            {
                AddMenuToController(mcMenu);
            }
        }

        /// <summary>
        /// Adds a new menu to the array of menus in the menucontroller.
        /// </summary>
        /// <param name="mcMenu"></param>
        private void AddMenuToController(MCMenu mcMenu)
        {
            if (_editorMenuCreatorSettings.MenuController)
            {
                if (_menuController == null)
                {
                    _menuController = new SerializedObject(_editorMenuCreatorSettings.MenuController);
                }
                _menuController.Update();
                SerializedProperty menuArray = _menuController.FindProperty("_mcMenus");
                menuArray.arraySize += 1;
                menuArray.GetArrayElementAtIndex(menuArray.arraySize - 1).objectReferenceValue = mcMenu;
                _menuController.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Draw the create preset panel.
        /// </summary>
        private void DrawCreatePreset()
        {
            bool hasPresets = CheckPresets();
            if (!hasPresets)
            {
                EditorGUILayout.HelpBox("You don't seem to have any presets, start by creating one!", MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();

            _newFileName = GUILayout.TextField(_newFileName, 20);

            if (GUILayout.Button("Create new"))
            {
                CreatePreset(_newFileName);
                UpdatePresetsList();
            }

            EditorGUILayout.EndHorizontal();

            if (!hasPresets)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Label("Preset list", EditorStyles.centeredGreyMiniLabel);

            _scrollPositionPresetList = EditorGUILayout.BeginScrollView(_scrollPositionPresetList);
            for (int i = 0; i < _presets.Length; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(_presets[i].Title))
                {
                    _currentSelectedPreset = _presets[i];
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Label("Preset detail", EditorStyles.centeredGreyMiniLabel);

            if (_currentSelectedPreset != null)
            {
                _scrollPositionPresetDetail = EditorGUILayout.BeginScrollView(_scrollPositionPresetDetail);
                Editor editor = Editor.CreateEditor(_currentSelectedPreset);
                editor.DrawDefaultInspector();

                EditorGUILayout.Space();

                if (_currentSelectedPreset.PresetObject)
                {
                    MCMenu mcMenu = _currentSelectedPreset.PresetObject.GetComponentInChildren<MCMenu>();
                    if (mcMenu)
                    {
                        Editor mcEditor = Editor.CreateEditor(mcMenu);
                        mcEditor.DrawDefaultInspector();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// Create a new preset.
        /// </summary>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        private MenuCreatorPreset CreatePreset(string newFileName)
        {
            // TODO: check if this file already exists.
            if (string.IsNullOrEmpty(newFileName))
            {
                CreatePreset("newPreset");
            }
            string path = "assets" + _editorMenuCreatorSettings.DefaultPresetPath + newFileName + ".asset";
            if (AssetDatabase.LoadAssetAtPath(path, typeof(MenuCreatorPreset)))
            {
                CreatePreset(newFileName + "_");
            }

            MenuCreatorPreset menuCreatorPreset = CreateInstance<MenuCreatorPreset>();
            AssetDatabase.CreateAsset(menuCreatorPreset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return menuCreatorPreset;
        }

        /// <summary>
        /// Draws the options panel.
        /// </summary>
        private void DrawOptions()
        {
            _editorMenuCreatorSettings.MenuParent = (RectTransform)EditorGUILayout.ObjectField("Parent of new menus", _editorMenuCreatorSettings.MenuParent, typeof(RectTransform), true);
            _editorMenuCreatorSettings.DefaultPresetPath = EditorGUILayout.TextField("Preset location", _editorMenuCreatorSettings.DefaultPresetPath);
            _editorMenuCreatorSettings.MenuController = (MenuController)EditorGUILayout.ObjectField("Menu Controller",
                _editorMenuCreatorSettings.MenuController, typeof(MenuController), true);
            _editorMenuCreatorSettings.SpawnInScene =
                EditorGUILayout.Toggle("Spawn in scene", _editorMenuCreatorSettings.SpawnInScene);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_editorMenuCreatorSettings);
            }
        }

        /// <summary>
        /// Loads all presets of menus.
        /// </summary>
        /// <returns>True if it found any presets, false if not.</returns>
        private bool CheckPresets()
        {
            if (_presets == null)
            {
                List<MenuCreatorPreset> result = new List<MenuCreatorPreset>();

                string[] aMaterialFiles = Directory.GetFiles(Application.dataPath + _editorMenuCreatorSettings.DefaultPresetPath, "*.asset", SearchOption.AllDirectories);
                foreach (string matFile in aMaterialFiles)
                {
                    string assetPath = "Assets" + matFile.Replace(Application.dataPath, "").Replace('\\', '/');
                    MenuCreatorPreset menuCreatorPreset = AssetDatabase.LoadAssetAtPath<MenuCreatorPreset>(assetPath);
                    result.Add(menuCreatorPreset);
                }

                _presets = result.ToArray();
            }

            // Update tooltips and titles.
            _presetTitles = new string[_presets.Length];
            _presetDescriptions = new string[_presets.Length];
            for (int i = 0; i < _presets.Length; i++)
            {
                _presetTitles[i] = _presets[i].Title;
                _presetDescriptions[i] = _presets[i].Description;
            }

            return _presets != null && _presets.Length > 0;
        }

        /// <summary>
        /// Will force a rebuild of the preset list.
        /// </summary>
        private void UpdatePresetsList()
        {
            _presets = null;
            CheckPresets();
        }

        /// <summary>
        /// If no settings file exists, it will create one, otherwise it will use the existing one.
        /// </summary>
        private void CreateOrLoadSettings()
        {
            EditorMenuCreatorSettings settingsAsset = (EditorMenuCreatorSettings)AssetDatabase.LoadAssetAtPath(EditorMenuCreatorSettings.SettingsPath, typeof(EditorMenuCreatorSettings));
            if (!settingsAsset)
            {
                settingsAsset = CreateInstance<EditorMenuCreatorSettings>();
                AssetDatabase.CreateAsset(settingsAsset, EditorMenuCreatorSettings.SettingsPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            _editorMenuCreatorSettings = settingsAsset;

            if (!_editorMenuCreatorSettings.MenuController)
            {
                MenuController menuController = FindObjectOfType<MenuController>();
                _editorMenuCreatorSettings.MenuController = menuController;
                EditorUtility.SetDirty(_editorMenuCreatorSettings);
            }
        }
    }
}
