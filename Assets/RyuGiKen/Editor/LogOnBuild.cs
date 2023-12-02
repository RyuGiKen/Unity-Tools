using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using RyuGiKen;
namespace RyuGiKenEditor.Tools
{
    /// <summary>
    /// 构建时生成Version文本
    /// </summary>
    public class LogOnBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder { get; private set; }
        private DateTime m_startTime;
        public void OnPreprocessBuild(BuildReport report)
        {
            m_startTime = DateTime.Now;
        }
        public void OnPostprocessBuild(BuildReport report)
        {
            var endTime = DateTime.Now;
            var deltaTime = endTime - m_startTime;
            Debug.LogFormat("构建耗时：{0}:{1}:{2}:{3}", deltaTime.Hours.ToString("D2"), deltaTime.Minutes.ToString("D2"), deltaTime.Seconds.ToString("D2"), deltaTime.Milliseconds.ToString("D3"));

            FileInfo outputExe = new FileInfo(report.summary.outputPath);
            string xmlPath = outputExe.Directory.FullName + "\\" + outputExe.GetFileNameWithOutType() + "_Data\\BuildData.xml";
            GetFile.CreateXmlFile(xmlPath, "Root",
                GetFile.CreateXElement("BuildDate", report.summary.buildEndedAt.ToLocalTime().ToString("yyyy_MM_dd")),
                GetFile.CreateXElement("BuildTime", report.summary.buildEndedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")),
                GetFile.CreateXElement("BuildUnityVersion", RyuGiKen.Tools.Version.GetProgramUnityVersion())
            );
            string VersionFilePath = outputExe.Directory.FullName + "\\" + outputExe.GetFileNameWithOutType() + "_Data\\StreamingAssets";
            if (!Directory.Exists(VersionFilePath))
                Directory.CreateDirectory(VersionFilePath);
            VersionFilePath += "\\Version.txt";
            FileStream file = new FileStream(VersionFilePath, FileMode.OpenOrCreate);
            file.Close();
            File.WriteAllText(VersionFilePath, RyuGiKen.Tools.Version.LogVersionDate(), Encoding.UTF8);
        }
    }
}