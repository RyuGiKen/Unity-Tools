using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RyuGiKen;
using RyuGiKen.Tools;
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
        public struct Parameter
        {
            public int digit;
            public bool limitValue;
            public ValueRange valueRange;
            public float adjustSize;
            public float pressSpeed;
            public float pressTime;
            public string prefix;
            public string postfix;
            public void Initialize()
            {
                digit = 0;
                limitValue = false;
                valueRange = new ValueRange(0, 1);
                adjustSize = 1;
                pressSpeed = 0;
                pressTime = 0.5f;
                prefix = "";
                postfix = "";
            }
        }
        public float m_Value
        {
            set
            {
                //this.value = value;
                onValueChanged.Invoke(value);
            }
            get
            {
                return (LimitValue ? value.Clamp(m_ValueRange) : value).Round(digit);
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
        [Tooltip("精度")][SerializeField] int digit;
        public int Digit
        {
            set
            {
                digit = value;
                UpdateText();
            }
            get
            {
                return digit;
            }
        }
        [Tooltip("限制")][SerializeField] bool limitValue;
        public bool LimitValue
        {
            set
            {
                limitValue = value;
                onValueChanged.Invoke(this.value);
            }
            get
            {
                return limitValue;
            }
        }
        [Tooltip("值范围")][SerializeField] Vector2 valueRange;
        public ValueRange m_ValueRange
        {
            set
            {
                valueRange = value;
                if (!this.value.InRange(valueRange))
                    onValueChanged.Invoke(this.value);
            }
            get
            {
                return valueRange;
            }
        }
        [Tooltip("调整幅度")] public float AdjustSize = 1;
        [Tooltip("前缀")] public string Prefix;
        [Tooltip("后缀")] public string Postfix;
        [System.Serializable]
        public class ValueAdjustEvent : UnityEvent<float> { }
        [SerializeField]
        private ValueAdjustEvent m_OnValueChanged = new ValueAdjustEvent();
        public ValueAdjustEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }
        public event MyEvent OnValueChangedEvent;
        void Awake()
        {
            DecreaseButton.onClick.AddListener(ValueDecrease);
            IncreaseButton.onClick.AddListener(ValueIncrease);
            m_InputField.onSubmit.AddListener(InputChangeValue);
            onValueChanged.AddListener(ChangeValue);
            OnValueChangedEvent = ChangeValue;
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
        /// 设置数值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Set(float value, float min, float max)
        {
            this.valueRange = new ValueRange(min, max);
            m_Value = value;
        }
        /// <summary>
        /// 设置数值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueRange"></param>
        public void Set(float value, ValueRange valueRange)
        {
            this.valueRange = valueRange;
            m_Value = value;
        }
        /// <summary>
        /// 设置范围
        /// </summary>
        /// <param name="value"></param>
        public void Set(ValueInRange value)
        {
            valueRange = value.Range;
            m_Value = value.Value;
        }
        /// <summary>
        /// 设置范围
        /// </summary>
        /// <param name="value"></param>
        public void Set(ValueWithRange value)
        {
            valueRange = value.Range;
            m_Value = value.Value;
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="parameter"></param>
        public void SetPararmeter(Parameter parameter)
        {
            this.digit = parameter.digit;
            this.limitValue = parameter.limitValue;
            this.valueRange = parameter.valueRange;
            this.AdjustSize = parameter.adjustSize;
            this.PressSpeed = parameter.pressSpeed;
            this.PressTime = parameter.pressTime;
            this.Prefix = parameter.prefix;
            this.Postfix = parameter.postfix;

            onValueChanged.Invoke(this.value);
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="digit"></param>
        /// <param name="limitValue"></param>
        /// <param name="valueRange"></param>
        /// <param name="adjustSize"></param>
        /// <param name="pressSpeed"></param>
        /// <param name="pressTime"></param>
        public void SetPararmeter(int? digit = null, bool? limitValue = null, ValueRange? valueRange = null, float? adjustSize = null, float? pressSpeed = null, float? pressTime = null)
        {
            if (digit.HasValue)
                this.digit = digit.Value;
            if (limitValue.HasValue)
                this.limitValue = limitValue.Value;
            if (valueRange.HasValue)
                this.valueRange = valueRange.Value;
            if (adjustSize.HasValue)
                this.AdjustSize = adjustSize.Value;
            if (pressSpeed.HasValue)
                this.PressSpeed = pressSpeed.Value;
            if (pressTime.HasValue)
                this.PressTime = pressTime.Value;

            onValueChanged.Invoke(this.value);
        }
        public void SetPrefixPostfix(string prefix, string postfix)
        {
            this.Prefix = prefix;
            this.Postfix = postfix;

            UpdateText();
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
        /// <param name="str"></param>
        public void InputChangeValue(string str)
        {
            float temp = (m_InputField ? m_InputField.text.ToFloat() : m_Value);
            m_Value = LimitValue ? temp.Clamp(m_ValueRange) : temp;
        }
        void ChangeValue() { }
        void ChangeValue(float newValue)
        {
            value = LimitValue ? newValue.Clamp(m_ValueRange) : newValue;
            UpdateText();
            OnValueChangedEvent();
        }
        /// <summary>
        /// 值减少
        /// </summary>
        public void ValueDecrease()
        {
            float adjustSize = AdjustSize.Clamp(0);
            float minValue = m_ValueRange.MinValue;
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
            onValueChanged.Invoke(value);
        }
        /// <summary>
        /// 值增加
        /// </summary>
        public void ValueIncrease()
        {
            float adjustSize = AdjustSize.Clamp(0);
            float maxValue = m_ValueRange.MaxValue;
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
            onValueChanged.Invoke(value);
        }
        /// <summary>
        /// 值修改
        /// </summary>
        /// <param name="adjust"></param>
        public void ValueAdjust(float adjust)
        {
            if (!adjust.IsNaN() && adjust != 0)
                if (LimitValue)
                {
                    value = (value + adjust).Clamp(m_ValueRange);
                }
                else
                {
                    value += adjust;
                }
            onValueChanged.Invoke(value);
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
namespace RyuGiKenEditor.Tools
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ValueAdjustButton), true)]
    public class ValueAdjustButtonEditor : Editor
    {
        SerializedProperty value;
        SerializedProperty digit;
        SerializedProperty limitValue;
        SerializedProperty valueRange;
        void OnEnable()
        {
            value = serializedObject.FindProperty("value");
            digit = serializedObject.FindProperty("digit");
            limitValue = serializedObject.FindProperty("limitValue");
            valueRange = serializedObject.FindProperty("valueRange");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] Name = new string[5];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Name[0] = "限制";
                    Name[1] = "值";
                    Name[2] = "范围";
                    Name[3] = "精度";
                    Name[4] = "输出结果：";
                    break;
                default:
                    Name[0] = "Limit Range";
                    Name[1] = "Value";
                    Name[2] = "Range";
                    Name[3] = "Digit";
                    Name[4] = "Output：";
                    break;
            }
            //serializedObject.Update();

            ValueAdjustButton button = target as ValueAdjustButton;
            limitValue.boolValue = EditorGUILayout.ToggleLeft(Name[0], limitValue.boolValue);
            if (button.LimitValue)
            {
                value.floatValue = EditorGUILayout.Slider(Name[1], value.floatValue, button.m_ValueRange.MinValue, button.m_ValueRange.MaxValue);
                valueRange.vector2Value = EditorGUILayout.Vector2Field(Name[1], valueRange.vector2Value);
            }
            else
            {
                value.floatValue = EditorGUILayout.FloatField(Name[1], value.floatValue);
            }
            EditorGUILayout.Space();
            digit.intValue = EditorGUILayout.IntSlider(Name[3], digit.intValue, 0, 8);
            EditorGUILayout.Space();
            string temp = EditorGUILayout.TextField(Name[4], button.OutPut);
            if (button.m_Text && button.m_Text.text != temp)
                button.UpdateText();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
