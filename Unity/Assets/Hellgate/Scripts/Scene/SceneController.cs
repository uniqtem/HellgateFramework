//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hellgate
{
	public class SceneController : SSController
	{
		public override void Awake ()
		{
			base.Awake ();
			
			IsCache = false;
		}

		/// <summary>
		/// Quit the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
		public virtual void Quit (string message)
		{
			SceneManager.Instance.PopUp (message, PopUpType.YesAndNo, delegate(PopUpYNType type) {
				if (type == PopUpYNType.Yes) {
					Application.Quit ();
				}
			});
		}

		/// <summary>
		/// Raises the click close event.
		/// </summary>
		public void OnClickClose ()
		{
			SceneManager.Instance.Close ();
		}

		/// <summary>
		/// Sets the UI2DSprite.
		/// </summary>
		/// <param name="gObj">G object.</param>
		/// <param name="list">List.</param>
		protected void SetUI2DSprite (GameObject gObj, List<Sprite> list)
		{
			UI2DSprite[] sprites = gObj.GetComponentsInChildren<UI2DSprite> ();
			for (int i = 0; i < sprites.Length; i++) {
				if (sprites [i].name == "Sprite" || sprites [i].name == "2D Sprite") {
					continue;
				}

				sprites [i].sprite2D = Util.FindSprite (list, sprites [i].name);
			}
		}

		/// <summary>
		/// Sets the UI2DSprite
		/// </summary>
		/// <param name="gObj">G object.</param>
		/// <param name="list">List.</param>
		protected void SetUI2DSprite (GameObject gObj, List<object> list)
		{
			SetUI2DSprite (gObj, Util.GetListObjects<Sprite> (list));
		}

		/// <summary>
		/// Sets the user UIbutton.
		/// </summary>
		/// <param name="gObj">G object.</param>
		/// <param name="list">List.</param>
		protected void SetUIButton (GameObject gObj, List<Sprite> list)
		{
			UIButton[] buttons = gObj.GetComponentsInChildren<UIButton> ();
			for (int i = 0; i < buttons.Length; i++) {
				if (buttons [i].name == "Button") {
					continue;
				}

				buttons [i].normalSprite2D = Util.FindSprite (list, buttons [i].name);
			}
		}

		/// <summary>
		/// Sets the UIButton.
		/// </summary>
		/// <param name="gObj">G object.</param>
		/// <param name="list">List.</param>
		protected void SetUIButton (GameObject gObj, List<object> list)
		{
			SetUIButton (gObj, Util.GetListObjects<Sprite> (list));
		}

		/// <summary>
		/// Sets the color of the UILabel.
		/// </summary>
		/// <returns>The user interface label color.</returns>
		/// <param name="text">Text.</param>
		/// <param name="color">Color.</param>
		protected string SetUILabelColor (string text, string color)
		{
			return "[" + color + "]" + text + "[-]";
		}
	}
}
