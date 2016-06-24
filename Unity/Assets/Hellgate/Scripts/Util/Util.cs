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
        private static List<FileInfo> DirSearch (DirectoryInfo d, string searchFor)
        {
            List<FileInfo> founditems = d.GetFiles (searchFor).ToList ();
            DirectoryInfo[] dis = d.GetDirectories ();
            foreach (DirectoryInfo di in dis)
                founditems.AddRange (DirSearch (di, searchFor));

            return (founditems);
        }

        private static FileInfo SearchTemplateFile (string fileName)
        {
            string path = Application.dataPath;
            DirectoryInfo dir = new DirectoryInfo (path);
            List<FileInfo> lst = DirSearch (dir, fileName);

            if (lst.Count >= 1)
                return lst [0];

            return null;
        }

        public static string GetPathTemplateFile (string fileName)
        {
            FileInfo f = SearchTemplateFile (fileName);

            if (f == null)
                return null;

            string path = f.FullName;
            int index = path.IndexOf ("Assets");
            path = path.Substring (index);

            return path;
        }
#endif

        /// <summary>
        /// Gets the child object.
        /// </summary>
        /// <returns>The child object.</returns>
        /// <param name="GameObject">gameObject.</param>
        /// <param name="strName">String name.</param>
        public static GameObject GetChildObject (GameObject gObj, string strName)
        {
            Transform[] AllData = gObj.GetComponentsInChildren<Transform> (true);
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
        /// <param name="gO">GameObject.</param>
        /// <param name="strName">String name.</param>
        public static GameObject FindChildObject (GameObject gObj, string strName)
        {
            Transform[] AllData = gObj.GetComponentsInChildren<Transform> (true);
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
        public static T FindChildObject<T> (GameObject gObj, string strName)
        {
            GameObject target = FindChildObject (gObj, strName);
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
            while (enumerator.MoveNext ()) {
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
            while (enumerator.MoveNext ()) {
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
            while (enumerator.MoveNext ()) {
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
            while (enumerator.MoveNext ()) {
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
            while (enumerator.MoveNext ()) {
                if (enumerator.Current.name == strName) {
                    return enumerator.Current;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the Object.
        /// </summary>
        /// <returns>The Object.</returns>
        /// <param name="list">List.</param>
        /// <param name="strName">String name.</param>
        public static UnityEngine.Object FindObject (List<UnityEngine.Object> list, string strName)
        {
            var enumerator = list.GetEnumerator ();
            while (enumerator.MoveNext ()) {
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
        /// Converts the camel to underscore.
        /// </summary>
        /// <returns>The camel to underscore.</returns>
        /// <param name="input">Input.</param>
        public static string ConvertCamelToUnderscore (string input)
        {
            return System.Text.RegularExpressions.Regex.Replace (input, "(?x)( [A-Z][a-z,0-9]+ | [A-Z]+(?![a-z]) )", "_$0").ToLower ();
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
        public static T [] GetDistinctValues<T> (T[] array)
        {
            return GetDistinctValues<T> (new List<T> (array)).ToArray ();
        }

        /// <summary>
        /// Merge the specified dic and mergeDic.
        /// </summary>
        /// <param name="dic">Dic.</param>
        /// <param name="mergeDic">Merge dic.</param>
        /// <typeparam name="K">The 1st type parameter.</typeparam>
        /// <typeparam name="V">The 2nd type parameter.</typeparam>
        public static void Merge<K, V> (Dictionary<K, V> dic, Dictionary<K, V> mergeDic)
        {
            if (dic == null) {
                dic = mergeDic;
                return;
            }

            if (dic == null) {
                return;
            }

            foreach (KeyValuePair<K, V> pair in mergeDic) {
                if (!dic.ContainsKey (pair.Key)) {
                    dic.Add (pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Merge the specified list and mergeList.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="mergeList">Merge list.</param>
        /// <typeparam name="K">The 1st type parameter.</typeparam>
        /// <typeparam name="V">The 2nd type parameter.</typeparam>
        public static void Merge<K, V> (List<Dictionary<K, V>> list, List<Dictionary<K, V>> mergeList)
        {
            if (list == null) {
                list = mergeList;
                return;
            }

            if (list == null) {
                return;
            }

            for (int i = 0; i < list.Count; i++) {
                if (i > mergeList.Count) {
                    Util.Merge<K, V> (list [i], mergeList [i]);
                }
            }
        }

        /// <summary>
        /// Merge the specified iDic and mergeIDic.
        /// </summary>
        /// <param name="iDic">I dic.</param>
        /// <param name="mergeIDic">Merge I dic.</param>
        public static void Merge (IDictionary iDic, IDictionary mergeIDic)
        {
            if (iDic == null) {
                iDic = mergeIDic;
                return;
            }

            if (iDic == null) {
                return;
            }

            foreach (object obj in mergeIDic.Keys) {
                if (!iDic.Contains (obj)) {
                    iDic.Add (obj, mergeIDic [obj]);
                }
            }
        }

        /// <summary>
        /// Merge the specified iList and mergeIList.
        /// </summary>
        /// <param name="iList">I list.</param>
        /// <param name="mergeIList">Merge I list.</param>
        public static void Merge (IList iList, IList mergeIList)
        {
            if (iList == null) {
                iList = mergeIList;
                return;
            }

            if (iList == null) {
                return;
            }

            for (int i = 0; i < iList.Count; i++) {
                if (i < mergeIList.Count) {
                    Util.Merge ((IDictionary)iList [i], (IDictionary)mergeIList [i]);
                }
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="iList">IList.</param>
        /// <param name="keyName">Key name.</param>
        /// <param name="list">List.</param>
        public static List<object> GetValue (IList iList, string keyName, List<object> list = null)
        {
            if (list == null) {
                list = new List<object> ();
            }

            foreach (Dictionary<string, object> iDic in iList) {
                GetValue (iDic, keyName, list);
            }

            return list;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="iDic">IDictionary.</param>
        /// <param name="keyName">Key name.</param>
        /// <param name="list">List.</param>
        public static List<object> GetValue (IDictionary iDic, string keyName, List<object> list = null)
        {
            if (list == null) {
                list = new List<object> ();
            }

            foreach (object obj in iDic.Keys) {
                if (obj.ToString () == keyName) {
                    if (!list.Contains (iDic [obj])) {
                        list.Add (iDic [obj]);
                    }
                } else {
                    IList asList;
                    IDictionary asDic;
                    if ((asList = iDic [obj] as IList) != null) {
                        GetValue (asList, keyName, list);
                    } else if ((asDic = iDic [obj] as IDictionary) != null) {
                        GetValue (asDic as Dictionary<string, object>, keyName, list);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Determines if is integer the specified type.
        /// </summary>
        /// <returns><c>true</c> if is integer the specified type; otherwise, <c>false</c>.</returns>
        /// <param name="type">Type.</param>
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

        /// <summary>
        /// Determines if is float the specified type.
        /// </summary>
        /// <returns><c>true</c> if is float the specified type; otherwise, <c>false</c>.</returns>
        /// <param name="type">Type.</param>
        public static bool IsFloat (Type type)
        {
            return (type == typeof(float) | type == typeof(double) | type == typeof(Decimal));
        }

        /// <summary>
        /// Determines if is numeric the specified type.
        /// </summary>
        /// <returns><c>true</c> if is numeric the specified type; otherwise, <c>false</c>.</returns>
        /// <param name="type">Type.</param>
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
            } else {
                return true;
            }
        }

        /// <summary>
        /// Determines if is text the specified type.
        /// </summary>
        /// <returns><c>true</c> if is text the specified type; otherwise, <c>false</c>.</returns>
        /// <param name="type">Type.</param>
        public static bool IsText (Type type)
        {
            return (type == typeof(String) || type == typeof(Char));
        }

        /// <summary>
        /// Determines if is value type the specified type.
        /// </summary>
        /// <returns><c>true</c> if is value type the specified type; otherwise, <c>false</c>.</returns>
        /// <param name="type">Type.</param>
        public static bool IsValueType (Type type)
        {
            return (IsText (type) || IsNumeric (type));
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
    }
}
