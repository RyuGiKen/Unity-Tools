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
            if (ShowInspector)
            {
                base.OnInspectorGUI();
            }
            else
            {
                ValueAdjustButton button = target as ValueAdjustButton;
                //EditorGUILayout.LabelField("输出结果：" + button.Prefix + button.m_Value.ToString("F" + button.digit) + button.Postfix);
                EditorGUILayout.DelayedTextField(Name[1], button.Prefix + button.m_Value.ToString("F" + button.digit) + button.Postfix);
                EditorGUILayout.Space();
                button.LimitValue = EditorGUILayout.ToggleLeft(Name[2], button.LimitValue);
                if (button.LimitValue)
                {
                    button.m_Value = EditorGUILayout.Slider(Name[3], button.m_Value, button.ValueRange.x, button.ValueRange.y);
                    button.ValueRange = EditorGUILayout.Vector2Field(Name[4], button.ValueRange);
                }
                else
                {
                    button.m_Value = EditorGUILayout.FloatField(Name[3], button.m_Value);
                }
                EditorGUILayout.Space();
                button.digit = EditorGUILayout.IntSlider(Name[5], button.digit, 0, 8);
                EditorGUILayout.Space();
                button.Prefix = EditorGUILayout.DelayedTextField(Name[6], button.Prefix);
                button.Postfix = EditorGUILayout.DelayedTextField(Name[7], button.Postfix);
            }
        }
    }
}
