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
        /// <summary>
        /// Convert the specified data and flag.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="flag">Flag.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static object Convert (DataRow row, BindingFlags flag = BindingFlags.NonPublic, Type type = null)
        {
            object obj = Activator.CreateInstance (type, null);
            FieldInfo[] fieldInfos = obj.GetType ().GetFields (BindingFlags.Instance | flag);

            bool returnFlag = false;
            string tableName = Query.GetTableName (type);
            foreach (FieldInfo field in fieldInfos) {
                object data = null;
                if (Util.IsValueType (field.FieldType)) {
                    string key = string.Format ("{0}{1}{2}", tableName, Query.UNDERLINE, field.Name);
                    if (row.ContainsKey (tableName + Query.UNDERLINE + field.Name)) {
                        data = row [key];
                        data = ConvertIgnoreData (field, data);
                        if (data == null || data.ToString () == "") {
                            continue;
                        } else {
                            returnFlag = true;
                        }

                        row.Remove (key);
                    }
                } else {
                    data = Convert(row, flag, field.FieldType);
                }

                if (data != null) {
                    field.SetValue (obj, data);
                }
            }

            if (returnFlag) {
                return obj;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Convert the specified table and flag.
        /// </summary>
        /// <param name="table">Table.</param>
        /// <param name="flag">Flag.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] Convert<T> (DataTable table, BindingFlags flag = BindingFlags.NonPublic)
        {
            T[] ts = new T[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++) {
                ts[i] = (T)Convert (table.Rows [i], flag, typeof (T));
            }

            return ts;
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
        /// Sets the select ORM mapper.
        /// </summary>
        /// <returns>The select ORM mapper.</returns>
        /// <param name="mapper">Mapper.</param>
        /// <param name="flag">Flag.</param>
        /// <param name="index">Index.</param>
        public static SelectORMMaker SetSelectORMMaker (SelectORMMaker mapper, BindingFlags flag = BindingFlags.NonPublic)
        {
            if (mapper.Type == null) {
                return mapper;
            }

            Type copyType = mapper.Type;
            FieldInfo[] fieldInfos = mapper.Type.GetFields (BindingFlags.Instance | flag);
            foreach (FieldInfo field in fieldInfos) {
                // table
                mapper.SetType (copyType);

                // FK
                ColumnAttribute column = field.GetAttributeValue<ColumnAttribute> ();
                if (column != null && column.CheckConstraints (SqliteDataConstraints.FK)) {
                    if (column.Key != null && column.Value != "") {
                        mapper.SetJoin (column.Key, column.Value, field.Name);
                    } else {
                        HDebug.LogWarning (field.Name + " the column attribute is set problem");
                    }
                }

                // select
                if (Util.IsValueType (field.FieldType)) {
                    mapper.SetSelect (field.Name);
                } else {
                    if (mapper.SetType (field.FieldType, copyType) == "") {
                        HDebug.LogWarning (field.FieldType + " the table is not set.");
                        mapper.SetType (copyType);
                    } else {
                        JoinAttribute join = field.GetAttributeValue<JoinAttribute> ();
                        if (join == null) {
                            mapper.SetJoinType ();
                        } else {
                            mapper.SetJoinType (join.Type);
                        }

                        SetSelectORMMaker (mapper, flag);
                    }
                }
            }

            return mapper;
        }
    }
}