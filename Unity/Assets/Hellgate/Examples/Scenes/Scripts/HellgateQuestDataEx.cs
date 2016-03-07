//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;

public class HellgateQuestDataEx
{
    public class User
    {
        private string prefab = "";
        private int speed = 0;

        public string Prefab {
            get {
                return prefab;
            }
        }

        public float Speed {
            get {
                return (float)speed / 100;
            }
        }
    }

    public class Missile
    {
        private string prefab = "";
        private int speed = 0;

        public string Prefab {
            get {
                return prefab;
            }
        }

        public float Speed {
            get {
                return (float)speed / 100;
            }
        }
    }

    private User user = null;
    private Missile[] missile = null;

    public User _User {
        get {
            return user;
        }
    }

    public List<Missile> _Missile {
        get {
            return new List<Missile> (missile);
        }
    }
}
