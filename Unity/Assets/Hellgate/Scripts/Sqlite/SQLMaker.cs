/// <summary>
/// SQL maker.
/// </summary>
using UnityEngine;
using System.Collections;

namespace Hellgate
{
	public class SQLMaker : SQLConverter
	{
		protected string GenerateInsertSQL (string query, string[] sA, bool apostrophe = false)
		{
			string apos = apostrophe ? "'" : "";
			
			int index = 0;
			foreach (string s in sA) {
				index++;
				
				query += apos + s + apos;
				
				if (sA.Length > index) {
					query += ", ";
				}
			}
			
			return query;
		}

		/// <summary>
		/// Generates the insert SQL.
		/// </summary>
		/// <returns>The insert SQL.</returns>
		/// <param name="tableName">Table name.</param>
		/// <param name="columnName">Column name.</param>
		/// <param name="data">Data.</param>
		public string GenerateInsertSQL (string tableName, string[] columnName, string[] data)
		{
			if (columnName.Length != data.Length) {
				return "";
			}

			string query = "INSERT INTO " + tableName + " (";
			query = GenerateInsertSQL (query, columnName);
			query += ") VALUES (";
			query = GenerateInsertSQL (query, data, true);
			query += ");";

			return query;
		}

		/// <summary>
		/// Generates the insert batch SQL.
		/// </summary>
		/// <returns>The insert batch SQL.</returns>
		/// <param name="tableName">Table name.</param>
		/// <param name="columnName">Column name.</param>
		/// <param name="data">Data.</param>
		public string GenerateInsertBatchSQL (string tableName, string[] columnName, string[][] data)
		{
			string query = "";
			for (int i = 0; i < data.Length; i++) {
				if (columnName.Length != data [i].Length) {
					continue;
				}
				
				query += "INSERT INTO " + tableName + " (";
				query = GenerateInsertSQL (query, columnName);
				query += ") VALUES (";
				query = GenerateInsertSQL (query, data [i], true);
				query += "); ";
			}

			return query;
		}

		/// <summary>
		/// Generates the update SQL.
		/// </summary>
		/// <returns>The update SQL.</returns>
		/// <param name="tableName">Table name.</param>
		/// <param name="columnName">Column name.</param>
		/// <param name="data">Data.</param>
		/// <param name="addQuery">Add query.</param>
		public string GenerateUpdateSQL (string tableName, string[] columnName, string[] data, string addQuery)
		{
			if (columnName.Length != data.Length) {
				return "";
			}
			
			string query = "UPDATE " + tableName + " SET ";
			for (int i = 0; i < columnName.Length; i++) {
				string temp = columnName [i] + " = '" + data [i] + "'";
				query += temp;
				if (i < (columnName.Length - 1)) {
					query += " , ";
				}
			}
			query += " " + addQuery + ";";

			return query;
		}

		/// <summary>
		/// Generates the update batch SQL.
		/// </summary>
		/// <returns>The update batch SQL.</returns>
		/// <param name="tableName">Table name.</param>
		/// <param name="whereName">Where name.</param>
		/// <param name="dataName">Data name.</param>
		/// <param name="where">Where.</param>
		/// <param name="data">Data.</param>
		public string GenerateUpdateBatchSQL (string tableName, string whereName, string dataName, string[] where, string[] data)
		{
			if (where.Length != data.Length) {
				return "";
			}
			
			string query = "UPDATE " + tableName + " SET " + dataName + " = CASE " + whereName + " ";
			
			for (int i = 0; i < where.Length; i++) {
				query += "WHEN " + where [i] + " THEN '" + data [i] + "' ";
			}
			
			query += "END WHERE " + whereName + " IN (";
			for (int j = 0; j < where.Length; j++) {
				query += "'" + where [j] + "'";
				
				if (j < (where.Length - 1)) {
					query += ", ";
				}
			}
			query += ");";

			return query;
		}

		/// <summary>
		/// Generates the select SQL.
		/// </summary>
		/// <returns>The select SQL.</returns>
		/// <param name="tableName">Table name.</param>
		/// <param name="addQuery">Add query.</param>
		public string GenerateSelectSQL (string tableName, string addQuery = "")
		{
			return "SELECT * FROM " + tableName + " " + addQuery + ";";
		}

		/// <summary>
		/// Generates the select sync SQL.
		/// </summary>
		/// <returns>The select sync SQL.</returns>
		/// <param name="tableName">Table name.</param>
		/// <param name="whereKey">Where key.</param>
		/// <param name="where">Where.</param>
		public string GenerateSelectSyncSQL (string tableName, string whereKey, string[] where)
		{
			string query = "SELECT * FROM " + tableName;
			
			query += " WHERE ";
			
			for (int i = 0; i < where.Length; i++) {
				query += whereKey + " = " + "'" + where [i] + "'";
				
				if (i < where.Length - 1) {
					query += " OR ";
				}
			}
			query += ";";

			return query;
		}

		/// <summary>
		/// Generates the delete SQL.
		/// </summary>
		/// <returns>The delete SQL.</returns>
		/// <param name="tableName">Table name.</param>
		/// <param name="addQuery">Add query.</param>
		public string GenerateDeleteSQL (string tableName, string addQuery = "")
		{
			string query = "DELETE FROM " + tableName;
			query += " " + addQuery + ";";

			return query;
		}

		/// <summary>
		/// Generates the where key value SQL.
		/// </summary>
		/// <returns>The where key value SQL.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public string GenerateWhereKeyValueSQL (string key, object value)
		{
			return "WHERE " + key + " = '" + value + "'";
		}

		/// <summary>
		/// Generates the create table SQL.
		/// </summary>
		/// <returns>The create table SQL.</returns>
		/// <param name="tableName">Table name.</param>
		/// <param name="configs">Configs.</param>
		public string GenerateCreateTableSQL (string tableName, AttributeMappingConfig<ColumnAttribute>[] configs)
		{
			string query = "CREATE TABLE '" + tableName + "' (";

			for (int i = 0; i < configs.Length; i++) {
				query += "'" + configs [i].name + "' ";
				if (configs [i].t == null || configs [i].t.Type == "") {
					query += ConvertToSQLType (configs [i].type);
				} else {
					query += configs [i].t.Type;
				}

				if (configs [i].t != null) {
					query += ConvertToSQLConstraints (configs [i].t.Constraints);
				}

				if (i < configs.Length - 1) {
					query += ", ";
				}

			}
			query += ");";

			return query;
		}
	}
}