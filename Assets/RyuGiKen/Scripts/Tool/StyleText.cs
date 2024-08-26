using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 根据背景自动设置字体颜色
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public class StyleText : MonoBehaviour
    {
        public bool AutoSetTextColor;
        [SerializeField] protected ColorStyle BackgroundStyle;
        [SerializeField] protected ColorStyle TextStyle;
        public Graphic TextBackground;
        public Text m_Text;
        public bool ShowTestGrayData;
        [Range(0, 1)] public float TestGrayData;
        public const float GrayValue = 0.71f;
        protected void Awake()
        {
            m_Text = GetComponent<Text>();
            if (!TextBackground)
            {
                Graphic[] temp = this.GetComponentsInParent<Graphic>();
                for (int i = 0; i < (temp != null ? temp.Length : 0); i++)
                {
                    if (temp[i] && temp[i] != TextBackground && !(temp[i] is Text))
                    {
                        TextBackground = temp[i];
                        break;
                    }
                }
                if (!TextBackground || !TextBackground.enabled)
                {
                    temp = this.transform.parent.GetComponentsInChildren<Graphic>();
                    for (int i = 0; i < (temp != null ? temp.Length : 0); i++)
                    {
                        if (temp[i] && temp[i] != TextBackground && !(temp[i] is Text))
                        {
                            TextBackground = temp[i];
                            break;
                        }
                    }
                }
            }
        }
        protected void OnEnable()
        {
            SetStyle(TextStyle);
        }
        protected void LateUpdate()
        {
            if (AutoSetTextColor && m_Text && TextBackground)
            {
                BackgroundStyle = TextBackground.GetColorStyle();
                TextStyle = BackgroundStyle.Reversal();
                if (ShowTestGrayData)
                {
                    TestGrayData = new HSVColor(TextBackground.color.Graying()).Value;
                    //m_Text.text = TestGrayData.ToString("F3");
                }
                SetStyle(TextStyle);
            }
        }
        public void SetStyle(ColorStyle TextStyle)
        {
            if (!m_Text)
                return;
            m_Text.SetTextStyle(TextStyle);
        }
    }
    /// <summary>
    /// 颜色风格
    /// </summary>
    public enum ColorStyle
    {
        /// <summary>
        /// 浅色
        /// </summary>
        Light,
        /// <summary>
        /// 深色
        /// </summary>
        Dark,
    }
    public static partial class Extension
    {
        public static void SetTextStyle(this Text text, ColorStyle style)
        {
            if (text)
            {
                Color? color = SetColorByStyle(text.color, style);
                if (color.HasValue)
                    text.color = color.Value;
                Outline outline = text.GetOutline();
                if (outline && outline.enabled)
                {
                    color = SetColorByStyle(outline.effectColor, style.Reversal());
                    if (color.HasValue)
                        outline.effectColor = color.Value;
                }
            }
        }
        public static Color? SetColorByStyle(Color color, ColorStyle style)
        {
            Color? result = null;
            if (!color.Graying().g.JudgeRange(style == ColorStyle.Light ? 1 : 0, 0.1f))
                result = ColorAdjust.AdjustColor(color, style.ToColor(), ColorAdjust.ColorAdjustMode.RGB);
            return result;
        }
        public static void SetTextStyle(this Text[] texts, ColorStyle style)
        {
            if (texts.CheckArrayLength(1))
            {
                for (int i = 0; i < texts.Length; i++)
                    texts[i].SetTextStyle(style);
            }
        }
        public static void SetGraphicStyle(this Graphic graphic, ColorStyle style)
        {
            if (graphic)
            {
                Color? color = SetColorByStyle(graphic.color, style);
                if (color.HasValue)
                    graphic.color = color.Value;
            }
        }
        public static void SetGraphicStyle(this Graphic[] graphic, ColorStyle style)
        {
            if (graphic.CheckArrayLength(1))
            {
                for (int i = 0; i < graphic.Length; i++)
                    graphic[i].SetGraphicStyle(style);
            }
        }
        public static ColorStyle Reversal(this ColorStyle style)
        {
            return style == ColorStyle.Light ? ColorStyle.Dark : ColorStyle.Light;
        }
        public static Color ToColor(this ColorStyle style)
        {
            return style == ColorStyle.Light ? Color.white : Color.black;
        }
        public static ColorStyle GetColorStyle(this Graphic graphic)
        {
            ColorStyle result = ColorStyle.Light;
            if (graphic)
                result = ColorToColorStyle(graphic.color);
            return result;
        }
        public static ColorStyle ColorToColorStyle(this Color m_Color)
        {
            ColorStyle result = ColorStyle.Dark;
            HSVColor color = new HSVColor(m_Color.Graying());
            //Debug.Log(color.Value);
            if (color.Value >= StyleText.GrayValue)
                result = ColorStyle.Light;
            return result;
        }
    }
}
