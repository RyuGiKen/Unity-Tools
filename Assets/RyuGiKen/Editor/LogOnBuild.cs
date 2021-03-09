using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
/// <summary>
/// 构建时生成Version文本，自动删除
/// </summary>
public class LogOnBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
	public int callbackOrder { get; private set; }
	private DateTime m_startTime;
	private string VersionFileFullName;
	public void OnPreprocessBuild(BuildReport report)
	{
		m_startTime = DateTime.Now;
		VersionFileFullName = Application.streamingAssetsPath + "/" + "Version.txt";
		//File.Create(VersionFileFullName);
		FileStream file = new FileStream(VersionFileFullName, FileMode.OpenOrCreate);
		file.Close();

		//File.WriteAllText(VersionFileFullName, RyuGiKen.Tools.Version.LogVersionDate());
	}
	public void OnPostprocessBuild(BuildReport report)
	{
		try
		{
			var endTime = DateTime.Now;
			var deltaTime = endTime - m_startTime;
			var hours = deltaTime.Hours.ToString("00");
			var minutes = deltaTime.Minutes.ToString("00");
			var seconds = deltaTime.Seconds.ToString("00");
			Debug.LogFormat("构建耗时：{0}:{1}:{2}", hours, minutes, seconds);
			File.Delete(VersionFileFullName);
			File.Delete(VersionFileFullName + ".meta");
		}
		catch { }
	}
}