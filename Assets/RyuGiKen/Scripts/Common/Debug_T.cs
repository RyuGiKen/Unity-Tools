using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        public static Debug_T instance;
        [Tooltip("允许收录")] public static bool CanAdd = true;
        [Tooltip("允许打印")] public static bool CanPrint = false;
        public static string Path = Application.streamingAssetsPath;
        [Tooltip("打印队列长度")] public int ListCount = 0;
        [Tooltip("打印队列")] public static List<string> LogList = new List<string>();
        Thread th;
        static StreamWriter sw;
        private void Awake()
        {
            if (!instance)
                instance = this;
            if (instance != this)
                Destroy(this);
            Clear();
            LogList.Clear();
            th = new Thread(new ThreadStart(Print));
            th.Start();
        }
        void Start()
        {
#if UNITY_EDITOR
            CanPrint = false;//编辑器状态打印可能导致循环导入
            CanAdd = false;
#endif
        }
        void Update()
        {
            ListCount = LogList.Count;
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
        void Print()
        {
            while (true)
            {
                if (CanPrint && LogList.Count > 0)
                {
                    try
                    {
                        //StreamWriter sw = new StreamWriter(Path + "/Log.txt", true);
                        int i = 0;
                        //for (int i = 0; i < ((LogList.Count < 10) ? LogList.Count : 10); i++)
                        {
                            if (LogList[i] != "" && LogList[i] != null)
                            {
                                string fileTitle = "[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]";
                                sw.WriteLine(fileTitle);
                                //开始写入
                                sw.WriteLine(LogList[i] + "\r\n");
                                //清空缓冲区
                                sw.Flush();
                                //关闭流
                                //sw.Close();
                            }
                            LogList.RemoveAt(i);
                        }
                    }
                    catch { }
                }
                Thread.Sleep(1);
            }
        }
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="Content"></param>
        public static void Log(string Content)
        {
            if (CanAdd)
                LogList.Add(Content);
            Debug.Log(Content);
        }
        public static void LogError(string Content)
        {
            if (CanAdd)
                LogList.Add(Content);
            Debug.LogError(Content);
        }
        /// <summary>
        /// 清空日志
        /// </summary>
        public static void Clear()
        {
            bool originEnable = CanPrint;
            CanPrint = false;//清空前需暂停打印
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
            File.WriteAllText(Path + "/Log.txt", string.Empty);
            CanPrint = originEnable;
            if (sw != null)
                sw.Close();
            sw = new StreamWriter(Path + "/Log.txt", true);
        }
        private void OnApplicationQuit()
        {
            if (th != null)
                th.Abort();
            if (sw != null)
            {
                sw.Flush();
                sw.Close();
            }
        }
    }
}
