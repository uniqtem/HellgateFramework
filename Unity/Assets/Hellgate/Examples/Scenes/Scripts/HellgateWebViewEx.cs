using UnityEngine;
using System.Collections;
using Hellgate;

public class HellgateWebViewEx : SceneController
{
    private string url;

    public override void OnSet (object data)
    {
        base.OnSet (data);

        url = data as string;
    }

    public override void Start ()
    {
        base.Start ();

        WebViewManager.Instance.ProgressReceivedEvent += OnProgress;
        WebViewManager.Instance.ErrorReceivedEvent += OnError;
        WebViewManager.Instance.LoadURL (url, 50, 100, 50, 50);
//        WebViewManager.Instance.SetBackground (false);
    }

    private void OnProgress (int progress)
    {
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
}
