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
                if (fileName != "")
                    ScreenCapture.CaptureScreenshot(fileName + ".png");
                else
                    ScreenCapture.CaptureScreenshot(Application.productName + ".png");
            }
        }
    }
}
