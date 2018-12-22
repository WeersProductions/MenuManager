using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WeersProductions
{
	public static class HierarchyHelper {
		public static T[] GetObjectsOfType<T>() where T : Component
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
	}
}