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
        [Header("同尺寸其他文本框")]
        [SerializeField] protected Text[] OtherTexts_SameGroup;
        //public ValueRange lineSpacingRange = new ValueRange(1, 2);
        void Awake()
        {
            if (!m_Text)
                m_Text = this.GetComponent<Text>();
        }
        void Reset()
        {
            m_Text = this.GetComponent<Text>();
        }
        void LateUpdate()
        {
            float lineSpacing = AdjustSpacing(m_Text, new ValueRange(1.2f, 2f));
            if (OtherTexts_SameGroup.CheckArrayLength(1))
            {
                for (int i = 0; i < OtherTexts_SameGroup.Length; i++)
                {
                    if (OtherTexts_SameGroup[i])
                        OtherTexts_SameGroup[i].lineSpacing = lineSpacing;
                }
            }
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
        public static float AdjustSpacing(Text text, ValueRange preferredHeightRange)
        {
            float result = -1;
            if (text)
            {
                result = 1;
                if (text.cachedTextGenerator.lineCount <= 1)
                {
                    result = 1;
                }
                else
                {
                    float preferredHeight = text.GetTextPreferredHeightForceLineSpacing1();
                    result = ValueAdjust.MappingRange(text.rectTransform.rect.height, preferredHeight * preferredHeightRange.Range, preferredHeightRange).Round(2);
                }
                if (!Mathf.Approximately(text.lineSpacing, result))
                    text.lineSpacing = result;
            }
            return result;
        }
    }
}
