using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement;
using WingjoyFramework.UIFramework.Runtime.Enums;
using WingjoyFramework.UIFramework.Runtime.Managers;
using ResourceManager = WingjoyFramework.UIFramework.Runtime.Managers.ResourceManager;

namespace WingjoyFramework.UIFramework.Runtime.Views
{
    public class BaseView
    {
        protected CanvasGroup _canvasGroup;
        protected UIViewType _viewType;
        protected Transform _transform;     
        protected Dictionary<string,Action<object>> _messageRegistered;
        public string Path
        {
            get;set;
        }

        public BaseView(Transform transform,UIViewType viewType)
        {
            _transform = transform;
            _viewType = viewType;
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        public virtual void Init()
        {
            _canvasGroup = _transform.GetComponent<CanvasGroup>();
            _messageRegistered = new Dictionary<string, Action<object>>();
        }

        /// <summary>
        /// 界面显示
        /// </summary>
        public virtual void OnStart()
        {
            _transform.gameObject.SetActive(true);
            //设置最上层显示
            _transform.SetAsLastSibling();
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// 界面暂停交互
        /// </summary>
        public virtual void OnPause()
        {
            _canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 界面继续交互
        /// </summary>
        public virtual void OnResume()
        {
            _canvasGroup.blocksRaycasts = true;
        }
        /// <summary>
        /// 关闭界面
        /// </summary>
        public virtual void OnExit()
        {
            _transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// 销毁界面
        /// </summary>
        public virtual void OnDestroy()
        {
            GameObject.DestroyImmediate(_transform);
            ResourceManager.ReleaseAsset(Path);
            foreach (var message in _messageRegistered)
            {
                MessageManager.RemoveMsgListener(message.Key, message.Value);
            }
        }

        public void ReceiveMessage(string messageName,Action<object> eventHandler)
        {
            MessageManager.RegisterMsgListener(messageName,eventHandler);
            _messageRegistered.Add(messageName,eventHandler);

        }
        public void SendMessage(string messageName, object value)
        {
            MessageManager.SendMessage(messageName, value);
        }
    }
}
