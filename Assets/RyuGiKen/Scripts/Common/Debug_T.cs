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
        public static void Log(string Content)
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
}
