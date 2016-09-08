//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

namespace Hellgate
{
    public class JsonUtil
    {
        /// <summary>
        /// Generate a JSON representation of the private fields of an object. but System.Serializable attribute is public fields.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="t">T.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string ToJson<T> (T t)
        {
            if (typeof(T).GetAttributeValue<SerializableAttribute> () == null) {
                Dictionary<string, object> temp = Reflection.Convert<T> (t);
                return Json.Serialize (temp);
            } else {
                return JsonUtility.ToJson (t);
            }
        }

        /// <summary>
        /// Generate a JSON representation of the private fields of an object. but System.Serializable attribute is public fields.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string ToJson<T> (T[] list)
        {
            return ToJson<T> (new List<T> (list));
        }

        /// <summary>
        /// Generate a JSON representation of the private fields of an object. System.Serializable attribute is public fields.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string ToJson<T> (List<T> list)
        {
            List<Dictionary<string, object>> temp = Reflection.Convert<T> (list, System.Reflection.BindingFlags.NonPublic);
            return Json.Serialize (temp);
        }

        /// <summary>
        /// Generate a JSON representation of the IDictionary of an object. System.Serializable attribute is public fields.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="iDic">I dic.</param>
        public static string ToJson (IDictionary iDic)
        {
            return Json.Serialize (iDic);
        }

        /// <summary>
        /// Generate a JSON representation of the IList of an object.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="iList">I list.</param>
        public static string ToJson (IList iList)
        {
            return Json.Serialize (iList);
        }

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="json">Json.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T FromJson<T> (string json)
        {
            if (typeof(T).GetAttributeValue<SerializableAttribute> () == null) {
                return FromJson<T> (FromJson (json));
            } else {
                return JsonUtility.FromJson<T> (json);
            }
        }

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="iDic">I dic.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T FromJson<T> (IDictionary iDic)
        {
            return Reflection.Convert<T> (iDic);
        }

        /// <summary>
        /// Create an IDictionary from its JSON representation.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="json">Json.</param>
        public static IDictionary FromJson (string json)
        {
            return (IDictionary)Json.Deserialize (json);
        }

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        /// <returns>The jsons.</returns>
        /// <param name="json">Json.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] FromJsonArray<T> (string json)
        {
            return Reflection.Convert<T> (FromJsonList (json));
        }

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        /// <returns>The json array.</returns>
        /// <param name="iList">I list.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] FromJsonArray<T> (IList iList)
        {
            return Reflection.Convert<T> (iList);
        }

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        /// <returns>The jsons.</returns>
        /// <param name="json">Json.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> FromJsonList<T> (string json)
        {
            return new List<T> (FromJsonArray<T> (json));
        }

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        /// <returns>The json list.</returns>
        /// <param name="iList">I list.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> FromJsonList<T> (IList iList)
        {
            return new List<T> (FromJsonArray<T> (iList));
        }

        /// <summary>
        /// Create an IList from its JSON representation.
        /// </summary>
        /// <returns>The json list.</returns>
        /// <param name="json">Json.</param>
        public static IList FromJsonList (string json)
        {
            return (IList)Json.Deserialize (json);
        }

        /// <summary>
        /// Froms the json overwrite.[uncompleted]
        /// </summary>
        /// <param name="json">Json.</param>
        /// <param name="obj">Object.</param>
        public static void FromJsonOverwrite (string json, object obj)
        {
            JsonUtility.FromJsonOverwrite (json, obj);
        }
    }
}
