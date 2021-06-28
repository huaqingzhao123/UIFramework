using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WingjoyFramework.UIFramework.Runtime.Configs;
using WingjoyFramework.UIFramework.Runtime.Constant;
using WingjoyFramework.UIFramework.Runtime.Enums;
using WingjoyFramework.UIFramework.Runtime.Factory;
using WingjoyFramework.UIFramework.Runtime.Utility;
using WingjoyFramework.UIFramework.Runtime.Views;

namespace WingjoyFramework.UIFramework.Runtime.Managers
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("UIMangerRoot");
                    _instance= obj.AddComponent<UIManager>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        private Transform _UIRoot;
        public Transform UIRoot
        {
            get
            {
                if (_UIRoot == null)
                {
                    _UIRoot = GameObject.Find("Canvas").transform;
                    DontDestroyOnLoad(_UIRoot);
                }
                return _UIRoot;
            }
        }
        /// <summary>
        /// 保存所有的UIPrefab
        /// </summary>
        private Dictionary<UIViewType, BaseView> _allViews;
        private Dictionary<UIViewType, string> _allViewPath;
        private Stack<BaseView> _viewsStack;

        /// <summary>
        /// 私有构造函数，禁止外部实例，只在该类内部调用一次
        /// </summary>
        private UIManager()
        {
            //读取所有UI的配置
            _allViewPath = new Dictionary<UIViewType, string>();
            var viewConfig = JsonHelper.DeserializeFromJson<ViewConfig>(Constants.ViewsConfig);
            //配置缓存到字典中
            foreach (var item in viewConfig.viewConfigs)
            {
                var viewType = (UIViewType)Enum.Parse(typeof(UIViewType), item.ViewType);
                _allViewPath.Add(viewType, item.Path);
            }
        }

        /// <summary>
        /// 把某个页面入栈，  把某个页面显示在界面上
        /// </summary>
        public void PushView(UIViewType viewType)
        {
            if (_viewsStack == null)
                _viewsStack = new Stack<BaseView>();
            LoadView(viewType);
        }
        /// <summary>
        /// 出栈 ，把页面从界面上移除
        /// </summary>
        public void PopView()
        {
            if (_viewsStack == null)
                _viewsStack = new Stack<BaseView>();

            if (_viewsStack.Count <= 0) return;
            //关闭栈顶页面的显示
            BaseView topPanel = _viewsStack.Pop();
            topPanel.OnExit();

        }

        /// <summary>
        /// 加载并初始化面板
        /// </summary>
        /// <returns></returns>
        public void LoadView(UIViewType viewType)
        {
            if (_allViews == null)
            {
                _allViews = new Dictionary<UIViewType, BaseView>();
            }
            BaseView view;
            if (!_allViews.TryGetValue(viewType, out view))
            {
                //如果找不到，那么就找这个面板的prefab的路径，然后去根据prefab去实例化面板
                string path;
                if(!_allViewPath.TryGetValue(viewType,out path))
                {
                    Debug.LogError("找不到指定UI的路径!");
                }
                ResourceManager.LoadAssetSuccessCallBack loadAssetSuccessCallBack = obj =>
                {
                    GameObject instViewObj = GameObject.Instantiate(obj) as GameObject;
                    instViewObj.transform.SetParent(UIRoot, false);
                    //这里通过工厂类实例化对应UI脚本
                    var instView = FactoryView.CreateView(instViewObj.transform, viewType);
                    _allViews.Add(viewType, instView);
                    instView.Init();
                    instView.Path = path;
                    view.OnStart();
                    _viewsStack.Push(view);
                };
                ResourceManager.LoadAssetAsync<GameObject>(path, loadAssetSuccessCallBack);
            }
            else
            {
                view.OnStart();
                _viewsStack.Push(view);
            }

        }


        /// <summary>
        /// 得到BaseView,需要是提前加载过得
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns></returns>
        public BaseView GetView(UIViewType viewType)
        {
            BaseView view;
            if(!_allViews.TryGetValue(viewType, out view))
            {
                Debug.LogError($"界面{viewType.ToString()}未提前加载");
            }
            return view;
        }
        /// <summary>
        /// 更换显示的UI界面
        /// </summary>
        public void ChangeScreenView(UIViewType viewType)
        {
            //关闭所有显示的
            for (int i = 0; i < _viewsStack.Count; i++)
            {
                PopView();
            }
            //显示新的界面
            PushView(viewType);
        }

        /// <summary>
        /// 叠加界面且关闭下层界面交互
        /// </summary>
        public void AddScreenMaskView(UIViewType viewType)
        {
            if (_viewsStack.Count > 0)
            {
                var viewTemp = _viewsStack.Peek();
                viewTemp.OnPause();
            }
            //显示新的界面
            PushView(viewType);
        }

        /// <summary>
        /// 叠加界面且其它界面不受任何影响
        /// </summary>
        public void AddScreenNormalView(UIViewType viewType)
        {
            //显示新的界面
            PushView(viewType);
        }

        /// <summary>
        /// 关闭界面并且恢复下层界面的交互
        /// </summary>
        public void HideScreenViewToResumeSubstrate()
        {
            PopView();
            if (_viewsStack.Count <= 0) return;
            var viewTemp = _viewsStack.Peek();
            viewTemp.OnResume();
        }
    }
}
