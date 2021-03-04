using UnityEngine;
using System.Collections;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 渲染动画
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/渲染动画")]
    public class FrameTextureRenderer : MonoBehaviour
    {
        public bool showFrameName = false;
        [Tooltip("渲染时长（s）")] public int renderTime = 30;//受时间缩放影响的运行时间
        [Tooltip("时间缩放")] public float timeScale = 0.08f;
        [Tooltip("输出路径")] public string outPutPath;
        [Tooltip("帧率")] public int frameRate = 30;
        private int curFrame = 0;//当前帧
        public bool stop = true;
        private string fileName;
        void Awake()
        {
            if (outPutPath == null)
            {
                stop = true;
            }
        }
        void Start()
        {
            if (!stop)
            {
                Application.targetFrameRate = frameRate;
                curFrame = 0;
                Invoke(nameof(StopRender), renderTime);
                Rendering();
                InvokeRepeating(nameof(RenderFrameTexture), 1f / frameRate, 1f / frameRate);
            }
        }
        /// <summary>
        /// 开始渲染
        /// </summary>
        [ContextMenu("开始渲染")]
        public void StartRender()
        {
            stop = false;
            Start();
        }
        private void OnGUI()
        {
            if (showFrameName)
            {
                GUI.Label(new Rect(10, 5, 200, 100), curFrame.ToString());
            }
        }
        /// <summary>
        /// 渲染单帧
        /// </summary>
        void RenderFrameTexture()
        {
            if (!stop)
            {
                curFrame++;
                Rendering();
            }
        }
        /// <summary>
        /// 渲染成图
        /// </summary>
        void Rendering()
        {
            Time.timeScale = 0;
            fileName = curFrame.ToString("D" + ValueAdjust.GetNumDigit(renderTime * frameRate));
            ScreenShot.PrintScreen(outPutPath + fileName);
            Time.timeScale = timeScale; Debug.Log(curFrame);
        }
        /// <summary>
        /// 停止渲染
        /// </summary>
        [ContextMenu("停止渲染")]
        public void StopRender()
        {
            stop = true;
            Time.timeScale = 0;
            CancelInvoke(nameof(RenderFrameTexture));
            Debug.Log("渲染完毕");
        }
    }
}