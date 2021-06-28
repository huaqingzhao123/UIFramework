using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingjoyFramework.UIFramework.Runtime.Utility;

namespace WingjoyFramework.UIFramework.Runtime.Configs
{
    //UI配置
    [Serializable]
    public class ViewConfig : GlobalConfig<ViewConfig>
    {
        [LabelText("UI列表"), ShowInInspector]
        public List<UIConfig> viewConfigs = new List<UIConfig>();
        [LabelText("文件名")]
        public string Name;
        [Serializable]
        public struct UIConfig
        {
            [LabelText("UI类型")]
            public string ViewType;
            [LabelText("预制体路径")]
            public string Path;
        }
#if UNITY_EDITOR
        [Button("保存修改", ButtonSizes.Medium)]
        public void SaveConfig()
        {
            if (Name == default || Name == "")
            {
                Name = "UI配置";
            }
            JsonHelper.SerializeToJson(this, Application.dataPath + $"/Resource/Configs/Views/{Name}.json");
        }

        public void AddView(string ViewType, string path)
        {
            bool isContain=false;
            int index=0;
            for (int i = 0; i < viewConfigs.Count; i++)
            {
                if (viewConfigs[i].ViewType == ViewType || viewConfigs[i].Path == path)
                {
                    isContain = true;
                    index = i;
                    break;
                }
            }
          if(isContain) viewConfigs.RemoveAt(index);
            viewConfigs.Add(new UIConfig { ViewType = ViewType, Path = path });
        }
#endif
    }
}
