//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Text;

namespace Hellgate
{
    public class SQLConverter
    {
        /// <summary>
        /// Encodes the non ASCII characters.
        /// </summary>
        /// <returns>The non ASCII characters.</returns>
        /// <param name="str">String.</param>
        public string EncodeNonAsciiCharacters (string str)
        {
            if (str == null || str == "" || str.Length <= 0) {
                return str;
            }

            StringBuilder sb = new StringBuilder ();
            foreach (char c in str) {
                if (c > 127) {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString ("x4");
                    sb.Append (encodedValue);
                } else {
                    sb.Append (c);
                }
            }
            return sb.ToString ();
        }

        /// <summary>
        /// Encodes the non ASCII characters.
        /// </summary>
        /// <returns>The non ASCII characters.</returns>
        /// <param name="strs">String array.</param>
        public string[] EncodeNonAsciiCharacters (string[] strs)
        {
            string[] temp = new string[strs.Length];
            for (int i = 0; i < strs.Length; i++) {
                temp [i] = EncodeNonAsciiCharacters (strs [i]);
            }

            return temp;
        }

        /// <summary>
        /// Decodes the encoded non ASCII characters.
        /// </summary>
        /// <returns>The encoded non ASCII characters.</returns>
        /// <param name="str">String.</param>
        public string DecodeEncodedNonAsciiCharacters (string str)
        {
            return System.Text.RegularExpressions.Regex.Replace (
                str,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse (m.Groups ["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString ();
                });
        }

        /// <summary>
        /// Decodes the encoded non ASCII characters.
        /// </summary>
        /// <returns>The encoded non ASCII characters.</returns>
        /// <param name="data">Data.</param>
        public DataTable DecodeEncodedNonAsciiCharacters (DataTable data)
        {
            if (data == null) {
                return null;
            }

            for (int i = 0; i < data.Rows.Count; i++) {
                for (int j = 0; j < data.Columns.Count; j++) {
                    object obj = DecodeEncodedNonAsciiCharacters (data.Rows [i] [data.Columns [j]].ToString ()) as object;
                    data.Rows [i] [data.Columns [j]] = obj;
                }
            }

            return data;
        }

        /// <summary>
        /// Converts the type of the to SQL.
        /// </summary>
        /// <returns>The to SQL type.</returns>
        /// <param name="type">Type.</param>
        public string ConvertToSQLType (Type type)
        {
            if (type == typeof(int) ||
                type == typeof(Int16) ||
                type == typeof(Int32) ||
                type == typeof(Int64) ||
                type == typeof(long)) {
                return "INTEGER";
            } else if (type == typeof(float) ||
                       type == typeof(double)) {
                return "REAL";
            } else if (type == typeof(byte[])) {
                return "BLOB";
            } else if (type == typeof(DateTime)) {
                return "NUMERIC";
            } else {
                return "TEXT";
            }
        }

        /// <summary>
        /// Converts to SQL constraints.
        /// </summary>
        /// <returns>The to SQL constraints.</returns>
        /// <param name="constraints">Constraints.</param>
        public string ConvertToSQLConstraints (DataConstraints[] constraints)
        {
            string cons = "";
            for (int i = 0; i < constraints.Length; i++) {
                switch (constraints [i]) {
                case DataConstraints.AI:
                    cons += " PRIMARY KEY AUTOINCREMENT";
                break;
                case DataConstraints.NOTNULL:
                    cons += " NOT NULL";
                break;
                case DataConstraints.PK:
                    if (Array.FindIndex (constraints, c => c == DataConstraints.AI) < 0) {
                        cons += " PRIMARY KEY";
                    }
                break;
                case DataConstraints.FK:

                break;
                case DataConstraints.UNIQUE:
                    cons += " UNIQUE";
                break;
                }
            }

            return cons;
        }

        /// <summary>
        /// Converts the type of the to SQL join.
        /// </summary>
        /// <returns>The to SQL join type.</returns>
        /// <param name="type">Type.</param>
        public string ConvertToSQLJoinType (SqliteJoinType type)
        {
            StringBuilder stringBuilder = new StringBuilder ();
            switch (type) {
            case SqliteJoinType.CROSS:
                stringBuilder.Append ("CROSS ");
            break;
            case SqliteJoinType.INNER:
                stringBuilder.Append ("INNER ");
            break;
            case SqliteJoinType.OUTER:
                stringBuilder.Append ("LEFT OUTER ");
            break;
            default:
                return "";
            }

            stringBuilder.Append ("JOIN {0} ON ");
            return stringBuilder.ToString ();
        }
    }
}
