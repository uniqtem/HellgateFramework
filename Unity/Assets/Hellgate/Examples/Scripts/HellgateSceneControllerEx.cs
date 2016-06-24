using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hellgate;

public class HellgateSceneControllerEx : SceneController
{
    private class ReflectionProduct
    {
        public GameObject gObj;
        public string nGUICompName;
        public string nGUIProdName;
        public string uGUICompName;
        public string uGUIProdName;
        public object data;

        public ReflectionProduct (GameObject gObj, object data = null)
        {
            this.gObj = gObj;
            this.data = data;
        }
    }

    private void SetProdValue (ReflectionProduct prod)
    {
        // ngui
        Component component = prod.gObj.GetComponent (prod.nGUICompName);
        if (component != null) {
            Reflection.SetPropInvoke (component, prod.nGUIProdName, prod.data);
        }

        // ugui
        component = prod.gObj.GetComponent (prod.uGUICompName);
        if (component != null) {
            Reflection.SetPropInvoke (component, prod.uGUIProdName, prod.data);
        }
    }

    protected void SetScrollValue (GameObject gObj, float f)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj, f);
        prod.nGUICompName = "UISlider";
        prod.nGUIProdName = "value";
        prod.uGUICompName = "Scrollbar";
        prod.uGUIProdName = "size";

        SetProdValue (prod);
    }

    protected void Set2DSpriteValue (GameObject gObj, Sprite sprite)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj, sprite);
        prod.nGUICompName = "UI2DSprite";
        prod.nGUIProdName = "sprite2D";
        prod.uGUICompName = "Image";
        prod.uGUIProdName = "sprite";

        SetProdValue (prod);
    }

    protected void SetButton2DSpriteValue (GameObject gObj, Sprite sprite)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj, sprite);
        prod.nGUICompName = "UIButton";
        prod.nGUIProdName = "normalSprite2D";
        prod.uGUICompName = "Image";
        prod.uGUIProdName = "sprite";

        SetProdValue (prod);
    }

    protected void SetButtonEnabledValue (GameObject gObj, bool flag)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj, flag);
        prod.nGUICompName = "UIButton";
        prod.nGUIProdName = "enabled";
        prod.uGUICompName = "Button";
        prod.uGUIProdName = "enabled";

        SetProdValue (prod);
    }

    protected void SetButtonDefaultColor (GameObject gObj, Color color)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj, color);
        prod.nGUICompName = "UIButton";
        prod.nGUIProdName = "defaultColor";
        prod.uGUICompName = "Image";
        prod.uGUIProdName = "color";

        SetProdValue (prod);
    }

    protected void SetLabelTextValue (GameObject gObj, string text)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj, text);
        prod.nGUICompName = "UILabel";
        prod.nGUIProdName = "text";
        prod.uGUICompName = "Text";
        prod.uGUIProdName = "text";

        SetProdValue (prod);
    }

    protected void SetInputValue (GameObject gObj, string text)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj, text);
        prod.nGUICompName = "UIInput";
        prod.nGUIProdName = "value";
        prod.uGUICompName = "InputField";
        prod.uGUIProdName = "text";

        SetProdValue (prod);
    }

    protected void SetGridRepositionNow (GameObject gObj, bool flag = true)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj, flag);
        prod.nGUICompName = "UIGrid";
        prod.nGUIProdName = "repositionNow";

        SetProdValue (prod);
    }

    private object GetProdValue (ReflectionProduct prod)
    {
        // ngui
        Component component = prod.gObj.GetComponent (prod.nGUICompName);
        if (component != null) {
            return Reflection.GetPropValue (component, prod.nGUIProdName);
        }

        // ugui
        component = prod.gObj.GetComponent (prod.uGUICompName);
        if (component != null) {
            return Reflection.GetPropValue (component, prod.uGUIProdName);
        }
        
        return null;
    }

    protected string GetInputValue (GameObject gObj)
    {
        ReflectionProduct prod = new ReflectionProduct (gObj);
        prod.nGUICompName = "UIInput";
        prod.nGUIProdName = "value";
        prod.uGUICompName = "InputField";
        prod.uGUIProdName = "text";

        object obj = GetProdValue (prod);
        if (obj == null) {
            return "";
        } else {
            return obj.ToString ();
        }
    }

    protected void SetUI2DSpriteValue (GameObject gObj, List<Sprite> list)
    {
        Transform[] trans = gObj.GetComponentsInChildren<Transform> ();
        for (int i = 0; i < trans.Length; i++) {
            if (trans [i].name == "Sprite" || trans [i].name == "2D Sprite") {
                continue;
            }

            Set2DSpriteValue (trans [i].gameObject, Util.FindSprite (list, trans [i].name));
        }
    }

    protected void SetUI2DSpriteValue (GameObject gObj, List<object> list)
    {
        SetUI2DSpriteValue (gObj, Util.GetListObjects<Sprite> (list));
    }
        
    protected void SetUIButton (GameObject gObj, List<Sprite> list)
    {
        Transform[] trans = gObj.GetComponentsInChildren<Transform> ();
        for (int i = 0; i < trans.Length; i++) {
            if (trans [i].name == "Button") {
                continue;
            }

            SetButton2DSpriteValue (trans [i].gameObject, Util.FindSprite (list, trans [i].name));
        }
    }

    protected void SetUIButton (GameObject gObj, List<object> list)
    {
        SetUIButton (gObj, Util.GetListObjects<Sprite> (list));
    }
}
