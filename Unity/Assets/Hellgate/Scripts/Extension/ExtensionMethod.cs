//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
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
        /// Merge the specified dic and mergeDic.
        /// </summary>
        /// <param name="dic">Dictionary.</param>
        /// <param name="mergeDic">Merge Dictionary.</param>
        /// <typeparam name="K">The 1st type parameter.</typeparam>
        /// <typeparam name="V">The 2nd type parameter.</typeparam>
        public static void Merge<K, V> (this Dictionary<K, V> dic, Dictionary<K, V> mergeDic)
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
        public static void Merge<K, V> (this List<Dictionary<K, V>> list, List<Dictionary<K, V>> mergeList)
        {
            if (list == null) {
                list = mergeList;
                return;
            }

            if (list == null) {
                return;
            }

            for (int i = 0; i < list.Count; i++) {
                list [i].Merge<K, V> (mergeList [i]);
            }
        }
    }
}

