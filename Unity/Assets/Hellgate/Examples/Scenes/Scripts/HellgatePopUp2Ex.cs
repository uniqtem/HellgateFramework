//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;
using Hellgate;

public class HellgatePopUp2Ex : SceneController
{
	[SerializeField]
	private UILabel label;

	public override void OnSet (object data)
	{
		base.OnSet (data);

		label.text = data.ToString ();
	}
}
