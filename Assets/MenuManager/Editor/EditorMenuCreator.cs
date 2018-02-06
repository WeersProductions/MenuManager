using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Vuforia.EditorClasses;
using Object = UnityEngine.Object;

namespace WeersProductions
{
    public class EditorMenuCreator : EditorWindow
    {
        /// <summary>
        /// Used to check what the user is currently setting in the editor for the set-up.
        /// </summary>
        private enum InitializeState
        {
            NONE,
            EDITOR_SETTINGS_DIRECTORY
        }

        #region variables

        private TabsBlock _tabsBlock;

        private InitializeState _initializeState = InitializeState.NONE;

        #region create menu

        private int _selectedPreset;
        private MenuCreatorPreset[] _presets;
        private string[] _presetTitles;
        private string[] _presetDescriptions;

        private SerializedObject _menuController;
        private bool _showMenuList;
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

        [MenuItem("GameObject/WeersProductions/Add menu", false, 0)]
        private static void AddMenu()
        {
            EditorMenuCreatorSettings settingsAsset = (EditorMenuCreatorSettings)AssetDatabase.LoadAssetAtPath(EditorMenuCreatorSettings.SettingsPathWithAssets, typeof(EditorMenuCreatorSettings));
            Transform parent = null;
            if (settingsAsset && settingsAsset.MenuParent)
            {
                parent = settingsAsset.MenuParent;
            }
            else
            {
                Canvas menuParent = GameObject.FindObjectOfType<Canvas>();
                if (menuParent)
                {
                    parent = menuParent.GetComponent<Transform>();
                }
            }

            if (!parent)
            {
                Debug.LogError("Please add a Canvas to the scene.");
                return;
            }

            GameObject newMenu = new GameObject("New menu");
            Undo.RegisterCreatedObjectUndo(newMenu, "Create menu");
            Undo.AddComponent<MCMenu>(newMenu);

            if (parent)
            {
                Undo.SetTransformParent(newMenu.GetComponent<Transform>(), parent, "Create menu");
                EditorUtils.Collapse(parent.gameObject, true);
            }
        }

        [MenuItem("Window/WeersProductions/MenuManager")]
        private static void Init()
        {
            EditorMenuCreator editorMenuCreator = GetWindow<EditorMenuCreator>(false, "Menu Manager");
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
            CheckPaths();
            if (_initializeState != InitializeState.NONE)
            {
                DrawFixPaths();
                return;
            }

            _tabsBlock.Draw();
        }

        /// <summary>
        /// Checks whether the user has correctly set up the environment and all directories are correctly created.
        /// </summary>
        private void CheckPaths()
        {
            if (_initializeState != InitializeState.NONE)
            {
                return;
            }
            bool settingsExists = Directory.Exists(EditorMenuCreatorSettings.SettingsPathDirectories);
            if (!settingsExists)
            {
                _initializeState = InitializeState.EDITOR_SETTINGS_DIRECTORY;
                return;
            }

            Directory.CreateDirectory(EditorMenuCreatorSettings.DefaultPresetPathFull);
        }

        /// <summary>
        /// The screen where the user can set the paths for both the settings and presets. Will be shown if some directories don't exist in the user's project.
        /// </summary>
        private void DrawFixPaths()
        {
            GUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("Set-up", EditorStyles.centeredGreyMiniLabel);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (_initializeState == InitializeState.EDITOR_SETTINGS_DIRECTORY)
            {
                EditorGUILayout.HelpBox("The MenuManager needs a file to save its settings, the directory does not seem to exist that is required.", MessageType.Warning);
                EditorMenuCreatorSettings.SettingsPath = EditorGUILayout.TextField("Settings path", EditorMenuCreatorSettings.SettingsPath);
                EditorGUILayout.LabelField("Full path: " + EditorMenuCreatorSettings.SettingsPathDirectories);
                if (GUILayout.Button("Confirm") && !string.IsNullOrEmpty(EditorMenuCreatorSettings.SettingsPath))
                {
                    Directory.CreateDirectory(EditorMenuCreatorSettings.SettingsPathDirectories);
                    _initializeState = InitializeState.NONE;
                }
                return;
            }
        }

        /// <summary>
        /// Draws the initalize panel.
        /// </summary>
        private void DrawInitialize()
        {
            EnsureSettingsObject();
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
                        SetMenuController(menuController);
                    }
                }
                if (GUILayout.Button("Add MenuController component to first canvas"))
                {
                    Canvas firstCanvas = GameObject.FindObjectOfType<Canvas>();

                    if (firstCanvas)
                    {
                        SetMenuController(firstCanvas.gameObject.AddComponent<MenuController>());
                    }
                }
                EditorGUILayout.EndHorizontal();

