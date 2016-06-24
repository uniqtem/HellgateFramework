//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;

namespace Hellgate
{
    [Serializable]
    public class SoundData
    {
        /// <summary>
        /// The name.
        /// </summary>
        [SerializeField]
        public string name;
        /// <summary>
        /// The audio clip.
        /// </summary>
        [SerializeField]
        public AudioClip clip;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.SoundData"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="audio">Audio.</param>
        public SoundData (string name, AudioClip audio)
        {
            this.name = name;
            this.clip = audio;
        }
    }
}