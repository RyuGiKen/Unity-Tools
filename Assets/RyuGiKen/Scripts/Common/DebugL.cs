using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 打印日志。
    /// </summary>
    [DisallowMultipleComponent]
    public class DebugL : MonoBehaviour
    {
        public static DebugL instance;
        [Tooltip("允许收录")] public static bool CanAdd = true;
        [Tooltip("允许打印")] public static bool CanPrint = false;
        public static string Path = Application.streamingAssetsPath;
        [Tooltip("打印队列长度")] public int ListCount = 0;
        [Tooltip("打印队列")] public static List<string> LogList = new List<string>();
        protected Thread th;
        protected static StreamWriter sw;
        protected virtual void Awake()
        {
            if (!instance)
                instance = this;
            if (instance != this)
                Destroy(this);
            Clear();
            LogList = new List<string>();
        }
        protected virtual void OnEnable()
        {
            th = new Thread(new ThreadStart(Print));
            th.Start();
        }
        protected virtual void Start()
        {
            CanPrint = true;
#if UNITY_EDITOR
            CanPrint = false;//编辑器状态打印可能导致循环导入
            CanAdd = false;
#endif
        }
        protected virtual void Update()
        {
            ListCount = LogList.Count;
        }
        protected virtual void LateUpdate()
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
        protected void Print()
        {
            while (true)
            {
                if (CanPrint && LogList.Count > 0)
                {
                    if (sw == null)
                        CheckStreamWriter();
                    try
                    {
                        //StreamWriter sw = new StreamWriter(Path + "/Log.txt", true);
                        int i = 0;
                        //for (int i = 0; i < ((LogList.Count < 10) ? LogList.Count : 10); i++)
                        {
                            if (!string.IsNullOrEmpty(LogList[i]))
                            {
                                Print(sw, LogList[i], true);
                            }
                            LogList.RemoveAt(i);
                        }
                    }
                    catch { }
                }
                Thread.Sleep(1);
            }
        }
        protected static void Print(StreamWriter sw, string str, bool printTime)
        {
            if (sw != null)
            {
                string fileTitle = printTime ? ("[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "]") : "";
                sw.WriteLine(fileTitle);
                //开始写入
                sw.WriteLine(str + "\r\n");
                //清空缓冲区
                sw.Flush();
                //关闭流
                //sw.Close();
            }
        }
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="Content"></param>
        public static void Log(string Content, bool Force = false)
        {
            Debug.Log(Content);
            if (instance)
            {
                if (CanAdd && LogList != null)
                    LogList.Add(Content);
            }
            else if (Force)
            {
                CheckStreamWriter();
                Print(sw, Content, true);
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
            }
        }
        public static void LogError(string Content, bool Force = false)
        {
            Debug.LogError(Content);
            if (instance)
            {
                if (CanAdd && LogList != null)
                    LogList.Add(Content);
            }
            else if (Force)
            {
                CheckStreamWriter();
                Print(sw, Content, true);
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
            }
        }
        /// <summary>
        /// 清空日志
        /// </summary>
        public static void Clear()
        {
            bool originEnable = CanPrint;
            CanPrint = false;//清空前需暂停打印
            SetPath();
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
            File.WriteAllText(Path + "/Log.txt", string.Empty, Encoding.UTF8);
            CanPrint = originEnable;
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
            CheckStreamWriter();
        }
        protected static void SetPath()
        {
            Path = Application.streamingAssetsPath;
        }
        protected static void CheckStreamWriter()
        {
            if (sw == null)
            {
                SetPath();
                sw = new StreamWriter(Path + "/Log.txt", true, Encoding.UTF8);
            }
        }
        protected virtual void OnDisable()
        {
            try
            {
                if (th != null)
                    th.Abort();
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
                Debug.Log("停止打印");
            }
            catch (System.Exception e)
            {
                Debug.LogError("失败 " + e);
            }
        }
    }
}
