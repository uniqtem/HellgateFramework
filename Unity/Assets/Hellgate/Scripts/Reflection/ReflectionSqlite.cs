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
        private static bool SqliteIgnoreAttribute (FieldInfo field)
        {
            ColumnAttribute column = field.GetAttributeValue<ColumnAttribute> ();
            if (column != null) {
                if (column.CheckConstraints (DataConstraints.AI)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Convert the specified data and flag.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="flag">Flag.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static object Convert (DataRow row, BindingFlags flag = BindingFlags.NonPublic, Type type = null)
        {
            object obj = Activator.CreateInstance (type, null);
            List<FieldInfo>fieldInfos = GetFields (obj.GetType (), flag);

            bool returnFlag = false;
            string tableName = Query.GetTableName (type);
            foreach (FieldInfo field in fieldInfos) {
                object data = null;
                if (Util.IsValueType (field.FieldType)) {
                    string key = new SQLMaker ().Underline (tableName, field.UnderscoreName ());
                    if (row.ContainsKey (key)) {
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
                    data = Convert (row, flag, field.FieldType);
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
                ts [i] = (T)Convert (table.Rows [i], flag, typeof(T));
            }

            return ts;
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
            List<FieldInfo> fieldInfos = GetFields (mapper.Type, flag);
            foreach (FieldInfo field in fieldInfos) {
                // table
                mapper.SetType (copyType);

                // FK
                ColumnAttribute column = field.GetAttributeValue<ColumnAttribute> ();
                if (column != null && column.CheckConstraints (DataConstraints.FK)) {
                    if (column.Key != null && column.Value != "") {
                        mapper.SetJoin (column.Key, column.Value, field.UnderscoreName ());
                    } else {
                        HDebug.LogWarning (field.Name + " the column attribute is set problem");
                    }
                }

                // select
                if (Util.IsValueType (field.FieldType)) {
                    mapper.SetSelect (field.UnderscoreName ());
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
