using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
namespace RyuGiKen.Tools
{
    [DisallowMultipleComponent]
    public class ValueAdjustButton : MonoBehaviour
    {
        [Tooltip("减少按钮")] public Button DecreaseButton;
        [Tooltip("增加按钮")] public Button IncreaseButton;
        [Tooltip("输入框")] public InputField m_InputField;
        [Tooltip("显示文本")] public Text m_Text;

        public float value;
        [Tooltip("精度")] public int digit;
        [Tooltip("限制")] public bool LimitValue;
        [Tooltip("值范围")] public Vector2 ValueRange;
        [Tooltip("调整幅度")] public float AdjustSize = 1;
        [Tooltip("前缀")] public string Prefix;
        [Tooltip("后缀")] public string Postfix;

        void Start()
        {
            HideInputField();
            UpdateText();
        }
        void Update()
        {

        }
        public void ShowInputField()
        {
            m_InputField.text = value.ToString("F" + digit);
            //m_InputField.SetActive(true);
            m_Text.transform.parent.SetActive(false);
        }
        public void HideInputField()
        {
            //m_InputField.SetActive(false);
            m_Text.transform.parent.SetActive(true);
        }
        /// <summary>
        /// 更新显示文本
        /// </summary>
        public void UpdateText()
        {
            m_Text.text = Prefix + value.ToString("F" + digit) + Postfix;
        }
        /// <summary>
        /// 输入框改变数值后
        /// </summary>
        public void ChangeValue()
        {
            value = m_InputField.text.ToInteger().Clamp(ValueRange.x, ValueRange.y);
            UpdateText();
        }
        /// <summary>
        /// 值减少
        /// </summary>
        public void ValueDecrease()
        {
            float minAngle = ValueRange.x;
            if (LimitValue)
            {
                if (value < minAngle + AdjustSize)
                    value = minAngle;//Mathf.RoundToInt((value - 5) / 5f) * 5;
                else if (value > minAngle)
                    value -= AdjustSize;
                else if (value < minAngle)
                    value = minAngle;
            }
            else
            {
                value -= AdjustSize;
            }
            UpdateText();
        }
        /// <summary>
        /// 值增加
        /// </summary>
        public void ValueIncrease()
        {
            float maxAngle = ValueRange.y;
            if (LimitValue)
            {
                if (value > maxAngle - AdjustSize)
                    value = maxAngle;//Mathf.RoundToInt((value + 5) / 5f) * 5;
                else if (value < maxAngle)
                    value += AdjustSize;
                else if (value > maxAngle)
                    value = maxAngle;
            }
            else
            {
                value += AdjustSize;
            }
            UpdateText();
        }
    }
}
