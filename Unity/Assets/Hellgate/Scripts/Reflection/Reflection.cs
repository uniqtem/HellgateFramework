//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Hellgate
{
    public partial class Reflection
    {
        private static object ConvertIgnoreData (FieldInfo field, object data)
        {
            if (data is Int64) {
                data = int.Parse (data.ToString ());
            } else if (field.FieldType == typeof(bool)) {
                data = Util.ToBoolean (data.ToString ());
            } else if (field.FieldType.IsEnum) {
                data = Enum.Parse (field.FieldType, data.ToString ());
            }

            return data;
        }

        private static List<FieldInfo> GetFields (Type type, BindingFlags flag, List<FieldInfo> list = null)
        {
            SerializableAttribute serializable = type.GetAttributeValue<SerializableAttribute> ();
            if (serializable != null) {
                flag = BindingFlags.NonPublic | BindingFlags.Public;
            }

            FieldInfo[] fieldInfos = type.GetFields (BindingFlags.Instance | flag);
            if (serializable != null) {
                if (list == null) {
                    list = new List<FieldInfo> ();
                }

                for (int i = 0; i < fieldInfos.Length; i++) {
                    if (fieldInfos [i].IsPrivate) {
                        if (fieldInfos [i].GetAttributeValue<SerializeField> () == null) {
                            continue;
                        }
                    }

                    list.Add (fieldInfos [i]);
                }

                return list;
            }

            if (list == null) {
                list = new List<FieldInfo> (fieldInfos);
            } else {
                list.AddRange (fieldInfos);
            }

            if (!(type.BaseType is System.Object)) {
                return GetFields (type.BaseType, flag, list);
            }

            return list;
        }

        private static IList CreateIListInstance (Type type)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType (type);
            var instance = Activator.CreateInstance (constructedListType);

            return (IList)instance;
        }

        private static Array CreateArrayInstance (IList list, Type type)
        {
            Array array = Array.CreateInstance (type, list.Count);
            for (int i = 0; i < list.Count; i++) {
                array.SetValue (System.Convert.ChangeType (list [i], type), i);
            }

            return array;
        }

        /// <summary>
        /// Convert the specified dic, fieldInfo, flag and type.
        /// </summary>
        /// <param name="dic">Dic.</param>
        /// <param name="fieldInfo">Field info.</param>
        /// <param name="flag">Flag.</param>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Convert<T> (IDictionary dic, List<FieldInfo> fieldInfos = null, BindingFlags flag = BindingFlags.NonPublic, Type type = null)
        {
            if (dic == null) {
                return default (T);
            }

            object obj = (T)Activator.CreateInstance (typeof(T), null);
            if (type != null) {
                obj = Activator.CreateInstance (type, null);
            }

            if (fieldInfos == null) {
                fieldInfos = GetFields (obj.GetType (), flag);
            }

            foreach (FieldInfo field in fieldInfos) {
                if (!dic.Contains (field.Name)) {
                    continue;
                }

                object data = dic [field.Name];
                data = ConvertIgnoreData (field, data);

                if (field.FieldType.IsClass && field.FieldType != typeof(String)) {
                    if (Util.IsArray (field.FieldType)) {
                        IList iList = (IList)data;
                        Type element = field.FieldType.GetElementType ();
                        if (element == null) {
                            Type[] types = field.FieldType.GetGenericArguments ();
                            if (types.Length <= 0) {
                                continue;
                            }

                            element = types [0];
                        }

                        if (Util.IsValueType (element)) {
                            field.SetValue (obj, CreateArrayInstance (iList, element));
                        } else {
                            data = Convert<object> (iList, flag, element);
                            Array someArray = data as Array;
                            if (someArray == null) {
                                continue;
                            }

                            if (field.FieldType.GetElementType () == null) { // list
                                iList = CreateIListInstance (element);
                                for (int i = 0; i < someArray.Length; i++) {
                                    iList.Add (System.Convert.ChangeType (someArray.GetValue (i), element));
                                }

                                field.SetValue (obj, iList);
                            } else { // array
                                Array filledArray = Array.CreateInstance (element, someArray.Length);
                                Array.Copy (someArray, filledArray, someArray.Length);

                                field.SetValue (obj, filledArray);
                            }
                        }

                        continue;
                    } else {
                        IDictionary iDic = (IDictionary)data;
                        List<FieldInfo> fields = GetFields (field.FieldType, flag);
                        data = Convert<object> (iDic, fields, flag, field.FieldType);
                    }
                }

                if (data == null) {
                    continue;
                }

                if (field.FieldType != data.GetType ()) {
                    try {
                        data = System.Convert.ChangeType (data, field.FieldType);
                    } catch (Exception e) {
                        HDebug.LogWarning (string.Format ("{0}\nclass : {1}, field : {2}", e.Message, obj, field.Name));
                        continue;
                    }
                }

                field.SetValue (obj, data);
            }

            return (T)obj;
        }

        /// <summary>
        /// Convert the specified list, flag and type.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="flag">Flag.</param>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] Convert<T> (IList list, BindingFlags flag = BindingFlags.NonPublic, Type type = null)
        {
            if (list == null) {
                return null;
            }

            object obj = (T)Activator.CreateInstance (typeof(T), null);
            if (type != null) {
                obj = Activator.CreateInstance (type, null);
            }

            List<FieldInfo> fieldInfo = GetFields (obj.GetType (), flag);
            T[] ts = new T[list.Count];
            for (int i = 0; i < list.Count; i++) {
                ts [i] = Convert<T> ((IDictionary)list [i], fieldInfo, flag, obj.GetType ());
            }

            return ts;
        }

        /// <summary>
        /// Convert the specified t, fieldInfos and flag.
        /// </summary>
        /// <param name="t">T.</param>
        /// <param name="fieldInfos">Field infos.</param>
        /// <param name="flag">Flag.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Dictionary<string, object> Convert<T> (T t = default (T), List<FieldInfo> fieldInfos = null, BindingFlags flag = BindingFlags.NonPublic)
        {
            if (t == null) {
                t = (T)Activator.CreateInstance (typeof(T), null);
            }

            if (Util.IsValueType (t.GetType ())) {
                return null;
            }

            if (fieldInfos == null) {
                fieldInfos = GetFields (t.GetType (), flag);
            }

            Dictionary<string, object> dic = new Dictionary<string, object> ();
            foreach (FieldInfo field in fieldInfos) {
                // attribute filter
                if (field.GetAttributeValue<IgnoreAttribute> () != null) {
                    continue;
                }

                // sqlite ignore
                if (SqliteIgnoreAttribute (field)) {
                    continue;
                }

                object data = field.GetValue (t);
                if (data == null) {
                    continue;
                }

                if (field.FieldType.IsClass && field.FieldType != typeof(String)) {
                    if (Util.IsArray (field.FieldType)) {
                        Type type = field.FieldType.GetElementType ();
                        if (type == null) {
                            type = field.FieldType.GetGenericArguments () [0];
                        }

                        if (!Util.IsValueType (type)) {
                            List<object> list = new List<object> ();
                            IList iList = (IList)data;
                            for (int i = 0; i < iList.Count; i++) {
                                list.Add (iList [i]);
                            }

                            data = Convert<object> (list, flag);
                        }
                    } else {
                        List<FieldInfo> fields = GetFields (field.FieldType, flag);
                        data = Convert<object> (data, fields, flag);
                    }
                }

                dic.Add (field.Name, data);
            }

            return dic;
        }

        /// <summary>
        /// Convert the specified list and flag.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="flag">Flag.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<Dictionary<string, object>> Convert<T> (List<T> list, BindingFlags flag = BindingFlags.NonPublic)
        {
            List<FieldInfo> fieldInfos = null;
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>> ();
            for (int i = 0; i < list.Count; i++) {
                if (i == 0) {
                    fieldInfos = GetFields (list [i].GetType (), flag);
                }

                Dictionary<string, object> temp = Convert<T> (list [i], fieldInfos, flag);
                if (temp != null) {
                    data.Add (temp);
                }
            }

            return data;
        }

        /// <summary>
        /// Convert the specified list and flag.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="flag">Flag.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<Dictionary<string, object>> Convert<T> (T[] list, BindingFlags flag = BindingFlags.NonPublic)
        {
            return Convert<T> (new List<T> (list), flag);
        }

        /// <summary>
        /// Fields the AMC retrieve.
        /// </summary>
        /// <returns>The AMC retrieve.</returns>
        /// <param name="type">Type.</param>
        /// <param name="flag">Flag.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static AttributeMappingConfig<T>[] FieldAMCRetrieve<T> (Type type, BindingFlags flag = BindingFlags.NonPublic) where T : class
        {
            List<FieldInfo> fieldInfos = GetFields (type, flag);

            AttributeMappingConfig<T>[] configs = new AttributeMappingConfig<T> [fieldInfos.Count];
            for (int i = 0; i < fieldInfos.Count; i++) {
                AttributeMappingConfig<T> temp = new AttributeMappingConfig<T> ();

                temp.t = fieldInfos [i].GetAttributeValue<T> ();
                temp.name = fieldInfos [i].Name;
                temp.type = fieldInfos [i].FieldType;
                configs [i] = temp;
            }

            return configs;
        }

        /// <summary>
        /// Gets the executing assembly.
        /// </summary>
        /// <returns>The executing assembly.</returns>
        public static Type[] GetExecutingAssembly ()
        {
            return Assembly.GetExecutingAssembly ().GetTypes ();
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <returns>The property type.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="flag">Flag.</param>
        public static Type GetPropType (object obj, string propName, BindingFlags flag = BindingFlags.Public)
        {
            return obj.GetType ().GetProperty (propName, BindingFlags.Instance | flag).PropertyType;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <returns>The property value.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="flag">Flag.</param>
        public static object GetPropValue (object obj, string propName, BindingFlags flag = BindingFlags.Public)
        {
            return obj.GetType ().GetProperty (propName, BindingFlags.Instance | flag).GetValue (obj, null);
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <returns>The field type.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <param name="flag">Flag.</param>
        public static Type GetFieldType (object obj, string fieldName, BindingFlags flag = BindingFlags.Public)
        {
            FieldInfo info = obj.GetType ().GetField (fieldName, BindingFlags.Instance | flag);
            if (info == null) {
                return null;
            }

            return info.FieldType;
        }

        /// <summary>
        /// Gets the type of the private field.
        /// </summary>
        /// <returns>The private field type.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="fieldName">Field name.</param>
        public static Type GetPrivateFieldType (object obj, string fieldName)
        {
            return GetFieldType (obj, fieldName, BindingFlags.NonPublic);
        }

        /// <summary>
        /// Gets the field vale.
        /// </summary>
        /// <returns>The field vale.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <param name="flag">Flag.</param>
        public static object GetFieldVale (object obj, string fieldName, BindingFlags flag = BindingFlags.Public)
        {
            FieldInfo info = obj.GetType ().GetField (fieldName, BindingFlags.Instance | flag);
            if (info == null) {
                return null;
            }

            return info.GetValue (obj);
        }

        /// <summary>
        /// Gets the private field value.
        /// </summary>
        /// <returns>The private field value.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="fieldName">Field name.</param>
        public static object GetPrivateFieldValue (object obj, string fieldName)
        {
            return GetFieldVale (obj, fieldName, BindingFlags.NonPublic);
        }
    }
}