//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

namespace Hellgate
{
	public class PopUpController : SceneController
	{
		public delegate void FinishedDelegate (PopUpYNType type);
		
		public FinishedDelegate finishedDelegate;
		[SerializeField]
		private GameObject
			text;
		[SerializeField]
		GameObject
			buttonOk;
		[SerializeField]
		GameObject
			buttonYes;
		[SerializeField]
		GameObject
			buttonNo;
		private PopUpType type;
		private PopUpYNType ynType;
		
		public override void OnSet (object data)
		{
			PopUpData popupData = (PopUpData)data;
			
			SetText (text, popupData.Title);
			type = popupData.Type;
			
			buttonYes.SetActive (type == PopUpType.YesAndNo);
			buttonNo.SetActive (type == PopUpType.YesAndNo);
			buttonOk.SetActive (type == PopUpType.Ok);
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			if (finishedDelegate != null) {
				finishedDelegate (ynType);
			}
		}

		public override void OnKeyBack ()
		{
			if (type == PopUpType.Ok) {
				OnOkButtonTap ();
			} else {
				OnNoButtonTap ();
			}
		}
		
		public void OnYesButtonTap ()
		{
			ynType = PopUpYNType.Yes;
			SceneManager.Instance.Close ();
		}
		
		public void OnNoButtonTap ()
		{
			ynType = PopUpYNType.No;
			SceneManager.Instance.Close ();
		}
		
		public void OnOkButtonTap ()
		{
			ynType = default (PopUpYNType);
			SceneManager.Instance.Close ();
		}
		
		private void SetText (GameObject go, string text)
		{
			Component nGuiLabel = go.GetComponent ("UILabel");
			if (nGuiLabel != null) {
				SetColorReflection (nGuiLabel, text);
			}
		}
		
		private void SetColorReflection (Component comp, string text)
		{
			SSceneReflection.SetPropValue (comp, "text", text);
		}
	}
}
