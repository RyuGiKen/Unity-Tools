using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 截图
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/截图")]
    public class ScreenShot : MonoBehaviour
    {
        [Tooltip("截图按键")][SerializeField] KeyCode ScreenShotKey = KeyCode.F12;
        [Tooltip("输出路径（为空时默认在工程文件夹根目录，游戏_Data文件夹内）")] public string fileName;
        void Update()
        {
            if (Input.GetKeyDown(ScreenShotKey) && Application.isPlaying)
            {
                PrintScreen();
            }
        }
        /// <summary>
        /// 截图(游戏视图)
        /// </summary>
        [ContextMenu("截图(游戏视图)")]
        public void PrintScreen()
        {
            PrintScreen(fileName, true);
        }
        /// <summary>
        /// 截图(场景视图)
        /// </summary>
        [ContextMenu("截图(场景视图)")]
        public void PrintSceneView()
        {
            PrintScreen(fileName, false);
        }
        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="GameView">false时为场景视图</param>
        public static void PrintScreen(bool GameView)
        {
            ScreenShot screenShootor = FindObjectOfType<ScreenShot>();
            PrintScreen(screenShootor ? screenShootor.fileName : null, GameView);
        }
        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="fileName">路径</param>
        /// <param name="GameView">false时为场景视图</param>
        public static string PrintScreen(string fileName, bool GameView = true)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = Application.productName + "_" + DateTime.Now.ToString().Replace(":", "_").Replace("/", "_").Replace("\\", "_");

            if (fileName.IndexOf(':') == 1)
            {
                fileName = ValueAdjust.CheckFilePath(fileName, "png", true);
            }
            else
            {
                fileName = ValueAdjust.CheckPathInRoot(fileName, "png", true);
            }

            if (GameView)
            {
                ScreenCapture.CaptureScreenshot(fileName);
            }
            else
            {
#if UNITY_EDITOR
                int width = Screen.currentResolution.width;
                int height = Screen.currentResolution.height;
                RenderTexture rt = new RenderTexture(width, height, 16);
                Camera m_Camera = UnityEditor.SceneView.GetAllSceneCameras()[0];
                m_Camera.targetTexture = rt;
                m_Camera.Render();
                RenderTexture.active = rt;
                Texture2D t = new Texture2D(width, height);
                t.ReadPixels(new Rect(0, 0, t.width, t.height), 0, 0);
                t.Apply();
                File.WriteAllBytes(fileName, t.EncodeToPNG());
#endif
            }
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Debug.Log("保存截图：" + fileName);
                    break;
                default:
                    Debug.Log("Screenshot Saved：" + fileName);
                    break;
            }
            return fileName;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ScreenShot), true)]
    public class ScreenShotEditor : Editor
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
                    ButtonName[0] = "截图(场景视图)";
                    ButtonName[1] = "截图(游戏视图)";
                    break;
                default:
                    ButtonName[0] = "ScreenShot (Scene View)";
                    ButtonName[1] = "ScreenShot (Game View)";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                ScreenShot.PrintScreen((target as ScreenShot).fileName, false);
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                ScreenShot.PrintScreen((target as ScreenShot).fileName, true);
            }
        }
    }
#endif
}