using RyuGiKen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 扇形范围填充
    /// </summary>
    [ExecuteAlways]
    public class CircleRangeFillImage : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Image imageBG;
        /// <summary>
        /// 显示范围值
        /// </summary>
        public ValueWithRange value = new ValueWithRange(0, -30, 30);//当前值、最小、最大
        /// <summary>
        /// 范围限制
        /// </summary>
        //public ValueRange valueRange_Limit = new ValueRange(-90, 90);
        /// <summary>
        /// 范围限制，用于区分方向，居中为0时为[-90，90]
        /// </summary>
        public Vector2Int RangeLimit = new Vector2Int(-90, 90);
        /// <summary>
        /// 当前值
        /// </summary>
        public Image ArrowImage;
        /// <summary>
        /// 边界轴
        /// </summary>
        public Image SideImage1;
        /// <summary>
        /// 边界轴
        /// </summary>
        public Image SideImage2;
        /// <summary>
        /// 填充扇形
        /// </summary>
        public Image FillImage;

        public float Radius = 500;
        void Start()
        {

        }
        void LateUpdate()
        {
            if (rectTransform)
            {
                Radius = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) / 2f;
            }
            ValueInRange angles = new ValueInRange(ValueAdjust.MappingRange(value, RangeLimit.x, RangeLimit.y, -90, 90), ValueAdjust.MappingRange(value.MinValue, RangeLimit.x, RangeLimit.y, -90, 90), ValueAdjust.MappingRange(value.MaxValue, RangeLimit.x, RangeLimit.y, -90, 90));
            float percent = ValueAdjust.ToPercent01(value.Range.Length, 0, 360);
            if (ArrowImage.IsActiveAndEnabled())
            {
                ArrowImage.rectTransform.localEulerAngles = Vector3.forward * angles.Value;
            }
            if (SideImage1.IsActiveAndEnabled())
            {
                SideImage1.rectTransform.localEulerAngles = Vector3.forward * angles.MinValue;
            }
            if (SideImage2.IsActiveAndEnabled())
            {
                SideImage2.rectTransform.localEulerAngles = Vector3.forward * angles.MaxValue;
            }
            if (FillImage.IsActiveAndEnabled())
            {
                FillImage.rectTransform.localEulerAngles = Vector3.forward * (FillImage.fillClockwise ? angles.MaxValue : angles.MinValue);
                FillImage.fillAmount = percent;
            }
        }
    }
}
