using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 范围滑动条
    /// </summary>
    [ExecuteAlways]
    public class RangeSlider : MonoBehaviour
    {
        public Camera.FieldOfViewAxis AxisMode = Camera.FieldOfViewAxis.Horizontal;
        public RectTransform rect;
        /// <summary>
        /// 显示范围值
        /// </summary>
        [SerializeField] protected ValueRange valueRange = Vector2.up;
        /// <summary>
        /// 范围限制
        /// </summary>
        [SerializeField] protected ValueRange valueRange_Limit = Vector2.up * 2;
        private void Reset()
        {
            //if (!rect)
            //    rect = GetComponent<RectTransform>();
        }
        void Start()
        {

        }
        void Update()
        {
            if (rect)
            {
                float percentMin = ValueAdjust.ToPercent01(valueRange.MinValue, valueRange_Limit);
                float percentMax = ValueAdjust.ToPercent01(valueRange.MaxValue, valueRange_Limit);
                switch (AxisMode)
                {
                    case Camera.FieldOfViewAxis.Horizontal:
                        rect.anchorMin = new Vector2(percentMin, 0);
                        rect.anchorMax = new Vector2(percentMax, 1);
                        break;
                    case Camera.FieldOfViewAxis.Vertical:
                        rect.anchorMin = new Vector2(0, percentMin);
                        rect.anchorMax = new Vector2(1, percentMax);
                        break;
                }
            }
        }
        public void SetValue(ValueRange range)
        {
            valueRange = range;
            UpdateValue();
        }
        public void SetRangeLimit(ValueRange range)
        {
            valueRange_Limit = range;
            UpdateValue();
        }
        public void UpdateValue()
        {
            float min = valueRange.MinValue.Clamp(valueRange_Limit);
            float max = valueRange.MaxValue.Clamp(valueRange_Limit);
            valueRange.SetRange(min, max);
        }
    }
}
