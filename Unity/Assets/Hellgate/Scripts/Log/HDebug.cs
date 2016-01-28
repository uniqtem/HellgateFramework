//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
    public class HDebug : MonoBehaviour
    {
        private class LogData
        {
            public string condition;
            public string stackTrace;
            public LogType type;
        }

        [SerializeField]
        [Range (0.3f, 1.0f)]
        private float height = 0.3f;
        [SerializeField]
        [Range (0.3f, 1.0f)]
        private float width = 1f;
        [SerializeField]
        private int margin = 10;
        [SerializeField]
        private int fontSize = 14;
        [SerializeField]
        private float backgroundAlpha = 0.5f;
        [SerializeField]
        private Color backgroundColor = Color.black;
        [SerializeField]
        private Color textColor = Color.white;
        [SerializeField]
        private int screenMaxLog = 7;
        private static Queue<LogData> data;
        private GUIStyle container;
        private GUIStyle text;

        protected void Awake ()
        {
            // background
            Texture2D background = new Texture2D (1, 1);
            backgroundColor.a = backgroundAlpha;
            background.SetPixel (0, 0, backgroundColor);
            background.Apply ();
            container = new GUIStyle ();
            container.normal.background = background;
            container.wordWrap = false;
            container.padding = new RectOffset (5, 5, 5, 5);
            container.fontSize = fontSize;

            text = new GUIStyle ();
            text.normal.textColor = textColor;

            data = new Queue<LogData> ();
        }

        protected void Update ()
        {
            while (screenMaxLog < data.Count) {
                data.Dequeue ();
            }
        }

        protected void OnEnable ()
        {
            Application.logMessageReceived += HandlelogMessageReceived;
        }

        protected void OnDisable ()
        {
            Application.logMessageReceived -= HandlelogMessageReceived;
        }

        protected void OnGUI ()
        {
            if (data.Count <= 0) {
                return;
            }

            float w = (Screen.width * width) - (margin * 2);
            float h = (Screen.height * height) - (margin * 2);

            GUILayout.BeginArea (new Rect (margin, margin, w, h), container);

            List<LogData> list = new List<LogData> (data);

            for (int i = list.Count - 1; i >= 0; i--) {
                switch (list [i].type) {
                case LogType.Assert:
                case LogType.Exception:
                case LogType.Error:
                    text.normal.textColor = Color.red;
                break;
                case LogType.Warning:
                    text.normal.textColor = Color.yellow;
                break;
                default:
                    text.normal.textColor = Color.white;
                break;
                }
				
                GUILayout.Label (list [i].condition, text);
                GUILayout.Label (list [i].stackTrace, text);
            }

            GUILayout.EndArea ();
        }

        protected void HandlelogMessageReceived (string condition, string stackTrace, LogType type)
        {
            LogData log = new LogData ();
            log.condition = condition;
            log.stackTrace = stackTrace;
            log.type = type;

            data.Enqueue (log);
        }

        /// <summary>
        /// Log the specified message.
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Log (object message)
        {
            if (Debug.isDebugBuild) {
                Debug.Log (message);
            }
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">Message.</param>
        public static void LogWarning (object message)
        {
            if (Debug.isDebugBuild) {
                Debug.LogWarning (message);
            }
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">Message.</param>
        public static void LogError (object message)
        {
            if (Debug.isDebugBuild) {
                Debug.LogError (message);
            }
        }
    }
}
