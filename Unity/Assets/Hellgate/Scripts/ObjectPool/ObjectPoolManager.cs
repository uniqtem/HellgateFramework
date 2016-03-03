//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    public class ObjectPoolManager : MonoBehaviour
    {
#region Const

        protected const string OBJECT_POOL_MANAGER = "ObjectPoolManager";

#endregion

#region Singleton

        private static ObjectPoolManager instance = null;

        protected static ObjectPoolManager Instance {
            get {
                if (instance == null) {
                    GameObject gObj = new GameObject ();
                    instance = gObj.AddComponent<ObjectPoolManager> ();
                    gObj.name = OBJECT_POOL_MANAGER;
                }

                return instance;
            }
        }

#endregion

        protected Dictionary<GameObject, ObjectPool> objectPool;

        protected virtual void Awake ()
        {
            if (instance == null) {
                instance = this;
            }

            objectPool = new Dictionary<GameObject, ObjectPool> ();
        }

        protected virtual void OnDestory ()
        {
            instance = null;
        }

        /// <summary>
        /// Init the specified gameobject and count.
        /// </summary>
        /// <param name="gObj">GameObject.</param>
        /// <param name="count">Count.</param>
        public static void Init (GameObject gObj, int count = ObjectPool.CREATE_COUNT)
        {
            if (!Instance.objectPool.ContainsKey (gObj)) {
                Instance.objectPool.Add (gObj, new ObjectPool (gObj, instance.gameObject, count));
            }
        }

        /// <summary>
        /// Spawn the specified gameobject, position, rotation and count.
        /// </summary>
        /// <param name="gObj">GameObject.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="count">Count.</param>
        public static GameObject Spawn (GameObject gObj, Vector3 position, Quaternion rotation, int count = ObjectPool.CREATE_COUNT)
        {
            GameObject obj = Spawn (gObj, count);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
			
            return obj;
        }

        /// <summary>
        /// Spawn the specified gameobject and count.
        /// </summary>
        /// <param name="gObj">GameObject.</param>
        /// <param name="count">Count.</param>
        public static GameObject Spawn (GameObject gObj, int count = ObjectPool.CREATE_COUNT)
        {
            Init (gObj, count);
            return Instance.objectPool [gObj].GetObject ();
        }

        /// <summary>
        /// Despawn the specified gameobject.
        /// </summary>
        /// <param name="gObj">GameObject.</param>
        public static void Despawn (GameObject gObj)
        {
            if (gObj != null) {
                gObj.SetActive (false);
            }
        }

        /// <summary>
        /// Delaies the despawn.
        /// </summary>
        /// <param name="gObj">GameObject.</param>
        /// <param name="time">Time.</param>
        public static void DelayDespawn (GameObject gObj, float time)
        {
            SceneManager.Instance.Wait (time, delegate () {
                if (instance != null) {
                    Despawn (gObj);
                }
            });
        }
    }
}
