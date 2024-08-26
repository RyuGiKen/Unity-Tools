using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// ¸¡¶¯µ¯´°
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class PromptText : MonoBehaviour
    {
        public bool show;
        public RectTransform rectTransform;
        public Image ConText;
        public Text m_Text;
        public RectTransform ShowPos;
        public RectTransform HidePos;
        protected virtual void Reset()
        {
            m_Text = this.GetComponentInChildren<Text>();
            rectTransform = this.GetComponent<RectTransform>();
        }
        protected virtual void OnEnable()
        {
            Hide();
        }
        protected virtual void Start()
        {

        }
        public virtual void SetDirection(bool isLeft = true, uint height = 70)
        {
            rectTransform.pivot = new Vector2(isLeft ? 0 : 1, 0);
            rectTransform.anchorMin = new Vector2(isLeft ? 0 : 1, 0);
            rectTransform.anchorMax = new Vector2(isLeft ? 0 : 1, 0);
            rectTransform.anchoredPosition = new Vector2(0, height);
            if (ConText)
            {
                ConText.rectTransform.anchorMin = new Vector2(isLeft ? 0 : 1, 0);
                ConText.rectTransform.anchorMax = new Vector2(isLeft ? 0 : 1, 0);
                ConText.rectTransform.localScale = new Vector2(isLeft ? -1 : 1, 1);
            }
            if (m_Text)
            {
                m_Text.rectTransform.localScale = new Vector2(isLeft ? -1 : 1, 1);
                m_Text.alignment = isLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
            }
        }
        protected virtual void Update()
        {

        }
        protected virtual void LateUpdate()
        {
            if (ConText && m_Text)// && !rectTransform.rect.width.JudgeRange(m_Text.preferredWidth + 90, 50))
                ConText.rectTransform.sizeDelta = new Vector2((m_Text.preferredWidth + 100).Clamp(300, 1000), ConText.rectTransform.sizeDelta.y);

            if (ConText && ShowPos && HidePos)
            {
                ConText.rectTransform.anchoredPosition = Vector2.Lerp(ConText.rectTransform.anchoredPosition, show ? ShowPos.anchoredPosition : HidePos.anchoredPosition, Time.unscaledDeltaTime);
            }
        }
        protected virtual void UpdateText(string str)
        {
            m_Text.text = str;
        }
        public virtual void Show()
        {
            if (ConText && ShowPos)
            {
                ConText.rectTransform.anchoredPosition = ShowPos.anchoredPosition;
            }
        }
        public virtual void Hide()
        {
            if (ConText && HidePos)
            {
                ConText.rectTransform.anchoredPosition = HidePos.anchoredPosition;
            }
        }
    }
}
