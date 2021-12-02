using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ValueAdjustButton))]
    public class ValueAdjustButtonEditor : Editor
    {
        bool ShowInspector = false;
        SerializedProperty value;
        SerializedProperty digit;
        SerializedProperty LimitValue;
        SerializedProperty ValueRange;
        SerializedProperty Prefix;
        SerializedProperty Postfix;
        void OnEnable()
        {
            value = serializedObject.FindProperty("value");
            digit = serializedObject.FindProperty("digit");
            LimitValue = serializedObject.FindProperty("LimitValue");
            ValueRange = serializedObject.FindProperty("ValueRange");
            Prefix = serializedObject.FindProperty("Prefix");
            Postfix = serializedObject.FindProperty("Postfix");
        }
        public override void OnInspectorGUI()
        {
            string[] Name = new string[8];
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
                    value.floatValue = EditorGUILayout.Slider(Name[3], value.floatValue, button.ValueRange.x, button.ValueRange.y);
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

                if (button.m_Text && button.m_Text.text != temp)
                    button.UpdateText();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
