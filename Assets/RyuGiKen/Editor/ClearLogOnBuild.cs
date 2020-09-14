using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
/// <summary>
/// 构建时清空日志
/// </summary>
public class ClearLogOnBuild : IPreprocessBuildWithReport
{
	public int callbackOrder { get; private set; }
	public void OnPreprocessBuild(BuildReport report)
	{
		Debug.Log("构建时清空日志");
		File.WriteAllText(Application.streamingAssetsPath + "/Log.txt", string.Empty);
	}
}