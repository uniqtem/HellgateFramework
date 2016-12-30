//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    [RequireComponent (typeof(AudioListener))]
    [RequireComponent (typeof(AudioSource))]
    [ExecuteInEditMode]
    public class SoundManager : MonoBehaviour
    {
#region Const

        protected const string soundManager = "SoundManager";

#endregion

#region Singleton

        private static SoundManager instance = null;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static SoundManager Instance {
            get {
                if (instance == null) {
                    SoundManager manager = GameObject.FindObjectOfType<SoundManager> ();
                    if (manager == null) {
                        GameObject gObj = new GameObject ();
                        gObj.name = soundManager;
                        instance = gObj.AddComponent<SoundManager> ();
                    } else {
                        instance = manager;
                    }

                    DontDestroyOnLoad (instance.gameObject);
                }

                return instance;
            }
        }

#endregion

        [SerializeField]
        protected AudioSource bgm;
        [SerializeField]
        protected AudioSource sound;
        [SerializeField]
        protected List<SoundData> bgms;
        [SerializeField]
        protected List<SoundData> sounds;
        protected ObjectPool objectPool;

        public AudioSource BGM {
            get {
                return bgm;
            }
        }

        /// <summary>
        /// Gets the sound.
        /// </summary>
        /// <value>The sound.</value>
        public AudioSource Sound {
            get {
                return sound;
            }
        }

        /// <summary>
        /// Gets the BGM.
        /// </summary>
        /// <value>The background ms.</value>
        public List<SoundData> BGMs {
            get {
                return bgms;
            }
        }

        /// <summary>
        /// Gets the sounds.
        /// </summary>
        /// <value>The sounds.</value>
        public List<SoundData> Sounds {
            get {
                return sounds;
            }
        }

        void Awake ()
        {
            if (Application.isPlaying) {
                instance = this;
                DontDestroyOnLoad (gameObject);
            }

            bgm = gameObject.GetComponent<AudioSource> ();
            if (bgm == null) {
                bgm = gameObject.AddComponent<AudioSource> ();
            }
            bgm.loop = true;

            if (sound == null) {
                GameObject gObj = new GameObject ();
                gObj.name = "Sound";
                gObj.transform.parent = transform;
                gObj.AddComponent<Sound> ();

                sound = gObj.GetComponent<AudioSource> ();
                sound.playOnAwake = false;
            }
        }

        /// <summary>
        /// Plaies the BGM.
        /// </summary>
        /// <param name="name">Name.</param>
        public void PlayBGM (string name = "")
        {
            if (bgms == null || bgms.Count <= 0) {
                HDebug.LogWarning ("Please add a " + name + " BGM");
                return;
            }

            SoundData sound = bgms [0];
            if (name != "") {
                sound = bgms.Find (x => x.name == name);
            }

            if (sound == null) {
                HDebug.LogWarning ("Please add a " + name + " BGM");
                return;
            }

            StopBGM ();
            bgm.clip = sound.clip;
            bgm.Play ();
        }

        /// <summary>
        /// Stops the BGM.
        /// </summary>
        public void StopBGM ()
        {
            bgm.Stop ();
        }

        /// <summary>
        /// Plaies the sound.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="createCount">Create count.</param>
        public void PlaySound (string name, int createCount = ObjectPool.createCount)
        {
            if (sounds == null) {
                HDebug.LogWarning ("Please add a " + name + " sound");
                return;
            }

            SoundData sound = sounds.Find (x => x.name == name);
            if (sound == null || sound.clip == null) {
                HDebug.LogWarning ("Please add a " + name + " sound");
                return;
            }

            if (objectPool == null) {
                objectPool = new ObjectPool (this.sound.gameObject, gameObject, createCount);
            }

            objectPool.GetObject ().GetComponent<Sound> ().Play (sound.clip);
        }

        /// <summary>
        /// Stops the sound.
        /// </summary>
        public void StopSound ()
        {
            Sound[] sounds = gameObject.GetComponentsInChildren<Sound> ();
            for (int i = 0; i < sounds.Length; i++) {
                sounds [i].gameObject.SetActive (false);
            }
        }

        /// <summary>
        /// Adds the background.
        /// </summary>
        /// <param name="clip">Clip.</param>
        public void AddBGM (AudioClip clip)
        {
            if (bgms == null) {
                bgms = new List<SoundData> ();
            }

            SoundData sound = bgms.Find (x => x.name == clip.name);
            if (sound != null) {
                bgms.Remove (sound);
            }

            bgms.Add (new SoundData (clip.name, clip));
        }

        /// <summary>
        /// Adds the range background.
        /// </summary>
        /// <param name="clips">Clips.</param>
        public void AddRangeBGM (AudioClip[] clips)
        {
            for (int i = 0; i < clips.Length; i++) {
                AddBGM (clips [i]);
            }
        }

        /// <summary>
        /// Adds the sound.
        /// Support Formats : http://docs.unity3d.com/Manual/AudioFiles.html
        /// </summary>
        /// <param name="clip">Clip.</param>
        public void AddSound (AudioClip clip)
        {
            if (sounds == null) {
                sounds = new List<SoundData> ();
            }

            SoundData sound = sounds.Find (x => x.name == clip.name);
            if (sound != null) {
                sounds.Remove (sound);
            }

            sounds.Add (new SoundData (clip.name, clip));
        }

        /// <summary>
        /// Adds the range sound.
        /// </summary>
        /// <param name="clips">Clips.</param>
        public void AddRangeSound (AudioClip[] clips)
        {
            for (int i = 0; i < clips.Length; i++) {
                AddSound (clips [i]);
            }
        }

        /// <summary>
        /// Destory this instance.
        /// </summary>
        public void Destory ()
        {
            instance = null;
            GameObject.Destroy (gameObject);
        }
    }
}
