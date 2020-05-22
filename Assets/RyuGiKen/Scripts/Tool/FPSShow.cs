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
        public bool hide = true;//默认隐藏
        public Text FPSText;
        float passTime;
        float deltaTime = 0;
        float m_FPS;
        int FrameCount;
        public float fixTime = 1;
        public KeyCode hideKey = KeyCode.F7;

        [Header("根据帧数自动切换画质(连续10s)")]
        public bool autoAdjustQualityLevel = false;//根据帧数自动切换画质
        public int adjustMinFPS = 20;
        public int adjustMaxFPS = 50;
        private void Start()
        {
            if (FPSText == null)
            { FPSText = GetComponent<Text>(); }
        }
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F1))//按F1降低画质
            { QualitySettings.DecreaseLevel(); }
            if (Input.GetKeyUp(KeyCode.F2))//按F2提高画质
            { QualitySettings.IncreaseLevel(); }
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
