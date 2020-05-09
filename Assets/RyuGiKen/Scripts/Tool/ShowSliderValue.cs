using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 显示Slider的值到Text中
    /// </summary>
    [AddComponentMenu("RyuGiKen/滑块值显示")]
    public class ShowSliderValue : MonoBehaviour
    {
        public Text text;
        public Slider slider;
        [Tooltip("值")] public float value;
        [Tooltip("限制")] public bool LimitValue;
        [Tooltip("精度")] public int digit;
        void Start()
        {
            if (!text)
                text = GetComponent<Text>();
            if (!slider)
                slider = GetComponent<Slider>();
            if (slider)
                value = slider.value;
        }
        private void LateUpdate()
        {
            if (text && slider)
            {
                if (LimitValue)
                    value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
                else
                    value = slider.value;
                slider.value = value;
                text.text = value.ToString("F" + digit);
            }
        }
        public void SetValue(float n)
        {
            slider.value = value = n;
        }
    }
}
