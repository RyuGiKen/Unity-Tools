using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
using UnityEngine.EventSystems;

namespace RyuGiKen.Tools
{
    /// <summary>
    /// 数值调整按钮
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/数值调整按钮")]
    public class ValueAdjustButton : MonoBehaviour
    {
        [Tooltip("减少按钮")] public Button DecreaseButton;
        [Tooltip("增加按钮")] public Button IncreaseButton;
        [Tooltip("输入框")] public InputField m_InputField;
        [Tooltip("显示文本")] public Text m_Text;

        [SerializeField] private float value;
        [Space(10)]
        [Tooltip("按钮长按累加速度")] public float PressSpeed;
        [Tooltip("按钮长按时长")] public float PressTime = 0.5f;
        float lastPressTime;
        public float m_Value
        {
            set
            {
                this.value = value;
                UpdateText();
            }
            get
            {
                return LimitValue ? value.Clamp(ValueRange.x, ValueRange.y) : value;
            }
        }
        public string OutPut
        {
            get
            {
                return (Prefix + m_Value.ToString("F" + digit) + Postfix);
            }
        }
        [Space(10)]
        [Tooltip("精度")] public int digit;
        [Tooltip("限制")] public bool LimitValue;
        [Tooltip("值范围")] public Vector2 ValueRange;
        [Tooltip("调整幅度")] public float AdjustSize = 1;
        [Tooltip("前缀")] public string Prefix;
        [Tooltip("后缀")] public string Postfix;
        void Awake()
        {
            DecreaseButton.onClick.AddListener(ValueDecrease);
            IncreaseButton.onClick.AddListener(ValueIncrease);
        }
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
            if (m_InputField && m_Text)
            {
                m_InputField.text = value.ToString("F" + digit);
                m_InputField.textComponent.SetActive(true);
                m_Text.transform.parent.SetActive(false);
            }
        }
        public void HideInputField()
        {
            if (m_InputField && m_Text)
            {
                m_InputField.textComponent.SetActive(false);
                m_Text.transform.parent.SetActive(true);
            }
        }
        /// <summary>
        /// 更新显示文本
        /// </summary>
        public void UpdateText()
        {
            if (m_Text)
                m_Text.text = OutPut;
        }
        /// <summary>
        /// 输入框改变数值后
        /// </summary>
        public void ChangeValue()
        {
            float temp = (m_InputField ? m_InputField.text.ToFloat() : m_Value);
            m_Value = LimitValue ? temp.Clamp(ValueRange.x, ValueRange.y) : temp;
        }
        /// <summary>
        /// 值减少
        /// </summary>
        public void ValueDecrease()
        {
            float adjustSize = AdjustSize.Clamp(0);
            float minValue = ValueRange.x;
            if (LimitValue)
            {
                if (value < minValue + adjustSize)
                    value = minValue;//Mathf.RoundToInt((value - 5) / 5f) * 5;
                else if (value > minValue)
                    value -= adjustSize;
                else if (value < minValue)
                    value = minValue;
            }
            else
            {
                value -= adjustSize;
            }
            UpdateText();
        }
        /// <summary>
        /// 值增加
        /// </summary>
        public void ValueIncrease()
        {
            float adjustSize = AdjustSize.Clamp(0);
            float maxValue = ValueRange.y;
            if (LimitValue)
            {
                if (value > maxValue - adjustSize)
                    value = maxValue;//Mathf.RoundToInt((value + 5) / 5f) * 5;
                else if (value < maxValue)
                    value += adjustSize;
                else if (value > maxValue)
                    value = maxValue;
            }
            else
            {
                value += adjustSize;
            }
            UpdateText();
        }
        /// <summary>
        /// 值修改
        /// </summary>
        /// <param name="adjust"></param>
        public void ValueAdjust(float adjust)
        {
            if (!adjust.IsNaN())
                if (LimitValue)
                {
                    value = (value + adjust).Clamp(ValueRange.x, ValueRange.y);
                }
                else
                {
                    value += adjust;
                }
            UpdateText();
        }
        public void OnBeginEdit(BaseEventData eventData)
        {
            lastPressTime = Time.unscaledTime;
        }
        public void OnPressValueIncreaseButton(BaseEventData eventData)
        {
            if (Time.unscaledTime - lastPressTime > PressTime.Clamp(0) && PressSpeed > 0)
            {
                ValueAdjust(PressSpeed * Time.unscaledDeltaTime);
            }
        }
        public void OnPressValueDecreaseButton(BaseEventData eventData)
        {
            if (Time.unscaledTime - lastPressTime > PressTime.Clamp(0) && PressSpeed > 0)
            {
                ValueAdjust(-PressSpeed * Time.unscaledDeltaTime);
            }
        }
        public void OnEndEdit(BaseEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
