//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace HellgateEditor
{
    public class EditorCoroutine
    {
        protected IEnumerator coroutine;

        public EditorCoroutine (IEnumerator coroutine)
        {
            this.coroutine = coroutine;
        }

        public void Start ()
        {
            EditorApplication.update += Update;
        }

        protected void Update ()
        {
            Stop ();
        }

        protected void Stop ()
        {
            if (!coroutine.MoveNext ()) {
                EditorApplication.update -= Update;
            }
        }
    }
}
