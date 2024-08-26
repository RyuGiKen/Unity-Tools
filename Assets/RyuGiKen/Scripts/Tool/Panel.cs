using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class Panel : MonoBehaviour
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
            /// 纯色+窄边
            /// </summary>
            SolidColorAndNarrowEdge,
            /// <summary>
            /// 纯色+厚边
            /// </summary>
            SolidColorAndFatEdge,
            /// <summary>
            /// 渐变+厚边
            /// </summary>
            GradientAndFatEdge,
        }
        public Style style = Style.GradientAndFatEdge;
        public Image PanelImage;
        public Image OutlineImage;
        [SerializeField] protected Text[] texts;
        public bool AutoSetTextColor;
        public MultiArraySprite sprites = new MultiArraySprite(new Sprite[4][]);
        protected void Awake()
        {
            if (!PanelImage)
                PanelImage = this.GetComponent<Image>();
            if (!OutlineImage)
                OutlineImage = this.transform.GetChild(0)?.GetComponent<Image>();
        }
        protected void Reset()
        {
            Awake();
            sprites = new MultiArraySprite(new Sprite[4, 2].ConvertArray());
        }
        protected void OnEnable()
        {
            texts = FindTextAutoSetColorInChildren();
        }
        public Text[] FindTextAutoSetColorInChildren()
        {
            List<Text> temp = this.GetDescendants().ToComponent<Text>().ToList().ClearRepeatingItem();
            if (!temp.CheckListLength(1))
                return null;
            for (int i = temp.Count - 1; i >= 0; i--)
            {
                if (!temp[i])
                    continue;

                temp[i].TryGetComponent(out StyleText styleText);
                if (styleText && styleText.AutoSetTextColor && styleText.TextBackground)
                {
                    temp.RemoveAt(i);
                }
            }
            return temp.ToArray();
        }
        public void AutoSetTextColorByStyle()
        {
            SetTextStyle(GetColorStyle().Reversal());
        }
        public void SetTextStyle(ColorStyle style)
        {
            texts.SetTextStyle(style);
            if (texts.CheckArrayLength(1))
            {
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i])
                    {
                        texts[i].TryGetComponent(out StyleText styleText);
                        if (styleText && styleText.AutoSetTextColor && styleText.TextBackground) { }
                        else
                        {
                            texts[i].SetTextStyle(style);
                        }
                    }
                }
            }
        }
        public ColorStyle GetColorStyle()
        {
            return Extension.GetColorStyle(PanelImage);
        }
        protected void LateUpdate()
        {
            switch (style)
            {
                case Style.SolidColor:
                    if (!PanelImage.enabled)
                        PanelImage.enabled = true;
                    if (OutlineImage.enabled)
                        OutlineImage.enabled = false;
                    break;
                case Style.SolidColorAndNarrowEdge:
                case Style.SolidColorAndFatEdge:
                case Style.GradientAndFatEdge:
                    if (!PanelImage.enabled)
                        PanelImage.enabled = true;
                    if (!OutlineImage.enabled)
                        OutlineImage.enabled = true;
                    break;
            }
            switch (style)
            {
                default:
                    Sprite sprite = sprites.GetItem((int)style, 0);
                    if (sprite)
                    {
                        if (PanelImage.sprite != sprite)
                            PanelImage.sprite = sprite;
                    }
                    else if (PanelImage.enabled)
                        PanelImage.enabled = false;

                    sprite = sprites.GetItem((int)style, 1);
                    if (sprite)
                    {
                        if (OutlineImage.sprite != sprite)
                            OutlineImage.sprite = sprite;
                    }
                    else if (OutlineImage.enabled)
                        OutlineImage.enabled = false;
                    break;
            }
            if (AutoSetTextColor)
                AutoSetTextColorByStyle();
        }
    }
}
