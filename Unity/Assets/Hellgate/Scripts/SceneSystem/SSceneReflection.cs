//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Reflection;

namespace Hellgate
{
	public class SSceneReflection
	{
		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <returns>The property value.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="propName">Property name.</param>
		public static object GetPropValue(object obj, string propName)
		{
			return obj.GetType().GetProperty(propName).GetValue(obj, null);
		}

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="propName">Property name.</param>
		/// <param name="data">Data.</param>
		public static void SetPropValue(object obj, string propName, object data)
		{
			obj.GetType().InvokeMember (
				propName, 
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
				System.Type.DefaultBinder,
				obj,
				new object[] {data}
			);
		}
	}
}
