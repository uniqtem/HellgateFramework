using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
        /// Set default headers.
        /// </summary>
        public static Dictionary<string, string> defaultHeaders = null;
        /// <summary>
        /// Set default data.
        /// </summary>
        public static Dictionary<string, string> defaultDatas = null;

#endregion

        public string url;
        public Dictionary<string, string> headers;
        public Dictionary<string, string> datas;
        public Http.FinishedDelegate finishedDelegate = null;
        public int retry = DEFAULT_RETRY;
        public float timeout = 0;
        public bool post = false;
        public bool popUp = true;

        private void Init (string url)
        {
            StringBuilder stringBuilder = new StringBuilder ();
            if (!url.Contains ("http")) {
                if (!BASE_URL.EndsWith ("/")) {
                    stringBuilder.AppendFormat ("{0}/", BASE_URL);
                } else {
                    stringBuilder.Append (BASE_URL);
                }
            }
            stringBuilder.Append (url);

            this.url = stringBuilder.ToString ();
            headers = AddStaticData (defaultHeaders);
            datas = AddStaticData (defaultDatas);
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

        private Dictionary<string, string> AddStaticData (Dictionary<string, string> def)
        {
            Dictionary<string, string> dic = null;
            if (def != null) {
                dic = new Dictionary<string, string> ();
                foreach (KeyValuePair<string, string> data in def) {
                    dic.Add (data.Key, data.Value);
                }
            }

            return dic;
        }
    }
}
