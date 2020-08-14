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
        float deltaTime = 0;
        float m_FPS;
        int FrameCount;
        [Tooltip("统计时间")] public float fixTime = 1;
        [Tooltip("切换按键")] public KeyCode hideKey = KeyCode.F7;

        [Header("根据帧数自动切换画质(连续10s)")]
        public bool autoAdjustQualityLevel = false;//根据帧数自动切换画质
        [Tooltip("最小帧数")] public int adjustMinFPS = 20;
        [Tooltip("最大帧数")] public int adjustMaxFPS = 50;
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
                QualitySettings.DecreaseLevel();
            }
            if (Input.GetKeyUp(KeyCode.F2))//按F2提高画质
            {
                QualitySettings.IncreaseLevel();
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
                FPS();
            }
            if (autoAdjustQualityLevel && m_FPS < adjustMinFPS)//自动降低画质
            {
                if (deltaTime <= 0)
                    deltaTime -= Time.deltaTime;
                else
                    deltaTime = 0;
                if (deltaTime <= -10)
                {
                    QualitySettings.DecreaseLevel();
                }
            }
            else if (autoAdjustQualityLevel && m_FPS > adjustMaxFPS)//自动提高画质
            {
                if (deltaTime >= 0)
                    deltaTime += Time.deltaTime;
                else
                    deltaTime = 0;
                if (deltaTime >= 10)
                {
                    QualitySettings.IncreaseLevel();
                }
            }
        }
        /// <summary>
        /// 显示FPS
        /// </summary>
        void FPS()
        {
            FrameCount += 1;
            passTime += Time.unscaledDeltaTime;//Time.deltaTime;

            if (passTime > fixTime)
            {
                m_FPS = Mathf.RoundToInt(FrameCount / passTime);
                FPSText.text = "FPS  " + m_FPS.ToString() + "  " + QualitySettings.GetQualityLevel().ToString();
                passTime = 0.0f;
                FrameCount = 0;
            }
        }
    }
}
