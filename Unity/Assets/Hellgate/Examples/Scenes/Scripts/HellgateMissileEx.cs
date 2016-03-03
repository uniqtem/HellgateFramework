//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HellgateMissileEx : MonoBehaviour
{
    public Vector3 target;
    public float speed = 10f;
    private Transform trans;

    void Start ()
    {
        trans = transform;
    }

    void Update ()
    {
        trans.forward = target;
        trans.Translate (Vector3.forward * Time.deltaTime * speed);
    }
}
