using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    public class HttpData
    {
#region Const

        private const int DEFAULT_RETRY = 1;

#endregion

#region Static

        /// <summary>
        /// Set base url.
        /// </summary>
        public static string BASE_URL = "";
        /// <summary>
        /// Set default data.
        /// </summary>
        public static Dictionary<string, string> defaultData = null;

#endregion

        public string url;
        public Dictionary<string, string> data;
        public Http.FinishedDelegate finishedDelegate = null;
        public int retry = DEFAULT_RETRY;
        public float timeout = 0;
        public bool post = false;
        public bool popUp = true;

        private void Init (string url)
        {
            if (!url.Contains ("http")) {
                this.url += BASE_URL + "/";
            }

            this.url += url;
            data = defaultData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.HttpData"/> class.
        /// </summary>
        /// <param name="u">Url.</param>
        public HttpData (string url)
        {
            Init (url);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.HttpData"/> class.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="extension">Extension.</param>
        public HttpData (string url, string extension)
        {
            Init (url + "." + extension);
        }
    }
}
