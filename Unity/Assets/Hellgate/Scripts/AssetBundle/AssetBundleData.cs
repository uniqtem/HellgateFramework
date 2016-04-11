//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Hellgate
{
    public class AssetBundleData
    {
#region Static

        /// <summary>
        /// The base url.
        /// </summary>
        public static string BASE_URL = "";
        /// <summary>
        /// The extension.
        /// </summary>
        public static string EXTENSION = "";

#endregion

        /// <summary>
        /// The URL.
        /// </summary>
        public string url;
        /// <summary>
        /// The name of the asset bundle.
        /// </summary>
        public string assetBundleName;
        /// <summary>
        /// The name of the object.
        /// </summary>
        public string objName;
        /// <summary>
        /// The version.
        /// </summary>
        public int version;
        /// <summary>
        /// The type.
        /// </summary>
        public System.Type type;
        /// <summary>
        /// The async flag.
        /// </summary>
        public bool async = true;
        /// <summary>
        /// The asset bundle.
        /// </summary>
        public AssetBundle assetBundle = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.AssetBundleData"/> class.
        /// </summary>
        /// <param name="assetBundleName">Asset bundle name.</param>
        /// <param name="vervion">Vervion.</param>
        public AssetBundleData (string assetBundleName)
        {
            Init (assetBundleName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.AssetBundleData"/> class.
        /// </summary>
        /// <param name="assetBundleName">Asset bundle name.</param>
        /// <param name="version">Version.</param>
        public AssetBundleData (string assetBundleName, int version)
        {
            Init (assetBundleName);
            this.version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.AssetBundleData"/> class.
        /// </summary>
        /// <param name="assetBundleName">Asset bundle name.</param>
        /// <param name="objName">Object name.</param>
        public AssetBundleData (string assetBundleName, string objName)
        {
            Init (assetBundleName);
            this.objName = objName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.AssetBundleData"/> class.
        /// </summary>
        /// <param name="assetBundeName">Asset bunde name.</param>
        /// <param name="objName">Object name.</param>
        /// <param name="version">Version.</param>
        /// <param name="type">Type.</param>
        public AssetBundleData (string assetBundleName, string objName, System.Type type)
        {
            Init (assetBundleName);
            this.objName = objName;
            this.type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.AssetBundleData"/> class.
        /// </summary>
        /// <param name="assetBundleName">Asset bundle name.</param>
        /// <param name="version">Version.</param>
        /// <param name="objName">Object name.</param>
        /// <param name="type">Type.</param>
        public AssetBundleData (string assetBundleName, int version, string objName, System.Type type)
        {
            Init (assetBundleName);
            this.version = version;
            this.objName = objName;
            this.type = type;
        }

        /// <summary>
        /// Common init the specified assetBundeName.
        /// </summary>
        /// <param name="assetBundeName">Asset bunde name.</param>
        protected void Init (string assetBundeName)
        {
            StringBuilder urlString = new StringBuilder ();
            StringBuilder assetBundleString = new StringBuilder ();
            if (BASE_URL != "") {
                if (!BASE_URL.EndsWith ("/")) {
                    urlString.AppendFormat ("{0}/", BASE_URL);
                } else {
                    urlString.Append (BASE_URL);
                }

                urlString.Append (assetBundeName);
            }
            assetBundleString.Append (assetBundeName);

            if (EXTENSION != "") {
                if (urlString.Length > 0) {
                    urlString.AppendFormat (".{0}", EXTENSION);
                    this.url = urlString.ToString ();
                }

                assetBundleString.AppendFormat (".{0}", EXTENSION);
                this.assetBundleName = assetBundleString.ToString ();
            }

            this.version = Register.GetInt (assetBundeName, 1);
            this.type = typeof(GameObject);
        }
    }
}
