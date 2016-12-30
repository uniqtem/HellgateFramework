//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//                  Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#endif

namespace Hellgate
{
    public class Http
    {
#region Const

        /// <summary>
        /// The timeout.
        /// </summary>
        public const float timeout = 7f;
        /// <summary>
        /// Create a UnityWebRequest intended to download an audio clip via HTTP GET and create an AudioClip based on the retrieved data.
        /// </summary>
        public const string kHttpVerbAudioClip = "GET_AUDIO_CLIP";
        /// <summary>
        /// Create a UnityWebRequest intended to download an image via HTTP GET and create a Texture based on the retrieved data.
        /// </summary>
        public const string kHttpVerbTexture = "GET_TEXTURE";

#endregion

#region Delegate

#if UNITY_5_4_OR_NEWER
        public delegate void FinishedDelegate (UnityWebRequest www);
#else
        public delegate void FinishedDelegate (WWW www);
#endif

#endregion

        private FinishedDelegate mOnDone;
        private FinishedDelegate mOnFail;
        private FinishedDelegate mOnDisposed;
        private MonoBehaviour mono;

#if UNITY_5_4_OR_NEWER
        private UnityWebRequest www;
        /// <summary>
        /// Defines the HTTP verb used by this UnityWebRequest, such as GET or POST.
        /// </summary>
        public string method = UnityWebRequest.kHttpVerbGET;
        /// <summary>
        /// Create a UnityWebRequest intended to download an audio clip type.
        /// </summary>
        public AudioType audioType;
#else
        private WWW www;
#endif

        private WWWForm wwwFrom;
        private Dictionary<string, string> mHeaders;
        private string url;
        private float timeOut;
        private bool mDisposed;

        private IEnumerator RequestCoroutine ()
        {
            url = ResetCache (url);

#if UNITY_5_4_OR_NEWER
            switch (method) {
            case UnityWebRequest.kHttpVerbGET:
                www = UnityWebRequest.Get (url);
            break;
            case UnityWebRequest.kHttpVerbPOST:
                www = UnityWebRequest.Post (url, wwwFrom);
            break;
            case kHttpVerbTexture:
                www = UnityWebRequest.GetTexture (url);
            break;
            case kHttpVerbAudioClip:
                www = UnityWebRequest.GetAudioClip (url, audioType);
            break;
            default:
                www = new UnityWebRequest (url);
                www.method = method;
            break;
            }

            yield return www.Send ();
#else
            if (wwwFrom.data.Length > 0) { // POST request
                foreach (var entry in wwwFrom.headers) {
                    mHeaders [System.Convert.ToString (entry.Key)] = System.Convert.ToString (entry.Value);
                }

                www = new WWW (url, wwwFrom.data, mHeaders);
            } else { // GET request
                www = new WWW (url, null, mHeaders);
            }
#endif

            yield return mono.StartCoroutine (CheckTimeout ());

            if (mDisposed) {
                if (mOnDisposed != null) {
                    mOnDisposed (null);
                }
            } else if (System.String.IsNullOrEmpty (www.error)) {
                if (mOnDone != null) {
                    mOnDone (www);
                }
            } else {
                if (mOnFail != null) {
                    mOnFail (www);
                }
            }
        }

        private IEnumerator CheckTimeout ()
        {
            float startTime = Time.time;

            while (!mDisposed && !www.isDone) {
                if (timeOut > 0 && (Time.time - startTime) >= timeOut) {
                    Dispose ();
                    break;
                } else {
                    yield return null;
                }
            }

            yield return null;
        }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers {
            set {
                mHeaders = value;
            }
            get {
                return mHeaders;
            }
        }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public float Timeout {
            set {
                timeOut = value;
            }
            get {
                return timeOut;
            }
        }

        /// <summary>
        /// Sets the on done.
        /// </summary>
        /// <value>The on done.</value>
        public FinishedDelegate OnDone {
            set {
                mOnDone = value;
            }
        }

        /// <summary>
        /// Sets the on fail.
        /// </summary>
        /// <value>The on fail.</value>
        public FinishedDelegate OnFail {
            set {
                mOnFail = value;
            }
        }

