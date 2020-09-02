using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen
{
	/// <summary>
	/// 版本号格式限制
	/// </summary>
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[RequireComponent(typeof(Text))]
	public class Version : MonoBehaviour
	{
		public static Version instance;
		public string versionNumber = "b1.0.0";
		void Awake()
		{
			if (Application.isPlaying)
			{
				//UpdateVersionNumber();
			}
		}
		private void LateUpdate()
		{
			if (versionNumber != Application.version || this.GetComponent<Text>().text != Application.version)
				UpdateVersionNumber();
		}
		[ContextMenu("更新版本号赋值")]
		void UpdateVersionNumber()
		{
			versionNumber = SetVersionNumber(Application.version);
			this.GetComponent<Text>().text = versionNumber;
		}
		/// <summary>
		/// 版本号赋值，格式限制
		/// </summary>
		/// <param name="number"></param>
		public static string SetVersionNumber(string number)
		{
			string versionNumber = number.Trim(' ');//移除空格
													//第一位字母
			string type = "b";
			if (versionNumber.Length != 0)
			{
				if (versionNumber.Substring(0, 1) == "v" || versionNumber.Substring(0, 1) == "V")
					type = "v";
			}
			//数字部分
			int index = ValueAdjust.FindIndexOfNumInString(versionNumber);//找出第一位数字
																		  //Debug.Log(index);
			string[] num;
			if (index > 0)
				num = versionNumber.Remove(0, index).Split('.');//移除第一位数字前的部分
			else
				num = versionNumber.Split('.');
			int[] ver = new int[3];
			for (int i = 0; i < num.Length; i++)
			{
				//Debug.Log(num[i]);
				if (i < 3)//三位版本号
					int.TryParse(num[i], out ver[i]);//能转换整数才读取
			}
			versionNumber = type + ver[0] + "." + ver[1] + "." + ver[2];
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
			return Version.SetVersionNumber(Application.version);
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
			Info += "程序名称：" + GetProgramName() + "\n";
			Info += "发布版本：" + GetProgramVersion() + "\n";
			Info += "构建版本：" + GetProgramUnityVersion() + "\n";
			Info += "打包日期：" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
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
