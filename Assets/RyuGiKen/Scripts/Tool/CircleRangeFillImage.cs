using RyuGiKen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// ���η�Χ���
    /// </summary>
    [ExecuteAlways]
    public class CircleRangeFillImage : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Image imageBG;
        /// <summary>
        /// ��ʾ��Χֵ
        /// </summary>
        public ValueWithRange value = new ValueWithRange(0, -30, 30);//��ǰֵ����С�����
        /// <summary>
        /// ��Χ����
        /// </summary>
        //public ValueRange valueRange_Limit = new ValueRange(-90, 90);
        /// <summary>
        /// ��Χ���ƣ��������ַ��򣬾���Ϊ0ʱΪ[-90��90]
        /// </summary>
        public Vector2Int RangeLimit = new Vector2Int(-90, 90);
        /// <summary>
        /// ��ǰֵ
        /// </summary>
        public Image ArrowImage;
        /// <summary>
        /// �߽���
        /// </summary>
        public Image SideImage1;
        /// <summary>
        /// �߽���
        /// </summary>
        public Image SideImage2;
        /// <summary>
        /// �������
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
