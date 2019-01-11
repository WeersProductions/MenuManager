using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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

        private int _selectedMenuController = -1;
        private MenuController[] _availableMenuControllers;
        private string[] _availableMenuControllersLabels;

        private EditorMenuCreatorSettings _editorMenuCreatorSettings;

        private MenuControllerSharedProps _menuControllerSharedProps;
        private SerializedObject _menuControllerSharedPropsObject;
        #endregion

        #endregion

        [MenuItem("Window/WeersProductions/MenuManager")]
        private static void Init()
        {
            EditorMenuCreator editorMenuCreator = GetWindow<EditorMenuCreator>(false, "Menu Manager");
            editorMenuCreator.CreateOrLoadSettings();
            editorMenuCreator.Show();
        }

        private void OnEnable()
        {
            _tabsBlock = new TabsBlock(new Dictionary<string, Action>
            {
                {"Manage menus", DrawManageMenus },
                {"Create menu", DrawCreateMenu },
                {"Create preset", DrawCreatePreset }
            });
            UpdateAvailableMenuControllers();
        }

        private void OnFocus()
        {
            CreateOrLoadSettings();
        }

        private void OnHierarchyChange() {
            UpdateAvailableMenuControllers();
            Repaint();
        }

        void OnGUI()
        {
            EnsureSettingsObject();
            DrawSelectMenuController();
            _tabsBlock.Draw();
        }

        private void UpdateAvailableMenuControllers()
        {
            EnsureSettingsObject();
            // TODO: support MenuControllers in prefabs?
            _availableMenuControllers = HierarchyHelper.GetObjectsOfType<MenuController>();
            int labelSize = _availableMenuControllers.Length;
            _availableMenuControllersLabels = new string[labelSize];
            for(int i = 0; i < _availableMenuControllers.Length; i++) {
                _availableMenuControllersLabels[i] = _availableMenuControllers[i].name;
                if (_editorMenuCreatorSettings.MenuController == _availableMenuControllers[i]) {
                    _selectedMenuController = i;
                }
            }
            if(_selectedMenuController >= _availableMenuControllers.Length) {
                _selectedMenuController = -1;
            }
        }

        private void DrawSelectMenuController()
        {
            EditorGUILayout.Space();
            _selectedMenuController = EditorGUILayout.Popup("Current MenuController: ", _selectedMenuController, _availableMenuControllersLabels);
            if(_selectedMenuController >= 0 && _selectedMenuController < _availableMenuControllers.Length) {
                _editorMenuCreatorSettings.MenuController = _availableMenuControllers[_selectedMenuController];
                if(_editorMenuCreatorSettings.MenuController) {
                    _menuController = new SerializedObject(_editorMenuCreatorSettings.MenuController);
                }
                EditorUtility.SetDirty(_editorMenuCreatorSettings);
            } else {
                _editorMenuCreatorSettings.MenuController = null;
                _menuController = null;
            }
            EditorGUILayout.Space();
            if(_availableMenuControllers == null || _availableMenuControllers.Length <= 0) {
                EditorGUILayout.HelpBox("No MenuControllers in this scene.", MessageType.Warning);
                if(GUILayout.Button("Create MenuController")) {
                    EditorApplication.ExecuteMenuItem("GameObject/WeersProductions/MenuController");
                }
                EditorGUILayout.Space();
            } else if(_selectedMenuController < 0) {
                EditorGUILayout.HelpBox("Select a MenuController to start.", MessageType.Warning);
                EditorGUILayout.Space();
            }
        }

        private void DrawManageMenus() 
        {
            bool hasMenuControllerSelected = _editorMenuCreatorSettings.MenuController;
            if (!hasMenuControllerSelected)
            {
                EditorGUILayout.HelpBox("Select a MenuController to change specific menus.", MessageType.Info);
            }
            
            if(hasMenuControllerSelected) {
                EditorGUILayout.LabelField("Specific menus", EditorStyles.boldLabel);
                EditorGUILayout.Space();

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
                EditorGUILayout.Space();
                _menuController.ApplyModifiedProperties();

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
                }, 40);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Shared menus", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            if(GUILayout.Button("", GUI.skin.GetStyle("IN ObjectField")))
            {
                EditorGUIUtility.PingObject(_menuControllerSharedProps);
            }
            EditorGUILayout.EndHorizontal();

            EnsureSharedProps();

            _menuControllerSharedPropsObject.Update();
            SerializedProperty sharedMenusProperty = _menuControllerSharedPropsObject.FindProperty("_menus");
            for (int i = 0; i < sharedMenusProperty.arraySize; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(sharedMenusProperty.GetArrayElementAtIndex(i));
                if(GUILayout.Button("Remove"))
                {
                    RemoveSharedMenuFromController(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            _menuControllerSharedPropsObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            DragDrop.DrawDragDrop("Drag menus to add them", objects =>
            {
                foreach (Object dropObject in objects)
                {
                    GameObject gameObject = dropObject as GameObject;
                    if (gameObject)
                    {
                        MCMenu mcMenu = gameObject.GetComponentInChildren<MCMenu>();
                        AddSharedMenuToController(mcMenu);
                    }
                }
            }, 40);
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
            GameObject newMenu = Instantiate(menuCreatorPreset.PresetObject, _editorMenuCreatorSettings.MenuController.transform);
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
            if(!mcMenu) {
                return;
            }
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

        private void AddSharedMenuToController(MCMenu mcMenu) {
            if(!mcMenu) {
                return;
            }

            SerializedProperty sharedProps = _menuControllerSharedPropsObject.FindProperty("_menus");
            sharedProps.arraySize += 1;
            sharedProps.GetArrayElementAtIndex(sharedProps.arraySize - 1).objectReferenceValue = mcMenu;
            _menuControllerSharedPropsObject.ApplyModifiedProperties();
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

        private void RemoveSharedMenuFromController(int index)
        {
            SerializedProperty menuArray = _menuControllerSharedPropsObject.FindProperty("_menus");
            int arraySize = menuArray.arraySize;
            menuArray.DeleteArrayElementAtIndex(index);
            if (menuArray.arraySize == arraySize)
            {
                // Hotfix, because of: https://answers.unity.com/questions/555724/serializedpropertydeletearrayelementatindex-leaves.html 
                menuArray.DeleteArrayElementAtIndex(index);
            }
            _menuControllerSharedPropsObject.ApplyModifiedProperties();
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
            if (string.IsNullOrEmpty(newFileName))
            {
                CreatePreset("newPreset");
            }
            string path = string.Format("{0}/{1}.asset", EditorMenuCreatorSettings.GetPresetsLocation(), newFileName);
            Debug.Log("Creating at: " + path);
            if (AssetDatabase.LoadAssetAtPath(path, typeof(MenuCreatorPreset)))
            {
                return CreatePreset(newFileName + "_");
            }

            MenuCreatorPreset menuCreatorPreset = CreateInstance<MenuCreatorPreset>();
            menuCreatorPreset.Title = newFileName;
            AssetDatabase.CreateAsset(menuCreatorPreset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return menuCreatorPreset;
        }

        /// <summary>
        /// Loads all presets of menus.
        /// </summary>
        /// <returns>True if it found any presets, false if not.</returns>
        private bool CheckPresets()
        {
            if (_presets == null)
            {
                _presets = EditorMenuCreatorSettings.GetAllPresets();
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
        /// TODO: use OnWillDeleteAsset to ensure it gets recreated if the user deletes it manually. (or AssetPostprocessor.OnPostprocessAllAssets)
        /// If no settings file exists, it will create one, otherwise it will use the existing one.
        /// </summary>
        private void CreateOrLoadSettings()
        {
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
            _editorMenuCreatorSettings = EditorMenuCreatorSettings.GetEditorMenuCreatorSettings();
        }

        private void EnsureSharedProps() 
        {
            if(_menuControllerSharedProps)
            {
                if(_menuControllerSharedPropsObject == null) 
                {
                    _menuControllerSharedPropsObject = new SerializedObject(_menuControllerSharedProps);
                }
                return;
            }
            _menuControllerSharedProps = MenuControllerSharedProps.GetOrCreateInstance();
            _menuControllerSharedPropsObject = new SerializedObject(_menuControllerSharedProps);
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
