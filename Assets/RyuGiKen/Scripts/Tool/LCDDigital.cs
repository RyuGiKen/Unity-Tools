using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 液晶数字
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class LCDDigital : MonoBehaviour
    {
        /// <summary>
        /// 样式
        /// </summary>
        public enum Style
        {
            /// <summary>
            /// 纯色
            /// </summary>
            SolidColor,
            /// <summary>
            /// 描边
            /// </summary>
            OutLine,
            /// <summary>
            /// 渐变
            /// </summary>
            Gradient,
            /// <summary>
            /// 纯色+描边
            /// </summary>
            SolidColorAndOutLine,
            /// <summary>
            /// 纯色+阴影
            /// </summary>
            SolidColorAndShadow,
            /// <summary>
            /// 渐变+描边
            /// </summary>
            GradientAndOutLine,
            /// <summary>
            /// 渐变+阴影
            /// </summary>
            GradientAndShadow,
            /// <summary>
            /// 纯色+描边+阴影
            /// </summary>
            SolidColorAndOutLineAndShadow,
            /// <summary>
            /// 渐变+描边+阴影
            /// </summary>
            GradientAndOutLineAndShadow,
        }
        public Style style = Style.GradientAndOutLineAndShadow;
        public DigitalSprite[] digitalSprites = new DigitalSprite[10];
        [Tooltip("值")] [SerializeField] int m_Value;
        public int Value
        {
            set { m_Value = value.Clamp(ValueRange.x, ValueRange.y); }
            get { return m_Value.Clamp(ValueRange.x, ValueRange.y); }
        }
        public RectTransform rectTransform
        {
            get { return this.GetComponent<RectTransform>(); }
        }
        [Tooltip("颜色")] public Color m_Color = Color.white;
        [Tooltip("描边颜色")] public Color OutLineColor = Color.white;
        public Image[] images = new Image[4];
        /// <summary>
        /// 取值范围
        /// </summary>
        public readonly static Vector2Int ValueRange = new Vector2Int(0, 9);
        private void Awake()
        {
            if (images.Length < 4)
            {
                images = new Image[4];
                images[3] = this.GetComponent<Image>();
                images[2] = this.transform.GetChild(0).GetComponent<Image>();
                images[1] = this.transform.GetChild(1).GetComponent<Image>();
                images[0] = this.transform.GetChild(2).GetComponent<Image>();
            }
        }
        private void LateUpdate()
        {
            for (int i = 0; i < images.Length; i++)
            {
                if (i < images.Length - 1)
                {
                    images[i].rectTransform.anchorMin = Vector2.zero;
                    images[i].rectTransform.anchorMax = Vector2.one;
                    images[i].rectTransform.offsetMin = images[i].rectTransform.offsetMax = Vector2.zero;
                    images[i].preserveAspect = images[0].preserveAspect;
                }
                images[i].sprite = digitalSprites[Value].sprites[i];
            }
            switch (style)
            {
                case Style.OutLine:
                case Style.SolidColorAndOutLine:
                case Style.GradientAndOutLine:
                case Style.SolidColorAndOutLineAndShadow:
                case Style.GradientAndOutLineAndShadow:
                    images[0].gameObject.SetActive(true);
                    images[0].color = OutLineColor;
                    break;
                default:
                    images[0].gameObject.SetActive(false);
                    break;
            }
            switch (style)
            {
                case Style.SolidColor:
                case Style.SolidColorAndOutLine:
                case Style.SolidColorAndShadow:
                case Style.SolidColorAndOutLineAndShadow:
                    images[1].gameObject.SetActive(true);
                    images[1].color = m_Color;
                    break;
                default:
                    images[1].gameObject.SetActive(false);
                    break;
            }
            switch (style)
            {
                case Style.Gradient:
                case Style.GradientAndOutLine:
                    images[2].gameObject.SetActive(true);
                    images[2].color = m_Color;
                    break;
                default:
                    images[2].gameObject.SetActive(false);
                    break;
            }
            switch (style)
            {
                case Style.GradientAndShadow:
                case Style.GradientAndOutLineAndShadow:
                case Style.SolidColorAndShadow:
                case Style.SolidColorAndOutLineAndShadow:
                    images[3].enabled = true;
                    images[3].color = m_Color;
                    break;
                default:
                    images[3].enabled = false;
                    break;
            }
        }
    }
    /// <summary>
    /// 数字图层
    /// </summary>
    [System.Serializable]
    public class DigitalSprite
    {
        public Sprite OutLine { get { return sprites[0]; } }
        public Sprite Mask { get { return sprites[1]; } }
        public Sprite Color { get { return sprites[2]; } }
        public Sprite Full { get { return sprites[3]; } }
        public Sprite[] sprites = new Sprite[4];
    }
}
