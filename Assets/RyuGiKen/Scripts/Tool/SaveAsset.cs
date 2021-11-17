using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RyuGiKen;
namespace RyuGiKen.Tools
{
	/// <summary>
	/// 保存资产
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
			UnityEngine.Object obj = null;
			if (this.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinnedMesh))
			{
				obj = new Mesh();
				skinnedMesh.BakeMesh(obj as Mesh);
			}
			else if (this.TryGetComponent<MeshFilter>(out MeshFilter mesh))
			{
				obj = mesh.mesh;
			}
			/*else if (this.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
			{
				obj = spriteRenderer.sprite;
			}
			else if (this.TryGetComponent<AudioSource>(out AudioSource audio))
			{
				obj = audio.clip;
			}
			else if (this.TryGetComponent<Image>(out Image image))
			{
				obj = image.sprite;
			}*/
			if (obj == null)
			{
				Debug.LogError("找不到可保存资源");
				return;
			}
			string OutputFullName = ValueAdjust.CheckPathInAssets(fileName, "asset", true, out string OutputPath, out string OutputFileName);

			if (OutputFileName == ".asset")
			{
				OutputFileName = name + ".asset";
				OutputFullName = OutputPath + OutputFileName;
			}

			Debug.Log(OutputFullName);
			AssetDatabase.CreateAsset(obj, OutputFullName);
#endif
		}
	}
}
