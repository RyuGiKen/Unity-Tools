﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using WindowsAPI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 有第二显示器的情况下，使用第二显示器。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/自动切换副屏")]
    public class SetDisplay : MonoBehaviour
    {
        public static SetDisplay instance;
        /// <summary>
        /// 屏幕横竖比例切换
        /// </summary>
        public MyEvent OnScreenAxisChange;
        public Camera.FieldOfViewAxis ScreenAxis;
        const int WS_POPUP = 0x800000;
        const int GWL_STYLE = -16;
        const uint SWP_SHOWWINDOW = 0x0040;
        /// <summary>
        /// 屏幕状态
        /// </summary>
        public struct ScreenState
        {
            /// <summary>
            /// 屏幕数量
            /// </summary>
            public int DisplayLength;
            /// <summary>
            /// 窗口宽
            /// </summary>
            public int WindowWidth;
            /// <summary>
            /// 窗口高
            /// </summary>
            public int WindowHeight;
            /// <summary>
            /// 窗口左上角的X
            /// </summary>
            public int WindowPosX;
            /// <summary>
            /// 窗口左上角的Y
            /// </summary>
            public int WindowPosY;
        }
        public ScreenState state;
        /// <summary>
        /// 窗口句柄
        /// </summary>
        public static IntPtr HWndIntPtr;
        /// <summary>
        /// 鼠标测试位置，右左上下，主屏左上角为原点
        /// </summary>
        static Vector2Int[] TestMousePos = {
            new Vector2Int(1920 + 50, 0),
            new Vector2Int(-1920, 0),
            new Vector2Int(0, -1080 + 50),
            new Vector2Int(0, 1080 + 50),
        };
        [Tooltip("鼠标位置误差范围")] static int ErrorRange = 500;//>2
        [Tooltip("覆盖鼠标位置")] bool OverrideMouse;
        [Tooltip("覆盖鼠标位置")] Vector2 OverrideMousePos;
        static bool useSecondScreen;
        Thread th_SetMouse;
        public static bool UseSecondScreen
        {
            get
            {
                return useSecondScreen && Display.displays.Length > 1;
            }
        }
        bool canRefreshSecondScreen;

        private void Awake()
        {
            if (gameObject.activeInHierarchy)
                if (!instance) instance = this;
            if (instance != this)
                DestroyImmediate(this);

            canRefreshSecondScreen = true;
            OnScreenAxisChange = ChangeScreenAxis;
        }
        private void OnEnable()
        {
#if UNITY_STANDALONE_WIN
            if (th_SetMouse != null)
                th_SetMouse.Abort();
            th_SetMouse = new Thread(new ThreadStart(SetMousePos));
            th_SetMouse.Start();
#endif
        }
        private void OnDisable()
        {
#if UNITY_STANDALONE_WIN
            if (th_SetMouse != null)
                th_SetMouse.Abort();
#endif
        }
        void Start()
        {
            string str = "displays(显示器数)： " + Display.displays.Length;
            DebugL.Log(str);

            string xmlData = GetFile.LoadXmlData(new string[] { "UseSecondScreen", "SecondScreen" }, Application.streamingAssetsPath + "/Setting.xml", "Data", true);
            if (!string.IsNullOrWhiteSpace(xmlData))
            {
                useSecondScreen = xmlData.ToBoolean();
            }
#if UNITY_STANDALONE_WIN
            //HWndIntPtr = (IntPtr)WindowsAPI.User32.FindWindow(null, Application.productName);//工程名，非进程名。非英文会因为编码格式问题找不到窗口。可能误判同名窗口。
            HWndIntPtr = GetProcessWnd();
            //HWndIntPtr = WindowsAPI.User32.GetForegroundWindow();//仅检测前台窗体，不一定为Unity。
#endif
            if (UseSecondScreen)//多显示器
            {
                UpdateToSecondScreen();
            }
            else
            {
                try
                {
#if UNITY_STANDALONE_WIN
                    User32.SetForegroundWindow(HWndIntPtr);
#endif
                }
                catch { }
            }
        }
        /// <summary>
        /// 检测切换副屏
        /// </summary>
        public void UpdateToSecondScreen()
        {
#if UNITY_STANDALONE_WIN
            if (!canRefreshSecondScreen || IsInvoking(nameof(TestRight)) || IsInvoking(nameof(TestLeft)) || IsInvoking(nameof(TestUp)) || IsInvoking(nameof(TestDown)))
#else
            if (!canRefreshSecondScreen)
#endif
                return;
            canRefreshSecondScreen = false;
            state.WindowWidth = Display.displays[1].systemWidth;
            state.WindowHeight = Display.displays[1].systemHeight;
            state.WindowPosX = Display.displays[0].systemWidth;
            state.WindowPosY = 0;
            ErrorRange = Mathf.Clamp(ErrorRange, 2, Mathf.Min(Display.displays[0].systemWidth, Display.displays[0].systemHeight, Display.displays[1].systemWidth, Display.displays[1].systemHeight) - 5);
            TestMousePos[0] = new Vector2Int(Display.displays[0].systemWidth + Display.displays[1].systemWidth / 2, Display.displays[1].systemHeight / 2);
            TestMousePos[1] = new Vector2Int(-Display.displays[1].systemWidth / 2, Display.displays[1].systemHeight / 2);
            TestMousePos[2] = new Vector2Int(Display.displays[0].systemWidth / 2, -Display.displays[1].systemHeight / 2);
            TestMousePos[3] = new Vector2Int(Display.displays[0].systemWidth / 2, Display.displays[0].systemHeight + Display.displays[1].systemHeight / 2);
#if UNITY_STANDALONE_WIN
            User32.SetWindowPos(HWndIntPtr, 0, 0, 0, 0, 0, 1);
            SetOverrideMousePos(TestMousePos[0].x, TestMousePos[0].y, true);
            Invoke(nameof(TestRight), 0.2f);//判定右
#endif
        }
#if UNITY_STANDALONE_WIN
        private void SetMousePos()
        {
            while (true)
            {
                if (OverrideMouse)
                {
                    User32.SetCursorPos((int)OverrideMousePos.x, (int)OverrideMousePos.y);
                }
                Thread.Sleep(50);
            }
        }
#endif
        void LateUpdate()
        {
            Camera.FieldOfViewAxis axis = CheckScreenAxis();
            if (axis != ScreenAxis)
            {
                ScreenAxis = axis;
                OnScreenAxisChange();
            }
            ChangeResolutionForTest();
        }
        /// <summary>
        /// 测试用调整分辨率
        /// </summary>
        public static void ChangeResolutionForTest()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                if (Display.main.systemWidth >= 1920 && Display.main.systemHeight >= 1080)
                    Screen.SetResolution(1920, 1080, true);
                else
                    Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
            }
            else if (Input.GetKeyDown(KeyCode.F10))
            {
                Screen.fullScreen = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetResolution(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SetResolution(0.9f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SetResolution(0.8f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SetResolution(0.7f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SetResolution(0.6f);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetResolution(0.5f);
            }
        }
#if UNITY_STANDALONE_WIN
        void SetOverrideMousePos(float X, float Y, bool Override)
        {
            OverrideMouse = Override;
            OverrideMousePos = new Vector2(X, Y);
        }
        Vector2 GetMousePosition()
        {
            User32.GetCursorPos(out User32.POINT point);
            return new Vector2(point.x, point.y);
        }
        /// <summary>
        /// 测试副屏是否在主屏右方
        /// </summary>
        void TestRight()
        {
            Vector2 mousePosition = GetMousePosition();
            bool result = ValueAdjust.JudgeRange(mousePosition.x, TestMousePos[0].x, ErrorRange);
            string info = "Target：" + TestMousePos[0] + " Mouse：" + mousePosition + " 副屏在主屏右方：" + result;
            DebugL.Log(info);
            SetOverrideMousePos(TestMousePos[1].x, TestMousePos[1].y, true);
            if (result)
                SetPosition(1, 0);//右
            else
                Invoke(nameof(TestLeft), 0.2f);//判定左
        }
        /// <summary>
        /// 测试副屏是否在主屏左方
        /// </summary>
        void TestLeft()
        {
            Vector2 mousePosition = GetMousePosition();
            bool result = ValueAdjust.JudgeRange(mousePosition.x, TestMousePos[1].x, ErrorRange);
            string info = "Target：" + TestMousePos[1] + " Mouse：" + mousePosition + " 副屏在主屏左方：" + result;
            DebugL.Log(info);
            SetOverrideMousePos(TestMousePos[2].x, TestMousePos[2].y, true);
            if (result)
                SetPosition(-1, 0);//左
            else
                Invoke(nameof(TestUp), 0.2f);//判定上
        }
        /// <summary>
        /// 测试副屏是否在主屏上方
        /// </summary>
        void TestUp()
        {
            Vector2 mousePosition = GetMousePosition();
            bool result = ValueAdjust.JudgeRange(mousePosition.y, TestMousePos[2].y, ErrorRange);
            string info = "Target：" + TestMousePos[2] + " Mouse：" + mousePosition + " 副屏在主屏上方：" + result;
            DebugL.Log(info);
            SetOverrideMousePos(TestMousePos[3].x, TestMousePos[3].y, true);
            if (result)
                SetPosition(0, -1);//上
            else
                Invoke(nameof(TestDown), 0.2f);//判定下
        }
        /// <summary>
        /// 测试副屏是否在主屏下方
        /// </summary>
        void TestDown()
        {
            Vector2 mousePosition = GetMousePosition();
            bool result = ValueAdjust.JudgeRange(mousePosition.y, TestMousePos[3].y, ErrorRange);
            string info = "Target：" + TestMousePos[3] + " Mouse：" + mousePosition + " 副屏在主屏下方：" + result;
            DebugL.Log(info);
            if (result)
                SetPosition(0, 1);//下
            else
                SetPosition(0, 0);//都不符合，放在主屏
        }
        /// <summary>
        /// 设置无边框窗口化（副屏相对主屏的位置差）
        /// </summary>
        public void SetPosition(int X, int Y)
        {
            StartCoroutine(Setposition(X, Y));
        }
        IEnumerator Setposition(int X, int Y)
        {
            Screen.SetResolution(state.WindowWidth, state.WindowHeight, false);
            yield return new WaitForSeconds(0.01f);
            OverrideMouse = false;
            User32.SetWindowLong(HWndIntPtr, GWL_STYLE, WS_POPUP);//无边框
            X = Mathf.Clamp(X, -1, 1);
            Y = Mathf.Clamp(Y, -1, 1);
            switch (X + "," + Y)
            {
                case "-1,0":
                    state.WindowPosX = -Display.displays[1].systemWidth;
                    state.WindowPosY = 0;
                    break;
                default:
                case "0,0":
                    state.WindowPosX = 0;
                    state.WindowPosY = 0;
                    yield break;
                    break;
                case "1,0":
                    state.WindowPosX = Display.displays[0].systemWidth;
                    state.WindowPosY = 0;
                    break;
                case "0,1":
                    state.WindowPosX = 0;
                    state.WindowPosY = Display.displays[0].systemHeight;
                    break;
                case "0,-1":
                    state.WindowPosX = 0;
                    state.WindowPosY = -Display.displays[1].systemHeight;
                    break;
            }
            bool result = User32.SetWindowPos(HWndIntPtr, 1, state.WindowPosX, state.WindowPosY, state.WindowWidth, state.WindowHeight, SWP_SHOWWINDOW);//设置屏幕大小和位置
            canRefreshSecondScreen = true;
            DebugL.Log("副屏设置" + result);
        }
#endif
        /// <summary>
        /// 窗口置顶
        /// </summary>
        public static void SetForegroundWindow()
        {
#if UNITY_STANDALONE_WIN
            switch (Screen.fullScreenMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                case FullScreenMode.FullScreenWindow:
                    User32.SetWindowPos(HWndIntPtr, -1, 0, 0, 0, 0, SWP_SHOWWINDOW);
                    //User32.SwitchToThisWindow(HWndIntPtr, true);
                    User32.SetForegroundWindow(HWndIntPtr);
                    User32.SetWindowPos(HWndIntPtr, -2, 0, 0, 0, 0, SWP_SHOWWINDOW);
                    break;
                case FullScreenMode.MaximizedWindow:
                case FullScreenMode.Windowed:
                    User32.SetWindowPos(HWndIntPtr, -1, 0, 0, 0, 0, 1 | 2);
                    //User32.SwitchToThisWindow(HWndIntPtr, true);
                    User32.SetForegroundWindow(HWndIntPtr);
                    User32.SetWindowPos(HWndIntPtr, -2, 0, 0, 0, 0, 1 | 2);
                    break;
            }
#endif
        }
#if UNITY_STANDALONE_WIN
        /// <summary>
        /// 获取当前窗体句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetProcessWnd()
        {
            IntPtr ptrWnd = IntPtr.Zero;
            uint pid = (uint)Process.GetCurrentProcess().Id;//当前进程ID
            bool bResult = WindowsAPI.User32.EnumWindows(new WindowsAPI.User32.WNDENUMPROC(delegate (IntPtr hwnd, uint lParam)
            {
                uint id = 0;
                if (WindowsAPI.User32.GetParent(hwnd) == IntPtr.Zero)
                {
                    WindowsAPI.User32.GetWindowThreadProcessId(hwnd, ref id);
                    if (id == lParam)//找到进程对应的主窗口句柄
                    {
                        ptrWnd = hwnd;//把句柄缓存起来
                        WindowsAPI.User32.SetLastError(0);//设置无错误
                        return false;//返回 false 以终止枚举窗口
                    }
                }
                return true;
            }), pid);
            return (!bResult && Marshal.GetLastWin32Error() == 0) ? ptrWnd : IntPtr.Zero;
        }
#endif
        /// <summary>
        /// 切换屏幕模式
        /// <para>1仅电脑屏幕</para>
        /// <para>2复制屏</para>
        /// <para>3扩展屏</para>
        /// <para>4仅第二屏幕</para>
        /// </summary>
        public static void DisplaySwitch(int mode)
        {
#if UNITY_STANDALONE_WIN
            string arguments = null;
            switch (mode)
            {
                case 1:
                    arguments = "/internal";
                    break;
                case 2:
                    arguments = "/clone";
                    break;
                case 3:
                    arguments = "/extend";
                    break;
                case 4:
                    arguments = "/external";
                    break;
            }
            if (string.IsNullOrEmpty(arguments))
                Process.Start("C:\\Windows\\System32\\DisplaySwitch.exe", arguments);
            else
                User32.SetScreenMode(mode);
#endif
        }
        /// <summary>
        /// 按比例缩放分辨率
        /// </summary>
        /// <param name="percent">比例</param>
        /// <param name="fullscreen">全屏</param>
        /// <param name="displayIndex">屏幕序号</param>
        public static void SetResolution(float percent = 1, bool fullscreen = true, int displayIndex = -1)
        {
            if (percent <= 0 || displayIndex >= Display.displays.Length)
                return;
            if (UseSecondScreen)
                return;
            if (displayIndex < 0)
                displayIndex = (UseSecondScreen ? 1 : 0).Clamp(0, Display.displays.Length - 1);
            int width = fullscreen ? Display.displays[displayIndex].systemWidth : Screen.width;
            int height = fullscreen ? Display.displays[displayIndex].systemHeight : Screen.height;
            Vector2Int resolution = new Vector2Int((width * percent).ToInteger(), (height * percent).ToInteger());
            Screen.SetResolution(resolution.x, resolution.y, fullscreen);
        }
        /// <summary>
        /// 从宽度设置分辨率（例如720i）
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="fullscreen"></param>
        /// <param name="displayIndex"></param>
        public static void SetResolutionByWidth(float Width, bool fullscreen = true, int displayIndex = -1)
        {
            if (displayIndex >= Display.displays.Length)
                return;
            if (UseSecondScreen)
                return;
            if (displayIndex < 0)
                displayIndex = (UseSecondScreen ? 1 : 0).Clamp(0, Display.displays.Length - 1);
            int width = fullscreen ? Display.displays[displayIndex].systemWidth : Screen.width;
            int height = fullscreen ? Display.displays[displayIndex].systemHeight : Screen.height;

            Vector2Int resolution = GetResolutionByWidth(Width, new Vector2Int(width, height));
            Screen.SetResolution(resolution.x, resolution.y, fullscreen);
        }
        /// <summary>
        /// 从高度设置分辨率（例如720p）
        /// </summary>
        /// <param name="Height"></param>
        /// <param name="fullscreen"></param>
        /// <param name="displayIndex"></param>
        public static void SetResolutionByHeight(float Height, bool fullscreen = true, int displayIndex = -1)
        {
            if (displayIndex >= Display.displays.Length)
                return;
            if (UseSecondScreen)
                return;
            if (displayIndex < 0)
                displayIndex = (UseSecondScreen ? 1 : 0).Clamp(0, Display.displays.Length - 1);
            int width = fullscreen ? Display.displays[displayIndex].systemWidth : Screen.width;
            int height = fullscreen ? Display.displays[displayIndex].systemHeight : Screen.height;

            Vector2Int resolution = GetResolutionByHeight(Height, new Vector2Int(width, height));
            Screen.SetResolution(resolution.x, resolution.y, fullscreen);
        }
        /// <summary>
        /// 从宽度计算分辨率
        /// </summary>
        /// <param name="width"></param>
        /// <param name="Resolution"></param>
        /// <returns></returns>
        public static Vector2Int GetResolutionByWidth(float width, Vector2Int Resolution)
        {
            Vector2Int result = Vector2Int.one;
            result.x = width.ToInteger();
            result.y = (width * Resolution.y * 1f / Resolution.x).ToInteger();
            return result;
        }
        /// <summary>
        /// 从高度计算分辨率
        /// </summary>
        /// <param name="height"></param>
        /// <param name="Resolution"></param>
        /// <returns></returns>
        public static Vector2Int GetResolutionByHeight(float height, Vector2Int Resolution)
        {
            Vector2Int result = Vector2Int.one;
            result.x = (height * Resolution.x * 1f / Resolution.y).ToInteger();
            result.y = height.ToInteger();
            return result;
        }
        /// <summary>
        /// 分辨率
        /// </summary>
        public static string GetResolution()
        {
            return DisplayData.GetResolution(true);
        }
        /// <summary>
        /// 判断横竖屏
        /// </summary>
        /// <returns></returns>
        public static Camera.FieldOfViewAxis CheckScreenAxis()
        {
            if (Screen.height > Screen.width * 1.2f)
            {
                return Camera.FieldOfViewAxis.Vertical;
            }
            return Camera.FieldOfViewAxis.Horizontal;
        }
        public void ChangeScreenAxis() { }
    }
}
