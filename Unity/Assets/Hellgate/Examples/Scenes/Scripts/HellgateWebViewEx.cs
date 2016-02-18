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

        WebViewManager.Instance.LoadURL (url, 0, 0, 100, 0);
    }

    public void OnClick ()
    {
        WebViewManager.Instance.Destroy ();
        OnClickClose ();
    }
}