        /// <summary>
        /// Sets the on disposed.
        /// </summary>
        /// <value>The on disposed.</value>
        public FinishedDelegate OnDisposed {
            set {
                mOnDisposed = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.Http"/> class.
        /// </summary>
        /// <param name="mB">MonoBehaviour.</param>
        /// <param name="u">Url.</param>
        public Http (MonoBehaviour mB, string u)
        {
            mono = mB;
            url = u;
            mHeaders = new Dictionary<string, string> ();
            wwwFrom = new WWWForm ();
            timeOut = timeout;
            mDisposed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.Http"/> class.
        /// </summary>
        /// <param name="mB">MonoBehaviour.</param>
        /// <param name="u">Url.</param>
        /// <param name="param">Parameter.</param>
        public Http (MonoBehaviour mB, string u, string param)
        {
            mono = mB;
            url = u + param;
            mHeaders = new Dictionary<string, string> ();
            wwwFrom = new WWWForm ();
            timeOut = timeout;
            mDisposed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.Http"/> class.
        /// </summary>
        /// <param name="mB">MonoBehaviour.</param>
        /// <param name="u">Url.</param>
        /// <param name="time">Time.</param>
        public Http (MonoBehaviour mB, string u, float time)
        {
            mono = mB;
            url = u;
            mHeaders = new Dictionary<string, string> ();
            wwwFrom = new WWWForm ();
            timeOut = time;
            mDisposed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hellgate.Http"/> class.
        /// </summary>
        /// <param name="mB">MonoBehaviour.</param>
        /// <param name="u">Url.</param>
        /// <param name="param">Parameter.</param>
        public Http (MonoBehaviour mB, string u, Dictionary<string, string> param)
        {
            mono = mB;
            url = u;

            if (param != null && param.Count > 0) {
                int count = 0;
                var enumerator = param.GetEnumerator ();
                while (enumerator.MoveNext ()) {
                    if (count == 0) {
                        url += "?" + enumerator.Current.Key + "=" + enumerator.Current.Value;
                    } else {
                        url += "&" + enumerator.Current.Key + "=" + enumerator.Current.Value;
                    }

                    count++;
                }
            }

            mHeaders = new Dictionary<string, string> ();
            wwwFrom = new WWWForm ();
            timeOut = timeout;
            mDisposed = false;
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="headerName">Header name.</param>
        /// <param name="value">Value.</param>
        public void AddHeader (string headerName, string value)
        {
            mHeaders.Add (headerName, value);
        }

        /// <summary>
        /// Adds the data.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="value">Value.</param>
        public void AddData (string fieldName, string value)
        {
            wwwFrom.AddField (fieldName, value);
        }

        /// <summary>
        /// Adds the range data.
        /// </summary>
        /// <param name="data">Data.</param>
        public void AddRangeData (Dictionary<string, string> data)
        {
            foreach (KeyValuePair<string, string> pair in data) {
                AddData (pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Adds the binary data.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="contents">Contents.</param>
        public void AddBinaryData (string fieldName, byte[] contents)
        {
            wwwFrom.AddBinaryData (fieldName, contents);
        }

        /// <summary>
        /// Adds the binary data.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="contents">Contents.</param>
        /// <param name="fileName">File name.</param>
        public void AddBinaryData (string fieldName, byte[] contents, string fileName)
        {
            wwwFrom.AddBinaryData (fieldName, contents, fileName);
        }

        /// <summary>
        /// Adds the binary data.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="contents">Contents.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="mimeType">MIME type.</param>
        public void AddBinaryData (string fieldName, byte[] contents, string fileName, string mimeType)
        {
            wwwFrom.AddBinaryData (fieldName, contents, fileName, mimeType);
        }

        /// <summary>
        /// Request this instance.
        /// </summary>
        public void Request ()
        {
            mono.StartCoroutine (RequestCoroutine ());
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Hellgate.Http"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Hellgate.Http"/>. The <see cref="Dispose"/>
        /// method leaves the <see cref="Hellgate.Http"/> in an unusable state. After calling <see cref="Dispose"/>, you must
        /// release all references to the <see cref="Hellgate.Http"/> so the garbage collector can reclaim the memory that the
        /// <see cref="Hellgate.Http"/> was occupying.</remarks>
        public void Dispose ()
        {
            if (www != null && !mDisposed) {
                www.Dispose ();
                mDisposed = true;
            }
        }

        /// <summary>
        /// Checks the connection.
        /// ex)StartCoroutine(checkInternetConnection((isConnected)=>{ // handle connection status here }));
        /// </summary>
        /// <returns>The connection.</returns>
        /// <param name="url">URL.</param>
        /// <param name="action">Action.</param>
        public IEnumerator CheckConnection (string url, Action<bool> action)
        {
#if UNITY_5_4_OR_NEWER
            UnityWebRequest www = new UnityWebRequest (url);
            yield return www.Send ();
#else
            WWW www = new WWW (url);
            yield return www;
#endif

            if (www.error != null) {
                action (false);
            } else {
                action (true);
            }
        }

        /// <summary>
        /// Creates the URL.
        /// </summary>
        /// <returns>The UR.</returns>
        /// <param name="u">string list.</param>
        /// <param name="extension">Extension.</param>
        /// <param name="param">Parameter.</param>
        public static string CreateURL (List<string> u, string extension = null, Dictionary<string, string> param = null)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder ();
            for (int i = 0; i < u.Count; i++) {
                if (i == (u.Count - 1)) {
                    stringBuilder.Append (u [i]);
                } else {
                    stringBuilder.Append (u [i] + "/");
                }
            }

            if (extension != null) {
                stringBuilder.Append ("." + extension);
            }

            if (param != null) {
                int count = 0;
                foreach (KeyValuePair<string, string> keyValuePair in param) {
                    if (count == 0) {
                        stringBuilder.Append ("?" + keyValuePair.Key + "=" + keyValuePair.Value);
                    } else {
                        stringBuilder.Append ("&" + keyValuePair.Key + "=" + keyValuePair.Value);
                    }
                    count++;
                }
            }

            return stringBuilder.ToString ();
        }

        /// <summary>
        /// Resets the cache.
        /// </summary>
        /// <returns>The cache.</returns>
        /// <param name="str">String.</param>
        public static string ResetCache (string str)
        {
            string ran = UnityEngine.Random.Range (0, 1000000).ToString ();

            if (str.Contains ("?")) {
                str += "&" + ran;
            } else {
                str += "?" + ran;
            }

            return str;
        }
    }
}