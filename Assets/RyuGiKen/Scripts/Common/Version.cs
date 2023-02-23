using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 版本号格式限制
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class Version : MonoBehaviour
    {
        /// <summary>
        /// 版本号位数
        /// </summary>
        public static int VersionLength { get { return Style.Length.Clamp(1); } }
        /// <summary>
        /// 版本号样式
        /// </summary>
        static readonly string[] Style = { "D1", "D1" };
        /// <summary>
        /// 测试版标识
        /// </summary>
        public const string DebugSign = " beta";
        /// <summary>
        /// 发布版标识
        /// </summary>
        public const string ReleaseSign = "v";
        public const VersionSignType SignType = VersionSignType.SamePrefixAndOnlyDebugPostfix;
        /// <summary>
        /// 版本号标识
        /// </summary>
        public enum VersionSignType
        {
            /// <summary>
            /// 无标识
            /// </summary>
            None,
            /// <summary>
            /// 统一前缀
            /// </summary>
            SamePrefix,
            /// <summary>
            /// 前缀，自动识别发布版测试版
            /// </summary>
            BothPrefix,
            /// <summary>
            /// 仅测试版有后缀，发布版无后缀
            /// </summary>
            OnlyDebugPostfix,
            /// <summary>
            /// 后缀，自动识别发布版测试版
            /// </summary>
            BothPostfix,
            /// <summary>
            /// 统一前缀。仅测试版有后缀
            /// </summary>
            SamePrefixAndOnlyDebugPostfix,
        }

        public string versionNumber = "b1.0.0";
        public static string BuildDate = "";
        public static string BuildTime = "";
        void Awake()
        {
            if (Application.isPlaying)
            {
                //UpdateVersionNumber();
            }
            BuildDate = GetFile.LoadXmlData("BuildDate", Application.dataPath + "/BuildData.xml", "Root", true);
            BuildTime = GetFile.LoadXmlData("BuildTime", Application.dataPath + "/BuildData.xml", "Root", true);
        }
        private void LateUpdate()
        {
            if (versionNumber != Application.version || this.GetComponent<Text>().text != Application.version)
                UpdateVersionNumber();
        }
        [ContextMenu("更新版本号赋值")]
        void UpdateVersionNumber()
        {
#if DEVELOPMENT_BUILD
            this.GetComponent<Text>().text = GetProgramVersion(true, out versionNumber);
#else
            versionNumber = SetVersionNumber(Application.version);
            this.GetComponent<Text>().text = versionNumber;
#endif
        }
        /// <summary>
        /// 版本号赋值，格式限制
        /// </summary>
        /// <param name="number"></param>
        public static string SetVersionNumber(string number)
        {
            string versionNumber;
            if (string.IsNullOrWhiteSpace(number))
                versionNumber = "";
            else
                versionNumber = number.Trim(' ');//移除空格
            int FirstNum = versionNumber.FindIndexOfNumInString();//找出第一位数字
            int FirstLetterBehindNum = (FirstNum >= 0 ? versionNumber.Remove(0, FirstNum) : versionNumber).FindIndexOfLetterInString();//找出数字部分后第一位字母
            string type = "";
            switch (SignType)//前缀
            {
                case VersionSignType.BothPrefix:
                    type = DebugSign;
                    if (versionNumber.Length > 0)
                    {
                        if ((FirstNum < 0 ? versionNumber : versionNumber.Substring(0, FirstNum)).ContainIgnoreCase(ReleaseSign))
                            type = ReleaseSign;
                    }
                    break;
                case VersionSignType.SamePrefix:
                case VersionSignType.SamePrefixAndOnlyDebugPostfix:
                    type = ReleaseSign;
                    break;
            }
            //数字部分
            string NumPart;
            string[] num;
            //移除第一位数字前的部分
            if (FirstNum >= 0 && FirstLetterBehindNum >= 0)
                NumPart = versionNumber.Substring(FirstNum, FirstLetterBehindNum);
            else if (FirstNum >= 0)
                NumPart = versionNumber.Substring(FirstNum);
            else
                NumPart = versionNumber;
            num = NumPart.Split('.');
            int[] ver = new int[VersionLength];
            for (int i = 0; i < num.Length; i++)
            {
                //Debug.Log(num[i]);
                if (i < VersionLength)
                    int.TryParse(num[i], out ver[i]);//能转换整数才读取
                else
                    break;
            }
            switch (SignType)//后缀
            {
                case VersionSignType.BothPostfix:
                    if (versionNumber.Substring(FirstNum < 0 ? 0 : FirstNum).ContainIgnoreCase(ReleaseSign))
                        type = ReleaseSign;
                    else
                        type = DebugSign;
                    break;
                case VersionSignType.OnlyDebugPostfix:
                case VersionSignType.SamePrefixAndOnlyDebugPostfix:
                    if (FirstLetterBehindNum >= 0)
                        type = DebugSign;
                    break;
            }
            switch (SignType)//前缀
            {
                case VersionSignType.SamePrefix:
                case VersionSignType.BothPrefix:
                case VersionSignType.SamePrefixAndOnlyDebugPostfix:
                    versionNumber = type;
                    break;
                case VersionSignType.None:
                case VersionSignType.BothPostfix:
                case VersionSignType.OnlyDebugPostfix:
                    versionNumber = "";
                    break;
            }
            for (int i = 0; i < ver.Length; i++)
            {
                string style;
                if (Style == null || Style.Length < 1)
                {
                    style = "D0";
                }
                else if (i >= Style.Length)
                {
                    style = Style[Style.Length - 1];
                }
                else
                {
                    style = Style[i];
                }
                if (i == 0) //第一位
                {
                    versionNumber += ver[0].ToString(style);
                }
                else
                {
                    versionNumber += "." + ver[i].ToString(style);
                }
            }
            switch (SignType)//后缀
            {
                case VersionSignType.OnlyDebugPostfix:
                case VersionSignType.BothPostfix:
                    versionNumber += type;
                    break;
                case VersionSignType.SamePrefixAndOnlyDebugPostfix:
                    versionNumber += DebugSign;
                    break;
            }
            return versionNumber;
        }
        /// <summary>
        /// 获取程序名称
        /// </summary>
        /// <returns></returns>
        public static string GetProgramName()
        {
            return Application.productName;
        }
        /// <summary>
        /// 获取程序发布版本号
        /// </summary>
        /// <returns></returns>
        public static string GetProgramVersion(bool includeBuildTime = false)
        {
            return GetProgramVersion(includeBuildTime, out string version);
        }
        /// <summary>
        /// 获取程序发布版本号
        /// </summary>
        /// <returns></returns>
        public static string GetProgramVersion(bool includeBuildTime, out string version)
        {
            version = SetVersionNumber(Application.version);
            return version + (includeBuildTime && !string.IsNullOrWhiteSpace(BuildDate) ? ("_" + BuildDate) : "");
        }
        /// <summary>
        /// 获取程序名称，发布版本号
        /// </summary>
        /// <returns></returns>
        public static string GetProgramNameVersion()
        {
            string Info = "";
            Info += GetProgramName();
            Info += " " + GetProgramVersion();
            return Info;
        }
        /// <summary>
        /// 获取程序Unity版本号
        /// </summary>
        /// <returns></returns>
        public static string GetProgramUnityVersion()
        {
            return "Unity " + Application.unityVersion;
        }
        /// <summary>
        /// 获取程序名称，发布版本号，Unity版本号
        /// </summary>
        /// <returns></returns>
        public static string GetProgramFullVersion()
        {
            string Info = GetProgramNameVersion();
            Info += " with " + GetProgramUnityVersion();
            return Info;
        }
        /// <summary>
        /// 打包时记录程序名称，发布版本号，Unity版本号，打包日期
        /// </summary>
        /// <returns></returns>
        public static string LogVersionDate()
        {
            string Info = "";
            Info += "程序名称：" + GetProgramName() + "\r\n";
            Info += "发布版本：" + GetProgramVersion() + "\r\n";
            Info += "构建版本：" + GetProgramUnityVersion() + "\r\n";
            Info += "打包日期：" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
            return Info;
        }
        /// <summary>
        /// 获取程序名称，发布版本号，Unity版本号，运行环境
        /// </summary>
        /// <returns></returns>
        public static string GetProgramFullVersionEnvironment()
        {
            string Info = GetProgramFullVersion();
            Info += " in " + Application.platform;
            if (System.IntPtr.Size == 4)
                Info += " 32 Bit";
            else if (System.IntPtr.Size == 8)
                Info += " 64 Bit";
            return Info;
        }
    }
}
