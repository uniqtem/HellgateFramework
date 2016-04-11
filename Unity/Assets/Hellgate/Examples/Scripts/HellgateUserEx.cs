//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using Hellgate;

public class HellgateUserEx : MonoBehaviour
{
    public float speed = 5f;
    private Transform trans;
    private Vector3 pos;
    private bool inputFlag;
    private bool dieFlag;

    private HellgateQuestEx manager;

    void Start ()
    {
        manager = GameObject.FindObjectOfType<HellgateQuestEx> ();

        trans = transform;
        pos = trans.position;
        inputFlag = false;
        dieFlag = false;
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown (0)) {
            if (!CameraUtil.GetClickNGUI ()) {
                inputFlag = true;
            }
        }

        if (inputFlag) {
            pos = Input.mousePosition;
            pos.y += 20f;
            pos = Camera.main.ScreenToWorldPoint (pos);
        }

        float step = speed * Time.deltaTime;
//      trans.position = Vector3.Lerp (trans.position, pos, step);
        trans.position = Vector3.MoveTowards (trans.position, pos, step);

        if (Input.GetMouseButtonUp (0)) {
            inputFlag = false;
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.GetComponent<HellgateMissileEx> () != null) {
            if (!dieFlag) {
                manager.Die ();
            }
            dieFlag = true;
        }
    }
}
