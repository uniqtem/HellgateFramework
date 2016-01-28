//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    /// <summary>
    /// Asset bundle inital status.
    /// </summary>
    public enum AssetBundleInitalStatus
    {
        Start = 1,
        Over,
        DownloadOver,
        HttpOver,
        HttpTimeover,
        HttpError
    }

    public class AssetBundleInitialData
    {
        public const string MAJOR = "major";
        public const string MINOR = "minor";

        public class Resource
        {
            private int major = 0;
            private int minor = 0;
            private int version = 0;
            private string name = "";

            public int Major {
                get {
                    return major;
                }
            }

            public int Minor {
                get {
                    return minor;
                }
            }

            public int _Version {
                get {
                    return version;
                }
            }

            public string Name {
                get {
                    return name;
                }
            }
        }

        public class AssetBundle
        {
            private string name = "";
            private int version = 0;

            public string Name {
                get {
                    return name;
                }
            }

            public int _Version {
                get {
                    return version;
                }
            }
        }

        private Resource resource = null;
        private AssetBundle[] assetbundle = null;

        public Resource _Resource {
            get {
                return resource;
            }
        }

        public List<AssetBundleInitialData.AssetBundle> _AssetBundle {
            get {
                return new List<AssetBundleInitialData.AssetBundle> (assetbundle);
            }
        }
    }

}
