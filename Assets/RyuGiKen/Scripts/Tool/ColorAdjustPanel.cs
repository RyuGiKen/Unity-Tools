using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    public class ColorAdjustPanel : MonoBehaviour
    {
        public enum ColorMode
        {
            RGB,
            HSV,
        }
        public Text PreviewText;
        public ColorMode mode;
        public HSVColor color;
        public Image ColorImage;
        [System.Serializable]
        public struct ParameterChannel
        {
            public Text m_Name;
            public Text m_Value;
            public Slider m_Slider;
            public float GetValue()
            {
                return m_Slider.value;
            }

            public string UpdateValue()
            {
                string str = GetValue().ToString();
                m_Value.text = str;
                return str;
            }
        }
        [SerializeField] ParameterChannel[] Channels = new ParameterChannel[4];
        public UnityEvent OnChangeColor;
        void Start()
        {
            UpdateMode();
        }
        void Update()
        {

        }
        private void LateUpdate()
        {
            UpdateValue();
        }
        public void UpdateValue()
        {
            for (int i = 0; i < Channels.Length; i++)
            {
                Channels[i].UpdateValue();
            }
        }
        public void UpdateColor()
        {
            switch (mode)
            {
                case ColorMode.RGB:
                    color = new Color(Channels[0].GetValue() / 255f, Channels[1].GetValue() / 255f, Channels[2].GetValue() / 255f, Channels[3].GetValue() / 255f).ConvertRgbToHsv();
                    break;
                case ColorMode.HSV:
                    color = new HSVColor(Channels[0].GetValue(), Channels[1].GetValue() / 100f, Channels[2].GetValue() / 100f, Channels[3].GetValue() / 100f);
                    break;
            }
            ColorImage.color = color;

            Color temp = color;
            PreviewText.text = string.Format("RGB:({0:F0}, {1:F0}, {2:F0}, {3:F0})\nHSV:({4:F0}, {5:F0}, {6:F0}, {7:F0})", temp.r * 255, temp.g * 255, temp.b * 255, temp.a * 255, color.Hue, color.Saturation * 100, color.Value * 100, color.alpha * 100);

            if (OnChangeColor != null)
                OnChangeColor.Invoke();
        }
        [ContextMenu("切换模式")]
        public void SwitchMode()
        {
            switch (mode)
            {
                case ColorMode.RGB:
                    mode = ColorMode.HSV;
                    break;
                case ColorMode.HSV:
                    mode = ColorMode.RGB;
                    break;
            }
            UpdateMode();
        }
        public void UpdateMode()
        {
            HSVColor temp = color;
            switch (mode)
            {
                case ColorMode.RGB:
                    Channels[0].m_Name.text = "红";
                    Channels[1].m_Name.text = "绿";
                    Channels[2].m_Name.text = "蓝";

                    Channels[0].m_Slider.maxValue = 255;
                    Channels[1].m_Slider.maxValue = 255;
                    Channels[2].m_Slider.maxValue = 255;
                    Channels[3].m_Slider.maxValue = 255;

                    Color temp2 = temp;
                    Channels[0].m_Slider.value = temp2.r * 255;
                    Channels[1].m_Slider.value = temp2.g * 255;
                    Channels[2].m_Slider.value = temp2.b * 255;
                    Channels[3].m_Slider.value = temp2.a * 255;
                    break;
                case ColorMode.HSV:
                    Channels[0].m_Name.text = "色调";
                    Channels[1].m_Name.text = "饱和";
                    Channels[2].m_Name.text = "明度";

                    Channels[0].m_Slider.maxValue = 360;
                    Channels[1].m_Slider.maxValue = 100;
                    Channels[2].m_Slider.maxValue = 100;
                    Channels[3].m_Slider.maxValue = 100;

                    Channels[0].m_Slider.value = temp.Hue;
                    Channels[1].m_Slider.value = temp.Saturation * 100;
                    Channels[2].m_Slider.value = temp.Value * 100;
                    Channels[3].m_Slider.value = temp.alpha * 100;
                    break;
            }
            //UpdateColor();
        }
    }
}
