//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    public class ObjectPool
    {
#region Const

        public const int CREATE_COUNT = 7;

#endregion

        protected GameObject parent;
        protected GameObject prefab;
        protected List<GameObject> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.ObjectPool"/> class.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="count">Count.</param>
        public ObjectPool (GameObject prefab, GameObject parent = null, int count = CREATE_COUNT)
        {
            this.prefab = prefab;
            this.parent = parent;

            list = new List<GameObject> ();
            for (int i = 0; i < count; i++) {
                GameObject temp = CreateObject ();
                temp.SetActive (false);

                list.Add (temp);
            }
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <returns>The object.</returns>
        public GameObject GetObject ()
        {
            for (int i = 0; i < list.Count; i++) {
                if (!list [i].activeSelf) {
                    list [i].SetActive (true);
                    return list [i];
                }
            }

            return CreateObject ();
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <returns>The object.</returns>
        public GameObject CreateObject ()
        {
            GameObject temp = GameObject.Instantiate (prefab) as GameObject;
            if (parent != null) {
                temp.transform.parent = parent.transform;
            }

            list.Add (temp);
            return temp;
        }
    }
}
