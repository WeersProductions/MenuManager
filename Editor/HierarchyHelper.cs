using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WeersProductions
{
	public static class HierarchyHelper {
		public static T[] GetSelectedObjectsOfType<T>() where T : Component
		{
			Transform[] selectedTransforms = Selection.GetTransforms(~SelectionMode.Assets);
			List<T> result = new List<T>();

			for(int i = 0; i < selectedTransforms.Length; i++) {
				T component = selectedTransforms[i].GetComponent<T>();
				if(component) {
					result.Add(component);
				}
			}
			return result.ToArray();
		}
		public static T[] GetObjectsOfType<T>() where T : Component
		{
			return GameObject.FindObjectsOfType<T>();
		}

		public static T[] GetSelectedOrGeneralObjectsOfType<T>() where T : Component
		{
			T[] result = GetSelectedObjectsOfType<T>();
			if(result == null || result.Length <= 0) {
				result = GetObjectsOfType<T>();
			} 
			return result;
		}
	}
}