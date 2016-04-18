//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace Hellgate
{
    public static class ExtensionMethod
    {
#if UNITY_EDITOR
        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <returns>The attribute value.</returns>
        /// <param name="type">Type.</param>
        /// <param name="valueSelector">Value selector.</param>
        /// <typeparam name="TAttribute">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static TValue GetAttributeValue<TAttribute, TValue> (
            this Type type,
            Func<TAttribute, TValue> valueSelector) 
            where TAttribute : Attribute
        {
            var attr = type.GetCustomAttributes (
                typeof(TAttribute), true
            ).FirstOrDefault () as TAttribute;

            if (attr != null) {
                return valueSelector (attr);
            }
            return default(TValue);
        }
#endif

        /// <summary>
        /// Finds the attribute.
        /// </summary>
        /// <returns>The attribute.</returns>
        /// <param name="attributes">Attributes.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private static T FindAttribute<T> (Attribute[] attributes) where T : class
        {
            for (int i = 0; i < attributes.Length; i++) {
                if (attributes [i] is T) {
                    return attributes [i] as T;
                }
            }

            return default (T);
        }

        /// <summary>
        /// Merge the specified dic and mergeDic.
        /// </summary>
        /// <param name="dic">Dictionary.</param>
        /// <param name="mergeDic">Merge Dictionary.</param>
        /// <typeparam name="K">The 1st type parameter.</typeparam>
        /// <typeparam name="V">The 2nd type parameter.</typeparam>
        public static void Merge<K, V> (this Dictionary<K, V> dic, Dictionary<K, V> mergeDic)
        {
            Util.Merge<K, V> (dic, mergeDic);
        }

        /// <summary>
        /// Merge the specified list and mergeList.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="mergeList">Merge list.</param>
        /// <typeparam name="K">The 1st type parameter.</typeparam>
        /// <typeparam name="V">The 2nd type parameter.</typeparam>
        public static void Merge<K, V> (this List<Dictionary<K, V>> list, List<Dictionary<K, V>> mergeList)
        {
            Util.Merge<K, V> (list, mergeList);
        }

        /// <summary>
        /// Merge the specified iDic and mergeIDic.
        /// </summary>
        /// <param name="iDic">I dic.</param>
        /// <param name="mergeIDic">Merge I dic.</param>
        public static void Merge (this IDictionary iDic, IDictionary mergeIDic)
        {
            Util.Merge (iDic, mergeIDic);
        }

        /// <summary>
        /// Merge the specified iList and mergeIList.
        /// </summary>
        /// <param name="iList">I list.</param>
        /// <param name="mergeIList">Merge I list.</param>
        public static void Merge (this IList iList, IList mergeIList)
        {
            Util.Merge (iList, mergeIList);
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <returns>The attribute value.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetAttributeValue<T> (this Type type) where T : class
        {
            Attribute[] attributes = Attribute.GetCustomAttributes (type);
            return FindAttribute<T> (attributes);
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <returns>The attribute value.</returns>
        /// <param name="fieldInfo">Field info.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetAttributeValue<T> (this System.Reflection.FieldInfo fieldInfo) where T : class
        {
            Attribute[] attributes = Attribute.GetCustomAttributes (fieldInfo);
            return FindAttribute<T> (attributes);
        }

        /// <summary>
        /// Underscore the specified fieldInfo.
        /// </summary>
        /// <param name="fieldInfo">Field info.</param>
        public static string UnderscoreName (this System.Reflection.FieldInfo fieldInfo)
        {
            return Util.ConvertCamelToUnderscore (fieldInfo.Name);
        }

        /// <summary>
        /// Convert the specified IDictionary.
        /// </summary>
        /// <param name="iDic">I dic.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Convert<T> (this IDictionary iDic)
        {
            return Reflection.Convert<T> (iDic);
        }

        /// <summary>
        /// Convert the specified iList.
        /// </summary>
        /// <param name="iList">I list.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] Convert<T> (this IList iList)
        {
            return Reflection.Convert<T> (iList);
        }

        /// <summary>
        /// Gets the list object.
        /// </summary>
        /// <returns>The list object.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetListObject<T> (this List<object> list)
        {
            return Util.GetListObject<T> (list);
        }

        /// <summary>
        /// Gets the list objects.
        /// </summary>
        /// <returns>The list objects.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> GetListObjects<T> (this List<object> list)
        {
            return Util.GetListObjects<T> (list);
        }

        /// <summary>
        /// Finds the child object.
        /// </summary>
        /// <returns>The child object.</returns>
        /// <param name="gObj">G object.</param>
        /// <param name="strName">String name.</param>
        public static GameObject FindChildObject (this GameObject gObj, string strName)
        {
            return Util.FindChildObject (gObj, strName);
        }

        /// <summary>
        /// Finds the child object.
        /// </summary>
        /// <returns>The child object.</returns>
        /// <param name="gObj">G object.</param>
        /// <param name="strName">String name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T FindChildObject<T> (this GameObject gObj, string strName)
        {
            return Util.FindChildObject<T> (gObj, strName);
        }

        /// <summary>
        /// Find the specified list and strName.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="strName">String name.</param>
        public static Sprite Find (this List<Sprite> list, string strName)
        {
            return Util.FindSprite (list, strName);
        }

        /// <summary>
        /// Find the specified list and strName.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="strName">String name.</param>
        public static TextAsset Find (this List<TextAsset> list, string strName)
        {
            return Util.FindTextAsset (list, strName);
        }

        /// <summary>
        /// Find the specified list and strName.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="strName">String name.</param>
        public static GameObject Find (this List<GameObject> list, string strName)
        {
            return Util.FindGameObject (list, strName);
        }

        /// <summary>
        /// Find the specified list and strName.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="strName">String name.</param>
        public static UnityEngine.Object Find (this List<UnityEngine.Object> list, string strName)
        {
            return Util.FindObject (list, strName);
        }
    }
}

