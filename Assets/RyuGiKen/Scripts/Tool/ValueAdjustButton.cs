using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        public float m_Value
        {
            set
            {
                this.value = value;
                UpdateText();
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
        [Tooltip("精度")] public int digit;
        [Tooltip("限制")] public bool LimitValue;
        [Tooltip("值范围")][SerializeField] Vector2 valueRange;
        public ValueRange m_ValueRange
        {
            set
            {
                valueRange = value;
                UpdateText();
            }
            get
            {
                return new ValueRange(valueRange);
            }
        }
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
            m_Value = LimitValue ? temp.Clamp(m_ValueRange) : temp;
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
            UpdateText();
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
                    value = (value + adjust).Clamp(m_ValueRange);
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
namespace RyuGiKenEditor.Tools
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ValueAdjustButton), true)]
    public class ValueAdjustButtonEditor : Editor
    {
        bool ShowInspector = false;
        SerializedProperty PressSpeed;
        SerializedProperty PressTime;
        SerializedProperty value;
        SerializedProperty digit;
        SerializedProperty LimitValue;
        SerializedProperty ValueRange;
        SerializedProperty Prefix;
        SerializedProperty Postfix;
        void OnEnable()
        {
            PressSpeed = serializedObject.FindProperty("PressSpeed");
            PressTime = serializedObject.FindProperty("PressTime");
            value = serializedObject.FindProperty("value");
            digit = serializedObject.FindProperty("digit");
            LimitValue = serializedObject.FindProperty("LimitValue");
            ValueRange = serializedObject.FindProperty("valueRange");
            Prefix = serializedObject.FindProperty("Prefix");
            Postfix = serializedObject.FindProperty("Postfix");
        }
        public override void OnInspectorGUI()
        {
            string[] Name = new string[10];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Name[0] = "显示值";
                    Name[1] = "输出结果：";
                    Name[2] = "限制";
                    Name[3] = "值";
                    Name[4] = "范围";
                    Name[5] = "精度";
                    Name[6] = "前缀";
                    Name[7] = "后缀";
                    Name[8] = "长按累加";
                    Name[9] = "长按间隔";
                    break;
                default:
                    Name[0] = "Show Inspector";
                    Name[1] = "Output：";
                    Name[2] = "Limit Range";
                    Name[3] = "Value";
                    Name[4] = "Range";
                    Name[5] = "Digit";
                    Name[6] = "Prefix";
                    Name[7] = "Postfix";
                    Name[8] = "Hold Press";
                    Name[9] = "Time";
                    break;
            }
            ShowInspector = EditorGUILayout.Foldout(ShowInspector, Name[0]);
            serializedObject.Update();
            if (ShowInspector)
            {
                base.OnInspectorGUI();
            }
            else
            {
                ValueAdjustButton button = target as ValueAdjustButton;
                //EditorGUILayout.LabelField("输出结果：" + button.Prefix + button.m_Value.ToString("F" + button.digit) + button.Postfix);
                string temp = EditorGUILayout.TextField(Name[1], Prefix.stringValue + value.floatValue.ToString("F" + digit.intValue) + Postfix.stringValue);
                EditorGUILayout.Space();
                LimitValue.boolValue = EditorGUILayout.ToggleLeft(Name[2], LimitValue.boolValue);
                if (button.LimitValue)
                {
                    value.floatValue = EditorGUILayout.Slider(Name[3], value.floatValue, button.m_ValueRange.MinValue, button.m_ValueRange.MaxValue);
                    ValueRange.vector2Value = EditorGUILayout.Vector2Field(Name[4], ValueRange.vector2Value);
                }
                else
                {
                    value.floatValue = EditorGUILayout.FloatField(Name[3], value.floatValue);
                }
                EditorGUILayout.Space();
                digit.intValue = EditorGUILayout.IntSlider(Name[5], digit.intValue, 0, 8);
                EditorGUILayout.Space();
                Prefix.stringValue = EditorGUILayout.TextField(Name[6], Prefix.stringValue);
                Postfix.stringValue = EditorGUILayout.TextField(Name[7], Postfix.stringValue);

                PressSpeed.floatValue = EditorGUILayout.FloatField(Name[8], PressSpeed.floatValue);
                if (button.PressSpeed > 0)
                {
                    PressTime.floatValue = EditorGUILayout.FloatField(Name[9], PressTime.floatValue.Clamp(0));
                }

                if (button.m_Text && button.m_Text.text != temp)
                    button.UpdateText();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
