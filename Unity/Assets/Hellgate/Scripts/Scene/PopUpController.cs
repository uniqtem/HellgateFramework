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
			m_Text;
		[SerializeField]
		GameObject
			m_ButtonOk;
		[SerializeField]
		GameObject
			m_ButtonYes;
		[SerializeField]
		GameObject
			m_ButtonNo;
		PopUpType m_Type = PopUpType.Ok;
		
		public override void OnSet (object data)
		{
			PopUpData popupData = (PopUpData)data;
			
			SetText (m_Text, popupData.Title);
			m_Type = popupData.Type;
			
			m_ButtonYes.SetActive (m_Type == PopUpType.YesAndNo);
			m_ButtonNo.SetActive (m_Type == PopUpType.YesAndNo);
			m_ButtonOk.SetActive (m_Type == PopUpType.Ok);
		}

		public override void OnKeyBack ()
		{
			if (m_Type == PopUpType.Ok) {
				OnOkButtonTap ();
			} else {
				OnNoButtonTap ();
			}
		}
		
		public void OnYesButtonTap ()
		{
			SceneManager.Instance.Close (delegate () {
				if (finishedDelegate != null) {
					finishedDelegate (PopUpYNType.Yes);
				}
			});
		}
		
		public void OnNoButtonTap ()
		{
			SceneManager.Instance.Close (delegate () {
				if (finishedDelegate != null) {
					finishedDelegate (PopUpYNType.No);
				}
			});
		}
		
		public void OnOkButtonTap ()
		{
			SceneManager.Instance.Close (delegate () {
				if (finishedDelegate != null) {
					finishedDelegate (default (PopUpYNType));
				}
			});
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
			SSReflection.SetPropValue (comp, "text", text);
		}
	}
}
