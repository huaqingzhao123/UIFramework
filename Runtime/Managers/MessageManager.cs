using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WingjoyFramework.UIFramework.Runtime.Managers
{
    public static class MessageManager
    {
        //消息中心缓存集合
        //<string : 消息名称，DelMessageDelivery 数据执行委托>
        public static Dictionary<string, Action<object>> _dicMessages = new Dictionary<string, Action<object>>();

        /// <summary>
        /// 增加消息的监听。
        /// </summary>
        /// <param name="messageType">消息分类</param>
        /// <param name="handler">消息委托</param>
        public static void RegisterMsgListener(string messageType, Action<object> handler)
        {
            if (!_dicMessages.ContainsKey(messageType))
            {
                _dicMessages.Add(messageType, null);
            }
            _dicMessages[messageType] += handler;
        }

        /// <summary>
        /// 取消消息的监听
        /// </summary>
        /// <param name="messageType">消息分类</param>
        /// <param name="handele">消息委托</param>
        public static void RemoveMsgListener(string messageType, Action<object> handeler)
        {
            if (_dicMessages.ContainsKey(messageType))
            {
                _dicMessages[messageType] -= handeler;
            }

        }

        /// <summary>
        /// 取消所有消息的监听
        /// </summary>
        public static void ClearALLMsgListener()
        {
            if (_dicMessages != null)
            {
                _dicMessages.Clear();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageType">消息的分类</param>
        /// <param name="kv">键值对(对象)</param>
        public static void SendMessage(string messageType, object data)
        {
            Action<object> del;                         //委托

            if (_dicMessages.TryGetValue(messageType, out del))
            {
                //调用委托
                del?.Invoke(data);
            }
        }
    }
}
