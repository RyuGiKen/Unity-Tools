using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        [Tooltip("截图按键")] [SerializeField] KeyCode ScreenShotKey = KeyCode.F12;
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
            PrintScreen(screenShootor?.fileName, GameView);
        }
        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="fileName">路径</param>
        /// <param name="GameView">false时为场景视图</param>
        public static void PrintScreen(string fileName, bool GameView = true)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = Application.dataPath + "/" + Application.productName;
            if (!GetFile.JudgeFileType(fileName, ".png"))
                fileName += ".png";
            if (GameView)
            {
                ScreenCapture.CaptureScreenshot(fileName);
            }
            else
            {
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
                System.IO.File.WriteAllBytes(fileName, t.EncodeToPNG());
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
        }
    }
}