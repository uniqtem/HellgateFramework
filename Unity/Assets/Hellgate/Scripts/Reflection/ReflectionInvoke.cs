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
        /// <summary>
        /// Invoke the specified iDic, obj and flag.
        /// </summary>
        /// <param name="iDic">IDictionary.</param>
        /// <param name="obj">Object.</param>
        /// <param name="flag">BindingFlags.</param>
        public static void Invoke (IDictionary iDic, object obj, BindingFlags flag = BindingFlags.NonPublic)
        {
            if (obj == null) {
                return;
            }

            if (iDic == null) {
                return;
            }

            foreach (object o in iDic.Keys) {
                IList asList = null;
                IDictionary asDic = null;
                string field = o.ToString ();
                object data = iDic [field];
                Type type = GetFieldType (obj, field, flag);

                if (type == null) {
                    continue;
                }

                if (data == null) {
                    continue;
                } else if ((asList = data as IList) != null) {
                    object value = GetFieldVale (obj, field, flag);
                    Type element = type.GetElementType ();
                    if (element == null) {
                        element = type.GetGenericArguments () [0];
                    }

                    if (value != null) {
                        if (Util.IsValueType (element)) {
                            data = CreateArrayInstance (asList, element);
                        } else {
                            Invoke (asList, value, flag);
                            continue;
                        }
                    } else {
                        object[] datas = Convert<object> (asList, flag, element);
                        if (element == null) {
                            element = type.GetGenericArguments () [0];
                            IList iList = CreateIListInstance (element);

                            for (int i = 0; i < datas.Length; i++) {
                                iList.Add (System.Convert.ChangeType (datas [i], element));
                            }

                            data = iList;
                        } else {
                            data = CreateArrayInstance (new List<object> (datas), element);
                        }
                    }
                } else if ((asDic = data as IDictionary) != null) {
                    object value = GetFieldVale (obj, field, flag);
                    if (value == null) {
                        data = Convert<object> (asDic, null, flag, type);
                    } else {
                        Invoke (asList, value, flag);
                        continue;
                    }
                }

                if (data == null) {
                    continue;
                }

                if (type != data.GetType ()) {
                    try {
                        data = System.Convert.ChangeType (data, type);
                    } catch (Exception e) {
                        HDebug.LogWarning (string.Format ("{0}\nclass : {1}, field : {2}", e.Message, obj, field));
                        continue;
                    }
                }

                if (flag == BindingFlags.NonPublic) {
                    SetPrivateFieldInvoke (obj, field, data);
                } else {
                    SetFieldInvoke (obj, field, data);
                }
            }
        }

        /// <summary>
        /// Invoke the specified iList, obj and flag.
        /// </summary>
        /// <param name="iList">Ilist.</param>
        /// <param name="obj">Object.</param>
        /// <param name="flag">BindingFlags.</param>
        public static void Invoke (IList iList, object obj, BindingFlags flag = BindingFlags.NonPublic)
        {
            IEnumerable enumerable = obj as IEnumerable;
            if (enumerable != null) {
                int index = 0;
                IEnumerator enumerator = enumerable.GetEnumerator ();
                while (enumerator.MoveNext ()) {
                    index++;
                }

                if (index == iList.Count) {
                    index = 0;
                    foreach (object o in enumerable) {
                        IDictionary asDic = null;
                        if ((asDic = o as IDictionary) != null) {
                            Invoke (asDic, o, flag);
                        }

                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the static method invoke.
        /// </summary>
        /// <returns>The static method invoke.</returns>
        /// <param name="typeName">Type name.</param>
        /// <param name="methodName">Method name.</param>
        /// <param name="types">Types.</param>
        /// <param name="datas">Datas.</param>
        public static object GetStaticMethodInvoke (string typeName, string methodName, Type[] types, object[] datas)
        {
            Type type = Type.GetType (typeName);
            MethodInfo method = type.GetMethod (
                                    methodName,
                                    BindingFlags.Static | BindingFlags.Public,
                                    Type.DefaultBinder,
                                    types,
                                    null
                                );

            return method.Invoke (null, datas);
        }

        /// <summary>
        /// Sets the invoke memeber.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="name">Name.</param>
        /// <param name="data">Data.</param>
        /// <param name="flags">Flags.</param>
        public static void SetInvokeMemeber (object obj, string name, object data, BindingFlags flags)
        {
            obj.GetType ().InvokeMember (name, flags, Type.DefaultBinder, obj, new object[] { data });
        }

        /// <summary>
        /// Sets the private property invoke.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="data">Data.</param>
        public static void SetPrivatePropInvoke (object obj, string propName, object data)
        {
            SetInvokeMemeber (obj, propName, data, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="data">Data.</param>
        public static void SetPropInvoke (object obj, string propName, object data)
        {
            SetInvokeMemeber (obj, propName, data, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
        }

        /// <summary>
        /// Sets the private field invoke.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <param name="data">Data.</param>
        public static void SetPrivateFieldInvoke (object obj, string fieldName, object data)
        {
            SetInvokeMemeber (obj, fieldName, data, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField);
        }

        /// <summary>
        /// Sets the field invoke.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <param name="data">Data.</param>
        public static void SetFieldInvoke (object obj, string fieldName, object data)
        {
            SetInvokeMemeber (obj, fieldName, data, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField);
        }
    }
}

