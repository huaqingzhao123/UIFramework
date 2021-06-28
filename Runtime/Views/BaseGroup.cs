using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingjoyFramework.UIFramework.Runtime.Managers;

namespace WingjoyFramework.UIFramework.Runtime.Views
{
    public class BaseGroup : MonoBehaviour
    {
        protected Dictionary<string, Action<object>> _messageRegistered;
        private void Start()
        {
            Init();
        }
        public virtual void Init()
        {

        }
        public void ReceiveMessage(string messageName, Action<object> eventHandler)
        {
            MessageManager.RegisterMsgListener(messageName, eventHandler);
            _messageRegistered.Add(messageName, eventHandler);

        }
        public void SendMesg(string messageName, object value)
        {
            MessageManager.SendMessage(messageName, value);
        }
    }

}
