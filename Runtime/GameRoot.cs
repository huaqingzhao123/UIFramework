using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingjoyFramework.UIFramework.Runtime.Managers;

namespace WingjoyFramework.UIFramework.Runtime
{
    /// <summary>
    /// 游戏入口，持有游戏所有的单例
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
       public UIManager SingleUIManager
        {
            get;private set;
        }
        private void Awake()
        {
            SingleUIManager = UIManager.Instance;
        }
    }
}

