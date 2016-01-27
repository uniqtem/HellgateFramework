//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.IO;
using System.Linq;
#endif

namespace Hellgate
{
	public class Util
	{
		public const string ANDROID = "android";
		public const string IOS = "ios";
		public const string PC = "pc";

#if UNITY_EDITOR
		private static List<FileInfo> DirSearch(DirectoryInfo d, string searchFor)
		{
			List<FileInfo> founditems = d.GetFiles(searchFor).ToList();
			DirectoryInfo[] dis = d.GetDirectories();
			foreach (DirectoryInfo di in dis)
				founditems.AddRange(DirSearch(di, searchFor));
			
			return (founditems);
		}
		
		private static FileInfo SearchTemplateFile(string fileName)
		{
			string path = Application.dataPath;
			DirectoryInfo dir = new DirectoryInfo (path);
			List<FileInfo> lst = DirSearch (dir, fileName);
			
			if (lst.Count >= 1)
				return lst [0];
			
			return null;
		}
		
		public static string GetPathTemplateFile(string fileName)
		{
			FileInfo f = SearchTemplateFile(fileName);
			
			if (f == null)
				return null;
			
			string path = f.FullName;
			int index = path.IndexOf ("Assets");
			path = path.Substring (index);
			
			return path;
		}
#endif

		/// <summary>
		/// Finds the child object.
		/// </summary>
		/// <returns>The child object.</returns>
		/// <param name="gO">GameObject.</param>
		/// <param name="strName">String name.</param>
		public static GameObject FindChildObject (GameObject gO, string strName)
		{
			Transform[] AllData = gO.GetComponentsInChildren<Transform> (true);
			GameObject target = null;
			
			for (int i = 0; i < AllData.Length; i++) {
				if (AllData [i].name == strName) {
					target = AllData [i].gameObject;
					break;
				}
			}
			
			return target;
		}

		/// <summary>
		/// Finds the child object.
		/// </summary>
		/// <returns>The child object.</returns>
		/// <param name="gO">G o.</param>
		/// <param name="strName">String name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T FindChildObject<T> (GameObject gO, string strName)
		{
			GameObject target = FindChildObject (gO, strName);
			if (target != null) {
				return target.GetComponent<T> ();
			} else {
				return default (T);
			}
		}

		/// <summary>
		/// Gets the list object.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetListObject<T> (List<object> list)
		{
			var enumerator = list.GetEnumerator ();
			while (enumerator.MoveNext()) {
				if (enumerator.Current is T) {
					return (T)enumerator.Current;
				}
			}
			
			return default(T);
		}

		/// <summary>
		/// Get the list objects.
		/// </summary>
		/// <returns>The list objects.</returns>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> GetListObjects<T> (List<object> list)
		{
			List<T> temp = new List<T> ();
			
			var enumerator = list.GetEnumerator ();
			while (enumerator.MoveNext()) {
				if (enumerator.Current is T) {
					temp.Add ((T)enumerator.Current);
				}
			}
			
			return temp;
		}

		/// <summary>
		/// Finds the sprite.
		/// </summary>
		/// <returns>The sprite.</returns>
		/// <param name="list">List.</param>
		/// <param name="strName">String name.</param>
		public static Sprite FindSprite (List<Sprite> list, string strName)
		{
			var enumerator = list.GetEnumerator ();
			while (enumerator.MoveNext()) {
				if (enumerator.Current.name == strName) {
					return enumerator.Current;
				}
			}
			
			return null;
		}

		/// <summary>
		/// Finds the text asset.
		/// </summary>
		/// <returns>The text asset.</returns>
		/// <param name="list">List.</param>
		/// <param name="strName">String name.</param>
		public static TextAsset FindTextAsset (List<TextAsset> list, string strName)
		{
			var enumerator = list.GetEnumerator ();
			while (enumerator.MoveNext()) {
				if (enumerator.Current.name == strName) {
					return enumerator.Current;
				}
			}
			
			return null;
		}

