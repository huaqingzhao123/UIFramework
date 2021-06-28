using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace WingjoyFramework.UIFramework.Runtime.Views
{
    public class ViewField : MonoBehaviour
    {
        [ValueDropdown("GetComponentsList", IsUniqueList = true)] 
        public List<Component> Components;

        [LabelText("组合类型")]
        public GroupType GroupType;
        /// <summary>
        /// 获取当前物体所挂载的可本地化组件
        /// </summary>
        /// <returns>可本地化组件列表</returns>
        public IEnumerable GetComponentsList()
        {
            var components = GetComponents<Component>().ToList();
            return components.Where((component => component.GetType() != typeof(ViewField))).Select((component =>
                    new ValueDropdownItem(component.GetType() + " ID:" + component.GetInstanceID(), component)));
        }
    }
    /// <summary>
    /// 绑定类型
    /// </summary>
    public enum GroupType
    {
        [LabelText("默认")]
        Default,
        [LabelText("滑动区域")]
        ScrollArea,
        [LabelText("按钮区域")]
        ButtonArea,
        [LabelText("普通窗口")]
        GeneralWindow,
    }
}