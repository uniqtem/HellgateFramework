//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

public class HellgateManifestDataEx
{
    public class Client
    {
        private int major = 0;
        private int minor = 0;
        private int version = 0;
        private int status = 0;
        private string platform = "";
        private string storeUrl = "";

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

        public int Version {
            get {
                return version;
            }
        }

        public int Status {
            get {
                return status;
            }
        }

        public string Platform {
            get {
                return platform;
            }
        }

        public string StoreUrl {
            get {
                return storeUrl;
            }
        }
    }

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

        public int Version {
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

    public class Host
    {
        private string resource = "";
        private string auth = "";
        private string login = "";
        private string game = "";
        private string notice = "";

        public string Resource {
            get {
                return resource;
            }
        }

        public string Auth {
            get {
                return auth;
            }
        }

        public string Login {
            get {
                return login;
            }
        }

        public string Game {
            get {
                return game;
            }
        }

        public string Notice {
            get {
                return notice;
            }
        }
    }

    private Client client = null;
    private Resource resource = null;
    private Host host = null;
    private long maxChacing = 0;

    public Client _Client {
        get {
            return client;
        }
    }

    public Resource _Resource {
        get {
            return resource;
        }
    }

    public Host _Host {
        get {
            return host;
        }
    }

    public long MaxChacing {
        get {
            return maxChacing;
        }
    }
}
