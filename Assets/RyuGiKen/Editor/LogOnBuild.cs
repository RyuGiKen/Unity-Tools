﻿using System;
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
            LogVersion(report.summary);
        }
        void LogVersion(BuildSummary summary)
        {
            var endTime = DateTime.Now;
            var deltaTime = endTime - m_startTime;
            Debug.LogFormat("构建耗时：{0}:{1}:{2}:{3}", deltaTime.Hours.ToString("D2"), deltaTime.Minutes.ToString("D2"), deltaTime.Seconds.ToString("D2"), deltaTime.Milliseconds.ToString("D3"));
            if (summary.platformGroup != BuildTargetGroup.Standalone)
                return;
            FileInfo outputExe = new FileInfo(summary.outputPath);
            string dataPath = outputExe.Directory.FullName + "\\" + outputExe.GetFileNameWithOutType() + "_Data";
            string xmlPath = dataPath + "\\BuildData.xml";
            GetFile.CreateXmlFile(xmlPath, "Root",
                GetFile.CreateXElement("BuildDate", summary.buildEndedAt.ToLocalTime().ToString("yyyy_MM_dd")),
                GetFile.CreateXElement("BuildTime", summary.buildEndedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")),
                GetFile.CreateXElement("BuildSize", summary.totalSize.ToString()),
                GetFile.CreateXElement("BuildUnityVersion", RyuGiKen.Tools.Version.GetProgramUnityVersion())
            );
            string VersionFilePath = dataPath + "\\StreamingAssets";
            if (!Directory.Exists(VersionFilePath))
                Directory.CreateDirectory(VersionFilePath);
            VersionFilePath += "\\Version.txt";
            FileStream file = new FileStream(VersionFilePath, FileMode.OpenOrCreate);
            file.Close();
            File.WriteAllText(VersionFilePath, RyuGiKen.Tools.Version.LogVersionDate(), Encoding.UTF8);
            DirectoryInfo dataDirectoryInfo = new DirectoryInfo(dataPath);
            dataDirectoryInfo.LastWriteTime = DateTime.Now;//刷新输出文件夹的修改时间
        }
    }
}