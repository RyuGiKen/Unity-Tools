using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// FPS显示，限制，画质切换
    /// </summary>
    [AddComponentMenu("RyuGiKen/FPS显示")]
    [DisallowMultipleComponent]
    public class FPSShow : MonoBehaviour
    {
        [Tooltip("隐藏")] public bool hide = true;//默认隐藏
        public Text FPSText;
        float passTime;
        float m_FPS;
        int FrameCount;
        [Tooltip("统计时间")] public float fixTime = 1;
        [Tooltip("切换按键")] public KeyCode hideKey = KeyCode.F7;
        [Tooltip("不受时间缩放影响")] public bool UnscaledDeltaTime = true;
        [Header("根据帧数自动切换画质(连续10s)")]
        [Tooltip("根据帧数自动切换画质")] public bool autoAdjustQualityLevel = false;
        [Tooltip("帧率超范围计时")] static float deltaTime = 0;
        [Tooltip("最小帧率")] public int adjustMinFPS = 20;
        [Tooltip("最大帧率")] public int adjustMaxFPS = 50;
        [Tooltip("限制帧率")] public int LockFrameRate = 60;
        private void Start()
        {
            if (FPSText == null)
            { FPSText = GetComponent<Text>(); }
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = LockFrameRate;
        }
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F1))//按F1降低画质
            {
                AdjustQualityLevel(false);
            }
            if (Input.GetKeyUp(KeyCode.F2))//按F2提高画质
            {
                AdjustQualityLevel(true);
            }
            if (Input.GetKeyUp(KeyCode.F3))//按F3限制帧数为60;
            {
                Application.targetFrameRate = LockFrameRate;
            }
            if (Input.GetKeyUp(KeyCode.F4))//按F4不限制帧数;
            {
                Application.targetFrameRate = -1;
            }
            if (FPSText != null)
            {
                if (Input.GetKeyDown(hideKey))
                {
                    hide = !hide;
                }
                FPSText.enabled = !hide;
                ShowFPS();
            }
            if (autoAdjustQualityLevel && m_FPS < adjustMinFPS && Time.timeScale > 0)//自动降低画质
            {
                if (deltaTime <= 0)
                    deltaTime -= Time.unscaledDeltaTime;
                else
                    deltaTime = 0;
                if (deltaTime <= -10)
                {
                    AdjustQualityLevel(false);
                    deltaTime = 0;
                }
            }
            else if (autoAdjustQualityLevel && m_FPS > adjustMaxFPS && Time.timeScale > 0)//自动提高画质
            {
                if (deltaTime >= 0)
                    deltaTime += Time.unscaledDeltaTime;
                else
                    deltaTime = 0;
                if (deltaTime >= 10)
                {
                    AdjustQualityLevel(true);
                }
            }
        }
        /// <summary>
        /// FPS计数
        /// </summary>
        void ShowFPS()
        {
            FrameCount += 1;
            passTime += (UnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime);

            if (passTime > fixTime)
            {
                m_FPS = Mathf.RoundToInt(FrameCount / passTime);
                FPSText.text = "FPS  " + m_FPS.ToString() + "  L" + QualitySettings.GetQualityLevel().ToString();
                passTime = 0.0f;
                FrameCount = 0;
            }
        }
        public static void AdjustQualityLevel(bool isRise)
        {
            deltaTime = 0;
            if (isRise)
            {
                QualitySettings.IncreaseLevel();
            }
            else
            {
                QualitySettings.DecreaseLevel();
            }
        }
    }
}
