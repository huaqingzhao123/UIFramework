using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WingjoyFramework.UIFramework.Runtime.Utility
{
        public class UIPathFind
        {
            /// <summary>
            /// 获取Transform路径
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <returns>Transform路径</returns>
            public static string GetPath(Transform transform)
            {
                return transform ? GetPath(transform.parent) + "/" + transform.gameObject.name : "";
            }

            /// <summary>
            /// 获取Transform路径移除Canvas（Environment）
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <returns>Transform路径</returns>
            public static string GetPathWithoutCanvasEnvironment(Transform transform)
            {
                return GetPath(transform).Replace("/Canvas (Environment)", "").TrimStart('/');
            }
        }
}