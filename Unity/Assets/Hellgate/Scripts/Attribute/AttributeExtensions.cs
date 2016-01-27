//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace Hellgate
{
	public static class AttributeExtensions
	{
#if UNITY_EDITOR
		/// <summary>
		/// Gets the attribute value.
		/// </summary>
		/// <returns>The attribute value.</returns>
		/// <param name="type">Type.</param>
		/// <param name="valueSelector">Value selector.</param>
		/// <typeparam name="TAttribute">The 1st type parameter.</typeparam>
		/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
		public static TValue GetAttributeValue<TAttribute, TValue>(
			this Type type,
			Func<TAttribute, TValue> valueSelector) 
			where TAttribute : Attribute
		{
			var attr = type.GetCustomAttributes(
				typeof(TAttribute), true
				).FirstOrDefault() as TAttribute;

			if (attr != null) {
				return valueSelector(attr);
			}
			return default(TValue);
		}
#endif

		/// <summary>
		/// Finds the attribute.
		/// </summary>
		/// <returns>The attribute.</returns>
		/// <param name="attributes">Attributes.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private static T FindAttribute<T> (Attribute[] attributes) where T : class
		{
			for (int i = 0; i < attributes.Length; i++) {
				if (attributes [i] is T) {
					return attributes [i] as T;
				}
			}
			
			return default (T);
		}

		/// <summary>
		/// Gets the attribute value.
		/// </summary>
		/// <returns>The attribute value.</returns>
		/// <param name="type">Type.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetAttributeValue<T> (this Type type) where T : class
		{
			Attribute[] attributes = Attribute.GetCustomAttributes (type);
			return FindAttribute<T> (attributes);
		}

		/// <summary>
		/// Gets the attribute value.
		/// </summary>
		/// <returns>The attribute value.</returns>
		/// <param name="fieldInfo">Field info.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetAttributeValue<T> (this System.Reflection.FieldInfo fieldInfo) where T : class
		{
			Attribute[] attributes = Attribute.GetCustomAttributes (fieldInfo);
			return FindAttribute<T> (attributes);
		}
	}
}
