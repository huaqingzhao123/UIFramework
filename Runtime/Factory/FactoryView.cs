namespace WingjoyFramework.UIFramework.Runtime.Factory
{
    using System;
    using UnityEngine;
    using System.Collections;
    using WingjoyFramework.UIFramework.Runtime.Enums;
    using WingjoyFramework.UIFramework.Runtime.Views;
    
    
    /// View工厂类---------------当前文件自动生成，禁止修改
    public class FactoryView
    {
        
        public static WingjoyFramework.UIFramework.Runtime.Views.BaseView baseView;
        
        public static WingjoyFramework.UIFramework.Runtime.Views.BaseView CreateView(UnityEngine.Transform transform, WingjoyFramework.UIFramework.Runtime.Enums.UIViewType uIViewType)
        {
            if ((uIViewType == UIViewType.GameHall))
            {
                baseView = new WingjoyFramework.UIFramework.Runtime.Views.GameHall(transform, uIViewType);
                return baseView;
            }
            if ((uIViewType == UIViewType.UITestForm))
            {
                baseView = new WingjoyFramework.UIFramework.Runtime.Views.UITestForm(transform, uIViewType);
                return baseView;
            }
            return baseView;
        }
    }
}