		/// <summary>
		/// Finds the game object.
		/// </summary>
		/// <returns>The game object.</returns>
		/// <param name="list">List.</param>
		/// <param name="strName">String name.</param>
		public static GameObject FindGameObject (List<GameObject> list, string strName)
		{
			var enumerator = list.GetEnumerator ();
			while (enumerator.MoveNext()) {
				if (enumerator.Current.name == strName) {
					return enumerator.Current;
				}
			}
			
			return null;
		}

		/// <summary>
		/// Tos the boolean.
		/// </summary>
		/// <returns><c>true</c>, if boolean was toed, <c>false</c> otherwise.</returns>
		/// <param name="str">String.</param>
		public static bool ToBoolean (string str)
		{
			string cleanValue = (str ?? "").Trim ();
			if (string.Equals (cleanValue, "false", System.StringComparison.OrdinalIgnoreCase)) {
				return false;
			}
			
			return (string.Equals (cleanValue, "true", System.StringComparison.OrdinalIgnoreCase)) || (cleanValue != "0");
		}

		/// <summary>
		/// Gets the device.
		/// pc & ios & android
		/// </summary>
		/// <returns>The device.</returns>
		public static string GetDevice ()
		{
			string typeCode = "";
#if UNITY_EDITOR
			if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android) {
				typeCode = ANDROID;
			} else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS) {
				typeCode = IOS;
			} else {
				typeCode = PC;
			}

#elif UNITY_ANDROID 
			if (Application.platform == RuntimePlatform.Android) {
				typeCode = ANDROID;
			}
#elif UNITY_IOS
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				typeCode = IOS;
			}
#elif UNITY_STANDALONE
			typeCode = PC;
#endif

			return typeCode;
			
		}

		/// <summary>
		/// Gets the distinct values.
		/// </summary>
		/// <returns>The distinct values.</returns>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> GetDistinctValues<T> (List<T> list)
		{
			List<T> temp = new List<T> ();
			for (int i = 0; i < list.Count; i++) {
				if (temp.Contains (list [i])) {
					continue;
				}

				temp.Add (list [i]);
			}

			return temp;
		}

		/// <summary>
		/// Gets the distinct values.
		/// </summary>
		/// <returns>The distinct values.</returns>
		/// <param name="array">Array.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> GetDistinctValues<T> (T[] array)
		{
			List<T> temp = new List<T> ();
			for (int i = 0; i < array.Length; i++) {
				if (temp.Contains (array [i])) {
					continue;
				}
				
				temp.Add (array [i]);
			}
			
			return temp;
		}
		
		public static bool IsInteger (Type type)
		{
			return (
				type == typeof(SByte) ||
				type == typeof(Int16) ||
				type == typeof(Int32) ||
				type == typeof(Int64) ||
				type == typeof(Byte) ||
				type == typeof(UInt16) ||
				type == typeof(UInt32) ||
				type == typeof(UInt64)
				); 
		}
		
		public static bool IsFloat (Type type)
		{
			return (type == typeof(float) | type == typeof(double) | type == typeof(Decimal));
		}
		
		public static bool IsNumeric (Type type)
		{
			if (!(
				type == typeof(Byte) ||
				type == typeof(Int16) ||
				type == typeof(Int32) ||
				type == typeof(Int64) ||
				type == typeof(SByte) ||
				type == typeof(UInt16) ||
				type == typeof(UInt32) ||
				type == typeof(UInt64) ||
				type == typeof(Decimal) ||
				type == typeof(Double) ||
				type == typeof(Single)
				)) {
				return false;
			}
			else {
				return true;
			}
		}

		public static bool IsText (Type type)
		{
			return (type == typeof(String) || type == typeof(Char));
		}

		public static bool IsValueType (Type type)
		{
			return (IsText (type) || IsNumeric (type));
		}
	}
}
