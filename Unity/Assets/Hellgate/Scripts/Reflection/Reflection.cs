//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
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

        /// <summary>
        /// Convert the specified dic, fieldInfo, flag and type.
        /// </summary>
        /// <param name="dic">Dic.</param>
        /// <param name="fieldInfo">Field info.</param>
        /// <param name="flag">Flag.</param>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Convert<T> (IDictionary dic, FieldInfo[] fieldInfos = null, BindingFlags flag = BindingFlags.NonPublic, Type type = null)
        {
            object obj = (T)Activator.CreateInstance (typeof(T), null);
            if (type != null) {
                obj = Activator.CreateInstance (type, null);
            }

            if (fieldInfos == null) {
                fieldInfos = obj.GetType ().GetFields (BindingFlags.Instance | flag);
            }

            foreach (FieldInfo field in fieldInfos) {
                if (!dic.Contains (field.Name)) {
                    continue;
                }

                object data = dic [field.Name];
                data = ConvertIgnoreData (field, data);

                if (field.FieldType.IsClass && field.FieldType != typeof(String)) {
                    if (field.FieldType.IsArray || typeof(IList).IsAssignableFrom (field.FieldType)) {
                        IList iList = (IList)data;
                        Type tType = field.FieldType.GetElementType ();
                        if (Util.IsValueType (tType)) {
                            Array filledArray = Array.CreateInstance (tType, iList.Count);
                            for (int i = 0; i < iList.Count; i++) {
                                filledArray.SetValue (System.Convert.ChangeType (iList [i], tType), i);
                            }

                            field.SetValue (obj, filledArray);
                        } else {
                            data = Convert<object> (iList, flag, tType);
                            Array someArray = data as Array;
                            Array filledArray = Array.CreateInstance (tType, someArray.Length);
                            Array.Copy (someArray, filledArray, someArray.Length);

                            field.SetValue (obj, filledArray);
                        }

                        continue;
                    } else {
                        IDictionary iDic = (IDictionary)data;
                        FieldInfo[] fields = field.FieldType.GetFields (BindingFlags.Instance | flag);
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
                        HDebug.LogWarning (string.Format ("{0}\nclass : {1}, field : {2}", e.Message, type.Name, field.Name));
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
            object obj = (T)Activator.CreateInstance (typeof(T), null);
            if (type != null) {
                obj = Activator.CreateInstance (type, null);
            }

            FieldInfo[] fieldInfo = obj.GetType ().GetFields (BindingFlags.Instance | flag);
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
        public static Dictionary<string, object> Convert<T> (T t = default (T), FieldInfo[] fieldInfos = null, BindingFlags flag = BindingFlags.NonPublic)
        {
            if (fieldInfos == null) {
                if (t == null) {
                    t = (T)Activator.CreateInstance (typeof(T), null);
                }

                fieldInfos = t.GetType ().GetFields (BindingFlags.Instance | flag);
            }

            Dictionary<string, object> data = new Dictionary<string, object> ();
            foreach (FieldInfo field in fieldInfos) {
                // attribute filter
                if (field.GetAttributeValue<IgnoreAttribute> () != null) {
                    continue;
                }

                // sqlite ignore
                if (SqliteIgnoreAttribute (field)) {
                    continue;
                }

                data.Add (field.Name, field.GetValue (t));
            }

            return data;
        }

        /// <summary>
        /// Convert the specified list and flag.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="flag">Flag.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<Dictionary<string, object>> Convert<T> (List<T> list, BindingFlags flag = BindingFlags.NonPublic)
        {
            FieldInfo[] fieldInfos = list [0].GetType ().GetFields (BindingFlags.Instance | flag);

            List<Dictionary<string, object>> data = new List<Dictionary<string, object>> ();
            for (int i = 0; i < list.Count; i++) {
                Dictionary<string, object> temp = Convert<T> (list [i], fieldInfos, flag);
                data.Add (temp);
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
            FieldInfo[] fieldInfos = type.GetFields (BindingFlags.Instance | flag);

            AttributeMappingConfig<T>[] configs = new AttributeMappingConfig<T> [fieldInfos.Length];
            for (int i = 0; i < fieldInfos.Length; i++) {
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
    }
}