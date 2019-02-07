using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace WeersProductions
{
	/// <summary>
	/// Contains properties that are shared along all MenuControllers.
	/// </summary>
	public class MenuControllerSharedProps : ScriptableObject {
		public const string RESOURCE_PATH =  "Assets/MenuManager/Resources";

		/// <summary>
		/// Menus that are shared along all MenuControllers.
		/// </summary>
		[SerializeField]
		private MCMenu[] _menus;

		public MCMenu[] Menus {
			get {
				return _menus;
			}
			set {
				_menus = value;
			}
		}

		public static MenuControllerSharedProps GetOrCreateInstance() {
			MenuControllerSharedProps instance = GetInstance();
			if(!instance) {
                Debug.Log(string.Format("Creating MenuControllerSharedProps in: {0}", RESOURCE_PATH));
                Directory.CreateDirectory(RESOURCE_PATH);

				string path = Path.Combine(RESOURCE_PATH, "MenuControllerSharedProps.asset");

				instance = CreateInstance<MenuControllerSharedProps>();
                AssetDatabase.CreateAsset(instance, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
			}
			return instance;
		}

		public static MenuControllerSharedProps GetInstance() {
			MenuControllerSharedProps[] allInstances = Resources.LoadAll<MenuControllerSharedProps>("");
			if(allInstances != null && allInstances.Length > 0) {
				return allInstances[0];
			}
			return null;
		}

		public static MCMenu[] GetSharedMenus() {
			MenuControllerSharedProps instance = GetInstance();
			if(instance) {
				if(instance != null) {
					return instance.Menus;
				}
			}
			return new MCMenu[0];
		}
	}
}
