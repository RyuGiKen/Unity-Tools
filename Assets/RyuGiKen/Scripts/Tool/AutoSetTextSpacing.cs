using System.Collections;
using System.Collections.Generic;
using RyuGiKen;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 自动间隔
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class AutoSetTextSpacing : MonoBehaviour
    {
        public Text m_Text;
        //public ValueRange lineSpacingRange = new ValueRange(1, 2);
        void Awake()
        {
            m_Text = this.GetComponent<Text>();
        }
        void Reset()
        {
            m_Text = this.GetComponent<Text>();
        }
        void LateUpdate()
        {
            AdjustSpacing(m_Text, new ValueRange(1.2f, 2f));
        }
        /// <summary>
        /// 自动间隔
        /// </summary>
        /// <param name="text"></param>
        public static void AdjustSpacing(Text text)
        {
            AdjustSpacing(text, new ValueRange(1.2f, 2f));
        }
        /// <summary>
        /// 自动间隔
        /// </summary>
        /// <param name="text"></param>
        /// <param name="preferredHeightRange"></param>
        public static void AdjustSpacing(Text text, ValueRange preferredHeightRange)
        {
            if (text)
            {
                if (text.cachedTextGenerator.lineCount <= 1)
                {
                    text.lineSpacing = 1;
                    return;
                }
                float preferredHeight = text.GetTextPreferredHeightForceLineSpacing1();
                float lineSpacing = ValueAdjust.MappingRange(text.rectTransform.rect.height, preferredHeight * preferredHeightRange.Range, preferredHeightRange).Round(2);
                if (!Mathf.Approximately(text.lineSpacing, lineSpacing))
                    text.lineSpacing = lineSpacing;
            }
        }
    }
}