                everythingIsPerfect = false;
            }
            if (!_editorMenuCreatorSettings.MenuParent)
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

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (_menuController == null)
            {
                EnsureSettingsObject();
                _menuController = new SerializedObject(_editorMenuCreatorSettings.MenuController);
            }
            EditorGUILayout.LabelField("Add existing menus");

            DragDrop.DrawDragDrop("Drag menus to add them", objects =>
            {
                foreach (Object dropObject in objects)
                {
                    GameObject gameObject = dropObject as GameObject;
                    if (gameObject)
                    {
                        MCMenu mcMenu = gameObject.GetComponentInChildren<MCMenu>();
                        AddMenuToController(mcMenu);
                    }
                }
            });

            _showMenuList = EditorGUILayout.Foldout(_showMenuList, "Current menus");
            if (_showMenuList)
            {
                _menuController.Update();
                SerializedProperty arrayProperty = _menuController.FindProperty("_mcMenus");
                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(arrayProperty.GetArrayElementAtIndex(i));
                    if (GUILayout.Button("Remove"))
                    {
                        RemoveMenuFromController(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                _menuController.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Called when the user wants to create a new menu.
        /// </summary>
        /// <param name="menuCreatorPreset"></param>
        private void CreateMenu(MenuCreatorPreset menuCreatorPreset)
        {
            GameObject newMenu = Instantiate(menuCreatorPreset.PresetObject, _editorMenuCreatorSettings.MenuParent);
            Undo.RegisterCreatedObjectUndo(newMenu, "Created menu");

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
        /// Remove a specific MCMenu from the list of menus from the current selected MenuController.
        /// </summary>
        /// <param name="index">Of the list</param>
        private void RemoveMenuFromController(int index)
        {
            if (_editorMenuCreatorSettings.MenuController)
            {
                if (_menuController == null)
                {
                    _menuController = new SerializedObject(_editorMenuCreatorSettings.MenuController);
                }
                _menuController.Update();
                SerializedProperty menuArray = _menuController.FindProperty("_mcMenus");
                int arraySize = menuArray.arraySize;
                menuArray.DeleteArrayElementAtIndex(index);
                if (menuArray.arraySize == arraySize)
                {
                    // Hotfix, because of: https://answers.unity.com/questions/555724/serializedpropertydeletearrayelementatindex-leaves.html 
                    menuArray.DeleteArrayElementAtIndex(index);
                }
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
            string path = string.Format("assets{0}{1}.asset", EditorMenuCreatorSettings.DefaultPresetPathFull, newFileName);
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
            _editorMenuCreatorSettings.MenuController = (MenuController)EditorGUILayout.ObjectField("Menu Controller",
                _editorMenuCreatorSettings.MenuController, typeof(MenuController), true);

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

                string[] aMaterialFiles = Directory.GetFiles(EditorMenuCreatorSettings.DefaultPresetPathFull, "*.asset", SearchOption.AllDirectories);
                foreach (string matFile in aMaterialFiles)
                {
                    string assetPath = string.Format("Assets{0}", matFile.Replace(Application.dataPath, "").Replace('\\', '/'));
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
            CheckPaths();
            if (_initializeState != InitializeState.NONE)
            {
                return;
            }
            EnsureSettingsObject();

            if (!_editorMenuCreatorSettings.MenuController)
            {
                MenuController menuController = FindObjectOfType<MenuController>();
                SetMenuController(menuController);
            }

            UpdatePresetsList();
        }

        /// <summary>
        /// After calling this, you can be sure that <see cref="_editorMenuCreatorSettings"/> has a value.
        /// </summary>
        private void EnsureSettingsObject()
        {
            if (_editorMenuCreatorSettings)
            {
                return;
            }
            EditorMenuCreatorSettings settingsAsset = (EditorMenuCreatorSettings)AssetDatabase.LoadAssetAtPath(EditorMenuCreatorSettings.SettingsPathWithAssets, typeof(EditorMenuCreatorSettings));
            if (!settingsAsset)
            {
                settingsAsset = CreateInstance<EditorMenuCreatorSettings>();
                AssetDatabase.CreateAsset(settingsAsset, EditorMenuCreatorSettings.SettingsPathWithAssets);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            _editorMenuCreatorSettings = settingsAsset;
        }

        /// <summary>
        /// Set the menuController in the options. Will also save it.
        /// </summary>
        /// <param name="menuController"></param>
        private void SetMenuController(MenuController menuController)
        {
            _editorMenuCreatorSettings.MenuController = menuController;
            EditorUtility.SetDirty(_editorMenuCreatorSettings);
        }
    }
}
