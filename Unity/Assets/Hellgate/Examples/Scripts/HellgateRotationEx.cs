//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

namespace HellgeteEx
{
    public class HellgateRotationEx : MonoBehaviour
    {
        private Transform trans;
        [SerializeField]
        private float time = 200f;

        void Start ()
        {
            trans = transform;
        }

        void Update ()
        {
            trans.Rotate (0, 0, -Time.deltaTime * time);
        }
    }
}
