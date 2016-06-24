//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

namespace Hellgate
{
    [RequireComponent (typeof(AudioSource))]
    public class Sound : MonoBehaviour
    {
        protected AudioSource source;

        /// <summary>
        /// Play the specified clip.
        /// </summary>
        /// <param name="clip">Clip.</param>
        public void Play (AudioClip clip)
        {
            if (source == null) {
                source = gameObject.GetComponent<AudioSource> ();
            }

            gameObject.name = clip.name;
            source.clip = clip;
            source.Play ();
            StartCoroutine (DelayDespawn ());
        }

        protected IEnumerator DelayDespawn ()
        {
            yield return new WaitForSeconds (source.clip.length + 0.2f);
            gameObject.SetActive (false);
        }
    }
}
