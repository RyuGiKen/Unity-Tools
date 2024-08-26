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
        //public static Version instance;
        /// <summary>
        /// 版本号位数
        /// </summary>
        public static int VersionLength { get { return Style.Length.Clamp(1); } }
        /// <summary>
        /// 版本号样式
        /// </summary>
        protected static readonly string[] Style = { "D1", "D1"};
        /// <summary>
        /// 测试版标识
        /// </summary>
        public const string DebugSign = " beta";
        /// <summary>
        /// 发布版标识
        /// </summary>
        public const string ReleaseSign = "v";
        public const VersionSignType SignType = VersionSignType.SamePrefixAndOnlyDebugPostfix;
        public static string BuildDate = "";
        public static string BuildTime = "";
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
        /// <summary>
        /// 外部配置覆盖显示
        /// </summary>
        public static string LoadOutSizeVersion = null;
        protected virtual void Awake()
        {
            if (Application.isPlaying)
            {
                //UpdateVersionNumber();
            }
            BuildDate = GetFile.LoadXmlData("BuildDate", Application.dataPath + "/BuildData.xml", "Root", true);
            BuildTime = GetFile.LoadXmlData("BuildTime", Application.dataPath + "/BuildData.xml", "Root", true);

            string xmlData = GetFile.LoadXmlData("Version", Application.streamingAssetsPath + "/Setting.xml", "Data", true);
            if (!string.IsNullOrWhiteSpace(xmlData))
            {
                LoadOutSizeVersion = SetVersionNumber(xmlData);
                //this.GetComponent<Text>().text = LoadOutSizeVersion;
            }
        }
        protected virtual void LateUpdate()
        {
            if (!string.IsNullOrEmpty(LoadOutSizeVersion))
            {
                this.GetComponent<Text>().text = LoadOutSizeVersion;
                return;
            }

            if (versionNumber != Application.version || this.GetComponent<Text>().text != Application.version)
                UpdateVersionNumber();
        }
        [ContextMenu("更新版本号赋值")]
        protected virtual void UpdateVersionNumber()
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
        /// <returns></returns>
        public static string SetVersionNumber(string number)
        {
            return SetVersionNumber(number, SignType);
        }
        /// <summary>
        /// 版本号赋值，格式限制
        /// </summary>
        /// <param name="number"></param>
        /// <param name="SignType"></param>
        /// <returns></returns>
        public static string SetVersionNumber(string number, VersionSignType SignType, string DebugSign = null, string ReleaseSign = null, int? VersionLength = null)
        {
            string versionNumber;
            if (string.IsNullOrWhiteSpace(number))
                versionNumber = "";
            else
                versionNumber = number.Trim(' ');//移除空格
            int FirstNum = versionNumber.FindIndexOfNumInString();//找出第一位数字
            int FirstLetterBehindNum = (FirstNum >= 0 ? versionNumber.Remove(0, FirstNum) : versionNumber).FindIndexOfLetterInString();//找出数字部分后第一位字母
            string Prefix = "";
            string Postfix = "";
            if (DebugSign == null)
                DebugSign = Version.DebugSign;
            if (ReleaseSign == null)
                ReleaseSign = Version.ReleaseSign;
            switch (SignType)//前缀
            {
                case VersionSignType.BothPrefix:
                    Prefix = DebugSign;
                    if (versionNumber.Length > 0)
                    {
                        if ((FirstNum < 0 ? versionNumber : versionNumber.Substring(0, FirstNum)).ContainIgnoreCase(ReleaseSign))
                            Prefix = ReleaseSign;
                    }
                    break;
                case VersionSignType.SamePrefix:
                case VersionSignType.SamePrefixAndOnlyDebugPostfix:
                    Prefix = ReleaseSign;
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
            int versionLength = VersionLength == null ? Style.Length.Clamp(1) : VersionLength.Value;
            int[] ver = new int[versionLength];
            for (int i = 0; i < num.Length; i++)
            {
                //Debug.Log(num[i]);
                if (i < versionLength)
                    int.TryParse(num[i], out ver[i]);//能转换整数才读取
                else
                    break;
            }
            switch (SignType)//后缀
            {
                case VersionSignType.BothPostfix:
                    if (versionNumber.Substring(FirstNum < 0 ? 0 : FirstNum).ContainIgnoreCase(ReleaseSign))
                        Postfix = ReleaseSign;
                    else
                        Postfix = DebugSign;
                    break;
                case VersionSignType.OnlyDebugPostfix:
                case VersionSignType.SamePrefixAndOnlyDebugPostfix:
                    if (FirstLetterBehindNum >= 0)
                        Postfix = DebugSign;
                    break;
            }
            switch (SignType)//前缀
            {
                case VersionSignType.SamePrefix:
                case VersionSignType.BothPrefix:
                case VersionSignType.SamePrefixAndOnlyDebugPostfix:
                    versionNumber = Prefix;
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
                    versionNumber += Postfix;
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
        public static string GetProgramVersion()
        {
            return SetVersionNumber(Application.version);
        }
        /// <summary>
        /// 获取程序发布版本号
        /// </summary>
        /// <returns></returns>
        public static string GetProgramVersion(bool includeBuildTime)
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
            if (includeBuildTime)
                return AddBuildTime(version);
            else
                return version;
        }
        /// <summary>
        /// 版本号后追加日期
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string AddBuildTime(string version)
        {
            return version + (!string.IsNullOrWhiteSpace(BuildDate) ? ("_" + BuildDate) : "");
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
