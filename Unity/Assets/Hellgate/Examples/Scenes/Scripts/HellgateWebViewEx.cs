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
        WebViewManager.Instance.LoadURL (url, 50, 100, 50, 50);
    }

    private void OnProgress (int progress)
    {
        Debug.Log ("OnProgress : " + progress);
    }

    public void OnClick ()
    {
        WebViewManager.Instance.ProgressReceivedEvent -= OnProgress;
        WebViewManager.Instance.Destroy ();
        OnClickClose ();
    }
}
