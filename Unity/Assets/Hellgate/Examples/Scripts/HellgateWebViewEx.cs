using UnityEngine;
using System.Collections;
using Hellgate;

using UnityEngine.UI;

public class HellgateWebViewEx : HellgateSceneControllerEx
{
    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private GameObject forwardButton;
    private string url;

    public override void OnSet (object data)
    {
        base.OnSet (data);

        url = data as string;

        WebViewManager.Instance.ProgressReceivedEvent += OnProgress;
        WebViewManager.Instance.ErrorReceivedEvent += OnError;
        WebViewManager.Instance.LoadURL (url, 50, 150, 50, 50);

        SetButton (backButton, false);
        SetButton (forwardButton, false);
    }

    private void SetButton (GameObject button, bool flag)
    {
        SetButtonEnabledValue (button, flag);

        byte alpha = 255;
        if (!flag) {
            alpha = 128;
        }
        SetButtonDefaultColor (button, new Color32 (255, 255, 255, alpha));
    }

    private void OnProgress (int progress)
    {
        if (progress >= 100) {
            SetButton (backButton, WebViewManager.Instance.CanGoBack ());
            SetButton (forwardButton, WebViewManager.Instance.CanGoForward ());
        }

        HDebug.Log ("OnProgress : " + progress);
    }

    private void OnError (string message)
    {
        HDebug.Log ("OnError : " + message);
    }

    public void OnClick ()
    {
        WebViewManager.Instance.ProgressReceivedEvent -= OnProgress;
        WebViewManager.Instance.ErrorReceivedEvent -= OnError;
        WebViewManager.Instance.Destroy ();

        OnClickClose ();
    }

    public void OnClickBack ()
    {
        WebViewManager.Instance.GoBack ();
    }

    public void OnClickForward ()
    {
        WebViewManager.Instance.GoForward ();
    }

    public void OnClickReload ()
    {
        WebViewManager.Instance.Reload ();
    }
}
