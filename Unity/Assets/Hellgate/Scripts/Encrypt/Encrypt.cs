//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Hellgate
{
    /// <summary>
    /// 3DESC, MD5, SHA1
    /// </summary>
    public class Encrypt
    {
        /// <summary>
        /// 3DESC descrypt
        /// </summary>
        /// <returns>The triple DES.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static string DeTripleDESC (string key, string value)
        {
            // retrieve encryped
            byte[] secret = MD5Hash (key);
            byte[] bytes = Convert.FromBase64String (value);

            // descrypt value 3DES
            TripleDES des = new TripleDESCryptoServiceProvider ();
            des.Key = secret;
            des.Mode = CipherMode.ECB;
            ICryptoTransform xform = des.CreateDecryptor ();
            byte[] decryped = xform.TransformFinalBlock (bytes, 0, bytes.Length);

            string decrypedString = Encoding.UTF8.GetString (decryped);

            return decrypedString;
        }

        /// <summary>
        /// 3DESC encrypt
        /// </summary>
        /// <returns>The DES.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static string TripleDESC (string key, string value)
        {
            // encrypt value
            byte[] secret = MD5Hash (key);
            byte[] bytes = Encoding.UTF8.GetBytes (value);

            TripleDES des = new TripleDESCryptoServiceProvider ();
            des.Key = secret;
            des.Mode = CipherMode.ECB;
            ICryptoTransform xform = des.CreateEncryptor ();
            byte[] encrypted = xform.TransformFinalBlock (bytes, 0, bytes.Length);

            // convert encrypt
            string encryptedString = Convert.ToBase64String (encrypted);
            return encryptedString;
        }

        /// <summary>
        /// MD5 hash.
        /// </summary>
        /// <returns>The d5 hash.</returns>
        /// <param name="strToEncrypt">String to encrypt.</param>
        public static byte[] MD5Hash (string strToEncrypt)
        {
            MD5 md5Hash = new MD5CryptoServiceProvider ();
            return md5Hash.ComputeHash (Encoding.UTF8.GetBytes (strToEncrypt));
        }

        /// <summary>
        /// Sha1s the key.
        /// </summary>
        /// <returns>The key.</returns>
        /// <param name="strToEncrypt">String to encrypt.</param>
        public static string SHA1Key (string strToEncrypt)
        {
            UTF8Encoding ue = new UTF8Encoding ();
            byte[] bytes = ue.GetBytes (strToEncrypt);

            SHA1 sha = new SHA1CryptoServiceProvider ();
            byte[] hashBytes = sha.ComputeHash (bytes);

            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++) {
                hashString += Convert.ToString (hashBytes [i], 16).PadLeft (2, '0');
            }

            return hashString.PadLeft (32, '0');
        }
    }
}
