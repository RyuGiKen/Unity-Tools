using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RyuGiKen;
namespace RyuGiKen.Tools
{
	/// <summary>
	/// 保存网格
	/// </summary>
	[DisallowMultipleComponent]
	public class SaveAsset : MonoBehaviour
	{
		public string fileName = "Assets/";
		private void Awake()
		{
			Destroy(this);
		}
		[ContextMenu("保存")]
		public void Save()
		{
#if UNITY_EDITOR
			Mesh mesh = null;
			if (this.GetComponent<SkinnedMeshRenderer>())
			{
				mesh = new Mesh();
				this.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
			}
			else if (this.GetComponent<MeshFilter>())
				mesh = this.GetComponent<MeshFilter>().mesh;
			if (mesh == null)
			{
				Debug.LogError("找不到网格体");
				return;
			}
			string OutputFullName = "";
			string OutputPath = "";
			string OutputFileName = "";

			List<string> tempString = ValueAdjust.SplitPath(fileName).ToList();
			if (tempString[0] != "Assets")
				tempString.Insert(0, "Assets");

			for (int i = 0; i < tempString.Count; i++)
			{

				if (i >= tempString.Count - 1)
				{
					if (fileName[fileName.Length - 1] == '/' || fileName[fileName.Length - 1] == '\\')
					{
						OutputPath += tempString[i] + "/";
						OutputFileName = name;
					}
					else
					{
						OutputFileName = tempString[i];
					}
					if (tempString[tempString.Count - 1].LastIndexOf(".asset", StringComparison.OrdinalIgnoreCase) < 0)
						OutputFileName += ".asset";
					break;
				}
				else
					OutputPath += tempString[i] + "/";
			}
			if (!Directory.Exists(OutputPath))
				Directory.CreateDirectory(OutputPath);
			OutputFullName = OutputPath + OutputFileName;
			Debug.Log(OutputFullName);
			AssetDatabase.CreateAsset(mesh, OutputFullName);
#endif
		}
		/// <summary>
		/// 检查路径输入（Assets文件夹内）
		/// </summary>
		/// <param name="path">输入</param>
		/// <param name="fileType">文件类型</param>
		/// <param name="createPath">如无则创建目录</param>
		/// <param name="OutputPath">输出路径</param>
		/// <param name="OutputFileName">输出文件名</param>
		/// <returns>输出</returns>
		public static string CheckPathInAssets(string path, string fileType, bool createPath, out string OutputPath, out string OutputFileName)
		{
			string OutputFullName = "";
			OutputPath = "";
			OutputFileName = "";

			List<string> tempString = ValueAdjust.SplitPath(path).ToList();
			if (tempString[0] != "Assets")
				tempString.Insert(0, "Assets");

			for (int i = 0; i < tempString.Count; i++)
			{
				if (i >= tempString.Count - 1)
				{
					OutputFileName = tempString[i];
					if (tempString[tempString.Count - 1].LastIndexOf("." + fileType, StringComparison.OrdinalIgnoreCase) < 0)
						OutputFileName += "." + fileType;
					break;
				}
				else
					OutputPath += tempString[i] + "/";
			}
			OutputFullName = OutputPath + OutputFileName;
			//Debug.Log(OutputFullName);

			if (!Directory.Exists(OutputPath) && createPath)
				Directory.CreateDirectory(OutputPath);

			return OutputFullName;
		}
	}
}
