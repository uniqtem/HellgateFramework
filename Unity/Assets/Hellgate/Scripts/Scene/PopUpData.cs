//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//					Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
using UnityEngine;
using System.Collections;

namespace Hellgate
{
    /// <summary>
    /// Pop up type.
    /// </summary>
    public enum PopUpType
    {
        Ok = 1,
        YesAndNo
    }

    /// <summary>
    /// Pop up Yes and No type.
    /// </summary>
    public enum PopUpYNType
    {
        Yes = 1,
        No
    }

    public class PopUpData
    {
        public string Title;
        public PopUpType Type;

        public PopUpData (string title, PopUpType type)
        {
            Title = title;
            Type = type;
        }
    }
}
