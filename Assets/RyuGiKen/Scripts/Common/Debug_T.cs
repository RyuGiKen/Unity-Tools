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
        [Tooltip("允许收录")] public static bool CanAdd = true;
        [Tooltip("允许打印")] public static bool CanPrint = false;
        public static string Path = Application.streamingAssetsPath;
        private void Awake()
        {
            Clear();
        }
        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))//切换是否打印日志
            {
                CanPrint = !CanPrint;
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
            if (CanPrint && Content != "" && Content != null)
            {
                StreamWriter sw = new StreamWriter(Path + "/Log.txt", true);
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
            bool originEnable = CanPrint;
            CanPrint = false;//清空前需暂停打印
            File.WriteAllText(Path + "/Log.txt", string.Empty);
            CanPrint = originEnable;
        }
    }
}
