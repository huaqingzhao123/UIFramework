using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace  WingjoyFramework.UIFramework.Runtime.Utility
{
    public static class JsonHelper
    {
        public static void SerializeToJson<T>(T obj, string path)
        {
            var text = JsonUtility.ToJson(obj);
            File.WriteAllText(path,text);
        }
        public static T DeserializeFromJson<T>(string path)
        {
            var fileInfo = new FileInfo(path);
            T obj = default(T);
            if (fileInfo.Exists)
            {
                obj = JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            else
            {
                Debug.LogError($"路径:{path}不存在");
            }

            return obj;
        }
    }

}
