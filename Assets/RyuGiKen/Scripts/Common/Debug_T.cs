using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 打印日志。
    /// </summary>
    [DisallowMultipleComponent]
    public class Debug_T : MonoBehaviour
    {
        public static bool Enable = true;
        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))//切换是否打印日志
            {
                Enable = !Enable;
            }
            if (Input.GetKeyDown(KeyCode.F6))//清空日志
            {
                Clear();
            }
        }
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="Content"></param>
        public static void Log(string Content)
        {
            if (Enable)
            {
                StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/Log.txt", true);
                string fileTitle = "[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]";
                sw.WriteLine(fileTitle);
                //开始写入
                sw.WriteLine(Content + "\r\n");
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
            }
        }
        /// <summary>
        /// 清空日志
        /// </summary>
        public static void Clear()
        {
            File.WriteAllText(Application.streamingAssetsPath + "/Log.txt", string.Empty);
        }
    }
}
