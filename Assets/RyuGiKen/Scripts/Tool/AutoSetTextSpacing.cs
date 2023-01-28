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
        public ValueRange lineSpacingRange = new ValueRange(1, 2);
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
            AdjustSpacing(m_Text, new ValueRange(1.2f, 2f), lineSpacingRange);
        }
        /// <summary>
        /// 自动间隔
        /// </summary>
        /// <param name="text"></param>
        public static void AdjustSpacing(Text text)
        {
            AdjustSpacing(text, new ValueRange(1.2f, 2f), new ValueRange(1, 2));
        }
        /// <summary>
        /// 自动间隔
        /// </summary>
        /// <param name="text"></param>
        /// <param name="preferredHeightRange"></param>
        /// <param name="lineSpacingRange"></param>
        public static void AdjustSpacing(Text text, ValueRange preferredHeightRange, ValueRange lineSpacingRange)
        {
            if (text)
            {
                if (text.cachedTextGenerator.lineCount <= 1)
                {
                    text.lineSpacing = 1;
                    return;
                }
                float preferredHeight = text.GetTextPreferredHeightForceLineSpacing1();
                text.lineSpacing = ValueAdjust.MappingRange(text.rectTransform.rect.height, preferredHeight * preferredHeightRange.Range, preferredHeightRange).Round(2);
            }
        }
    }
}
