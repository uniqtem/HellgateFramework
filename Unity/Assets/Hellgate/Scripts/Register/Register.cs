//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    /// <summary>
    /// Encryption/Decryption game sessions.
    /// </summary>
    public class Register
    {
        public const string DEFAULT = "0";

        /// <summary>
        /// Sets the int.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetInt (string key, int value)
        {
            SetString (key, value.ToString ());
        }

        /// <summary>
        /// Sets the float.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetFloat (string key, float value)
        {
            SetString (key, value.ToString ());
        }

        /// <summary>
        /// Sets the string.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetString (string key, string value)
        {
            PlayerPrefs.SetString (Encrypt.SHA1Key (key), Encrypt.TripleDESC (key, value));
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public static int GetInt (string key, int defaultValue = 0)
        {
            return int.Parse (GetString (key, defaultValue.ToString ()));
        }

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public static float GetFloat (string key, float defaultValue = 0)
        {
            return float.Parse (GetString (key, defaultValue.ToString ()));
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public static string GetString (string key, string defaultValue = DEFAULT)
        {
            string value = PlayerPrefs.GetString (Encrypt.SHA1Key (key), defaultValue);
            if (value != defaultValue) {
                value = Encrypt.DeTripleDESC (key, value);
            }
            return value;
        }

        /// <summary>
        /// Delete the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        public static void Delete (string key)
        {
            PlayerPrefs.DeleteKey (key);
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        public static void DeleteAll ()
        {
            PlayerPrefs.DeleteAll ();
        }
    }
}
