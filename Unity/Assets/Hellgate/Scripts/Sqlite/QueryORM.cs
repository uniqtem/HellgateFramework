//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
	public partial class Query
	{
		protected System.Reflection.BindingFlags blindingFlags = System.Reflection.BindingFlags.NonPublic;

		/// <summary>
		/// Sets the blinding flags.
		/// Only BindingFlags.NonPublic and BindingFlags.Public
		/// </summary>
		/// <value>The blinding flags.</value>
		public System.Reflection.BindingFlags BlindingFlags {
			set {
				if (value == System.Reflection.BindingFlags.NonPublic || value == System.Reflection.BindingFlags.Public) {
					blindingFlags = value;
				} else {
					Debug.LogWarning ("Only BindingFlags.NonPublic and BindingFlags.Public");
				}
			}
		}

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		/// <returns>The table name.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		protected string GetTableName<T> ()
		{
			TableAttribute table = typeof(T).GetAttributeValue<TableAttribute> ();

			if (table == null) {
				return "";
			} else if (table.TableName == "") {
				return typeof(T).Name;
			} else {
				return table.TableName;
			}
		}

		/// <summary>
		/// INSERT the specified tableName and t.
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <param name="t">T.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void INSERT<T> (string tableName, T t)
		{
			Dictionary<string, object> data = Reflection.Convert<T> (t, null, blindingFlags);
			INSERT (tableName, data);
		}

		/// <summary>
		/// INSERT the specified t.
		/// </summary>
		/// <param name="t">T.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void INSERT<T> (T t)
		{
			INSERT<T> (GetTableName<T> (), t);
		}

		/// <summary>
		/// INSERT BATCH.
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void INSERT_BATCH<T> (string tableName, List<T> list)
		{
			List<Dictionary<string, object>> data = Reflection.Convert<T> (list, blindingFlags);

			string[] columns = new string[data [0].Count];
			string[][] qData = new string[data.Count][];
			for (int i = 0; i < data.Count; i++) {
				string[] temp = new string[data [i].Count];
				int index = 0;
				foreach (KeyValuePair<string, object> pair in data [i]) {
					if (i == 0) {
						columns [index] = pair.Key;
					}

					temp [index] = pair.Value.ToString ();
					index++;
				}

				qData [i] = temp;
			}

			INSERT_BATCH (tableName, columns, qData);
		}

		/// <summary>
		/// INSERT BATCH.
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void INSERT_BATCH<T> (string tableName, T[] list)
		{
			INSERT_BATCH<T> (tableName, new List<T> (list));
		}

		/// <summary>
		/// INSERT BATCH.
		/// </summary>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void INSERT_BATCH<T> (List<T> list)
		{
			INSERT_BATCH<T> (GetTableName<T> (), list);
		}

		/// <summary>
		/// INSERT BATCH.
		/// </summary>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void INSERT_BATCH<T> (T[] list)
		{
			INSERT_BATCH<T> (GetTableName<T> (), new List<T> (list));
		}

		/// <summary>
		/// UPDATE the specified tableName, t and addQuery.
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <param name="t">T.</param>
		/// <param name="addQuery">Add query.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void UPDATE<T> (string tableName, T t, string addQuery)
		{
			Dictionary<string, object> data = Reflection.Convert<T> (t, null, blindingFlags);
			UPDATE (tableName, data, addQuery);
		}

		/// <summary>
		/// UPDATE the specified t and addQuery.
		/// </summary>
		/// <param name="t">T.</param>
		/// <param name="addQuery">Add query.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void UPDATE<T> (T t, string addQuery)
		{
			UPDATE<T> (GetTableName<T> (), t, addQuery);
		}

		/// <summary>
		/// UPDATE the specified tableName, t, key and value.
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <param name="t">T.</param>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void UPDATE<T> (string tableName, T t, string key, object value)
		{
			Dictionary<string, object> data = Reflection.Convert<T> (t, null, blindingFlags);
			UPDATE (tableName, data, key, value);
		}

		/// <summary>
		/// UPDATE the specified t, key and value.
		/// </summary>
		/// <param name="t">T.</param>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void UPDATE<T> (T t, string key, object value)
		{
			UPDATE<T> (GetTableName<T> (), t, key, value);
		}

		/// <summary>
		/// SELECT the specified tableName and addQuery.
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <param name="addQuery">Add query.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T[] SELECT<T> (string tableName = "", string addQuery = "")
		{
			if (tableName == "") {
				tableName = GetTableName<T> ();
			}
		
			DataTable data = SELECT (tableName, addQuery);
			if (data == null || data.Rows.Count <= 0) {
				return null;
			}

			return Reflection.Convert<T> (data, blindingFlags);
		}

		/// <summary>
		/// SELECT the specified addQuery.
		/// </summary>
		/// <param name="addQuery">Add query.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T[] SELECT<T> (string addQuery)
		{
			return SELECT<T> (GetTableName<T> (), addQuery);
		}

		/// <summary>
		/// SELECT the specified key and value.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T[] SELECT<T> (string key, object value)
		{
			return SELECT<T> (GetTableName<T> (), key, value);
		}

		/// <summary>
		/// SELECT the specified tableName, key and value.
		/// </summary>
		/// <param name="tableName">Table name.</param>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T[] SELECT<T> (string tableName, string key, object value)
		{
			DataTable data = SELECT (tableName, key, value);
			if (data == null || data.Rows.Count <= 0) {
				return null;
			}

			return Reflection.Convert<T> (data, blindingFlags);
		}

		/// <summary>
		/// DELETE the specified addQuery.
		/// </summary>
		/// <param name="addQuery">Add query.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void DELETE<T> (string addQuery = "")
		{
			DELETE (GetTableName<T> (), addQuery);
		}

		/// <summary>
		/// DELETE the specified key and value.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void DELETE<T> (string key, object value)
		{
			DELETE (GetTableName<T> (), key, value);
		}
	}
}
