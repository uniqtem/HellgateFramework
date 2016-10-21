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
        private static bool CheckSerializableAttribute (Type type)
        {
            return type.GetAttributeValue<SerializableAttribute> () == null;
        }

        /// <summary>
        /// Generate a JSON representation of the private fields of an object. but System.Serializable attribute is public fields.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="t">T.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string ToJson<T> (T t)
        {
            Dictionary<string, object> temp = null;
            if (CheckSerializableAttribute (typeof(T))) {
                temp = Reflection.Convert<T> (t);
            } else {
#if UNITY_5_3 || UNITY_5_4
                return JsonUtility.ToJson (t);
#else
                temp = Reflection.Convert<T> (t, null, System.Reflection.BindingFlags.Public);
#endif
            }

            return Json.Serialize (temp);
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
            List<Dictionary<string, object>> temp = null;
            if (CheckSerializableAttribute (typeof(T))) {
                temp = Reflection.Convert<T> (list, System.Reflection.BindingFlags.NonPublic);
            } else {
                temp = Reflection.Convert<T> (list, System.Reflection.BindingFlags.Public);
            }

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
        /// Generate a JSON representation of the Dictionary<string, object> of an object.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="dic">Dic.</param>
        public static string ToJson (Dictionary<string, object> dic)
        {
            return Json.Serialize (dic);
        }

        /// <summary>
        /// Generate a JSON representation of the DataRow of an object.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="data">Data.</param>
        public static string ToJson (DataRow data)
        {
            return Json.Serialize (data);
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
        /// Generate a JSON representation of the List<Dictionary<string, object>> of an object.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="list">List.</param>
        public static string ToJson (List<Dictionary<string, object>> list)
        {
            return Json.Serialize (list);
        }

        /// <summary>
        /// Generate a JSON representation of the List<DataRow> of an object.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="list">List.</param>
        public static string ToJson (List<DataRow> list)
        {
            return Json.Serialize (list);
        }

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        /// <returns>The json.</returns>
        /// <param name="json">Json.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T FromJson<T> (string json)
        {
            if (CheckSerializableAttribute (typeof(T))) {
                return FromJson<T> (FromJson (json));
            } else {
#if UNITY_5_3 || UNITY_5_4
                return JsonUtility.FromJson<T> (json);
#else
                return FromJson<T> (FromJson (json));
#endif
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
            if (CheckSerializableAttribute (typeof(T))) {
                return Reflection.Convert<T> (iDic);
            } else {
                return Reflection.Convert<T> (iDic, null, System.Reflection.BindingFlags.Public);
            }
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
            if (CheckSerializableAttribute (typeof(T))) {
                return Reflection.Convert<T> (FromJsonList (json));
            } else {
                return Reflection.Convert<T> (FromJsonList (json), System.Reflection.BindingFlags.Public);
            }
        }

        /// <summary>
        /// Create an object from its JSON representation.
        /// </summary>
        /// <returns>The json array.</returns>
        /// <param name="iList">I list.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] FromJsonArray<T> (IList iList)
        {
            if (CheckSerializableAttribute (typeof(T))) {
                return Reflection.Convert<T> (iList);
            } else {
                return Reflection.Convert<T> (iList, System.Reflection.BindingFlags.Public);
            }
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
        /// Froms the json overwrite.
        /// </summary>
        /// <param name="json">Json.</param>
        /// <param name="obj">Object.</param>
        public static void FromJsonOverwrite (string json, object obj)
        {
            Type type = obj.GetType ();
            if (Util.IsArray (type)) {
                Type temp = type.GetElementType ();
                if (temp == null) {
                    temp = type.GetGenericArguments () [0];
                }
                type = temp;

                if (CheckSerializableAttribute (type)) {
                    Reflection.Invoke (FromJsonList (json), obj);
                } else {
                    Reflection.Invoke (FromJsonList (json), obj, System.Reflection.BindingFlags.Public);
                }
            } else {
                if (CheckSerializableAttribute (type)) {
                    Reflection.Invoke (FromJson (json), obj);
                } else {
#if UNITY_5_3 || UNITY_5_4
                    JsonUtility.FromJsonOverwrite (json, obj);
#else
                    Reflection.Invoke (FromJson (json), obj, System.Reflection.BindingFlags.Public);
#endif
                }
            }
        }
    }
}
