using UnityEngine;
using System.Collections;
using System;

namespace WingjoyFramework.UIFramework.Runtime.Enums
{
    public enum UILoadType
    {
        //可以和其它页面一起正常显示的
        Normal,
        //需要遮挡射线，防止与其它页面交互的
        MaskOther,
        //需要关闭其它页面的
        CloseOther
    }
}
