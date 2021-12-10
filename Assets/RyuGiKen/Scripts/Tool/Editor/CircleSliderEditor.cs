using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(CircleSlider), true)]
    [CanEditMultipleObjects]
    public class CircleSliderEditor : SelectableEditor
    {
        SerializedProperty m_Background;
        SerializedProperty m_FillRect;
        SerializedProperty m_HandleRect;
        SerializedProperty m_MinValue;
        SerializedProperty m_MaxValue;
        SerializedProperty m_Value;
        SerializedProperty m_OnValueChanged;
        SerializedProperty m_WholeNumbers;

        SerializedProperty m_MinAngle;
        SerializedProperty m_MaxAngle;
        SerializedProperty m_FillAngleRange;
        SerializedProperty fillClockwise;
        SerializedProperty fillOrigin;
        SerializedProperty HandleFilled;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Background = serializedObject.FindProperty("m_Background");
            m_FillRect = serializedObject.FindProperty("m_FillImage");
            m_HandleRect = serializedObject.FindProperty("m_Handle");

            m_MinValue = serializedObject.FindProperty("m_MinValue");
            m_MaxValue = serializedObject.FindProperty("m_MaxValue");
            m_Value = serializedObject.FindProperty("m_Value");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
            m_WholeNumbers = serializedObject.FindProperty("m_WholeNumbers");

            m_MinAngle = serializedObject.FindProperty("m_MinAngle");
            m_MaxAngle = serializedObject.FindProperty("m_MaxAngle");
            m_FillAngleRange = serializedObject.FindProperty("m_FillAngleRange");
            fillClockwise = serializedObject.FindProperty("fillClockwise");
            fillOrigin = serializedObject.FindProperty("fillOrigin");
            HandleFilled = serializedObject.FindProperty("HandleFilled");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Background);
            EditorGUILayout.PropertyField(m_FillRect);
            EditorGUILayout.PropertyField(m_HandleRect);

            if (m_FillRect.objectReferenceValue != null || m_HandleRect.objectReferenceValue != null)
            {
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                float newMin = EditorGUILayout.FloatField("Min Value", m_MinValue.floatValue);
                if (EditorGUI.EndChangeCheck() && newMin <= m_MaxValue.floatValue)
                {
                    m_MinValue.floatValue = newMin;
                }

                EditorGUI.BeginChangeCheck();
                float newMax = EditorGUILayout.FloatField("Max Value", m_MaxValue.floatValue);
                if (EditorGUI.EndChangeCheck() && newMax >= m_MinValue.floatValue)
                {
                    m_MaxValue.floatValue = newMax;
                }

                EditorGUILayout.PropertyField(m_WholeNumbers);
                EditorGUILayout.Slider(m_Value, (target as CircleSlider).MinValue, (target as CircleSlider).MaxValue);
                EditorGUILayout.Space();

                /*EditorGUI.BeginChangeCheck();
                newMin = EditorGUILayout.FloatField("Min Angle", m_MinAngle.floatValue);
                if (EditorGUI.EndChangeCheck() && newMin <= m_MaxAngle.floatValue)
                {
                    m_MinAngle.floatValue = newMin;
                }

                EditorGUI.BeginChangeCheck();
                newMax = EditorGUILayout.FloatField("Max Angle", m_MaxAngle.floatValue);
                if (EditorGUI.EndChangeCheck() && newMax >= m_MinAngle.floatValue)
                {
                    m_MaxAngle.floatValue = newMax;
                }*/
                float temp;
                temp = EditorGUILayout.Slider("Min Angle", m_MinAngle.floatValue, 0, 360);
                if (temp <= m_MaxAngle.floatValue)
                    m_MinAngle.floatValue = temp.Clamp(0, 359.999f);
                temp = EditorGUILayout.Slider("Max Angle", m_MaxAngle.floatValue, 0, 360);
                if (temp >= m_MinAngle.floatValue)
                    m_MaxAngle.floatValue = temp.Clamp(0, 359.999f);

                EditorGUILayout.Slider("Angle", (target as CircleSlider).Angle, (target as CircleSlider).MinAngle, (target as CircleSlider).MaxAngle);
                EditorGUILayout.Slider(m_FillAngleRange, 0, (target as CircleSlider).MaxAngle - (target as CircleSlider).MinAngle);
                EditorGUILayout.PropertyField(fillClockwise);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(HandleFilled);
                if (EditorGUI.EndChangeCheck())
                {
                    CircleSlider.HandleFilledType type = (CircleSlider.HandleFilledType)HandleFilled.enumValueIndex;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        CircleSlider slider = obj as CircleSlider;
                        slider.HandleFilled = type;
                    }
                }
                fillOrigin.intValue = (int)(Image.Origin360)EditorGUILayout.EnumPopup("Fill Origin", (Image.Origin360)fillOrigin.intValue);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
