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
                // ignore
                if (field.GetAttributeValue<IgnoreAttribute> () != null) {
                    continue;
                }

                // table
                mapper.SetType (copyType);

                // FK
                ColumnAttribute column = field.GetAttributeValue<ColumnAttribute> ();
                if (column != null && column.CheckConstraints (SqliteDataConstraints.FK)) {
                    if (column.Key != null && column.Value != "") {
                        mapper.SetJoin (field.Name, column.Key, column.Value);
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
                        SetSelectORMMaker (mapper, flag);
                    }
                }
            }

            return mapper;
        }
    }
}
