using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using WindowsAPI;
namespace WindowsAPI
{
    /// <summary>
    /// 屏幕键盘
    /// </summary>
    [DisallowMultipleComponent]
    public class OpenKey : MonoBehaviour
    {
        [ContextMenu("调出屏幕键盘")]
        public void OpenKeyClick()
        {
            try
            {
                VirtualKeyboard.OpenKeyboard();
            }
            catch { }
        }
        [ContextMenu("隐藏屏幕键盘")]
        public void HideKey()
        {
            try
            {
                VirtualKeyboard.HideTabTip();
            }
            catch { }
        }
    }
    /// <summary>
    /// 屏幕键盘
    /// </summary>
    public static class VirtualKeyboard
    {

        private const uint WM_SYSCOMMAND = 274;
        private const int SC_CLOSE = 61536;
        private static Process ExternalCall(string filename, string arguments, bool hideWindow)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filename;
            startInfo.Arguments = arguments;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (hideWindow)
            {
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
            }
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            return process;
        }
        /// <summary>
        /// 识别系统打开虚拟键盘
        /// </summary>
        public static void OpenKeyboard()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            try
            {
                if (System.IO.File.Exists("C:\\Program Files\\Common Files\\Microsoft Shared\\ink\\TabTip.exe")
                    //&& FindWindow("IPTip_Main_Window", null) != IntPtr.Zero 
                    && SystemInfo.processorType.IndexOf("Atom", StringComparison.OrdinalIgnoreCase) < 0)//测试发现CPU为Atom型号的Win10平板有TabTip.exe但没成功实现切换，因此排除
                {
                    OpenTabTip();
                }
                else
                {
                    OpenOSK();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
#endif
        }
        /// <summary>
        /// 打开旧版屏幕键盘Osk.exe
        /// </summary>
        public static void OpenOSK()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Process.Start("C:\\Windows\\System32\\osk.exe");
#endif
        }
        /// <summary>
        /// 打开屏幕键盘TabTip.exe
        /// </summary>
        public static void OpenTabTip()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            //HideTabTip();
            ExternalCall("C:\\Program Files\\Common Files\\Microsoft Shared\\ink\\TabTip.exe", null, false);
#endif
        }
        /// <summary>
        /// 隐藏屏幕键盘TabTip.exe
        /// </summary>
        public static void HideTabTip()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            try
            {
                IntPtr ptr = WindowsAPI.User32.FindWindow("IPTip_Main_Window", null);
                if (ptr == IntPtr.Zero)
                    return;
                WindowsAPI.User32.PostMessage(ptr, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
#endif
        }
    }
}
#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomEditor(typeof(OpenKey), true)]
    public class OpenKeyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[2];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "调出屏幕键盘";
                    ButtonName[1] = "隐藏屏幕键盘";
                    break;
                default:
                    ButtonName[0] = "Open Keyboard";
                    ButtonName[1] = "Hide Keyboard";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as OpenKey).OpenKeyClick();
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                (target as OpenKey).HideKey();
            }
        }
    }
}
#endif
