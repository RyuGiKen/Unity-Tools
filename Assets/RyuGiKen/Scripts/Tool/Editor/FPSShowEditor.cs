using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FPSShow))]
    public class FPSShowEditor : Editor
    {
        bool ShowInspector = false;
        SerializedProperty hide;
        SerializedProperty UnscaledDeltaTime;
        SerializedProperty FPSLog;
        SerializedProperty LockFrameRate;
        SerializedProperty autoAdjustQualityLevel;
        SerializedProperty adjustMinFPS;
        SerializedProperty adjustMaxFPS;
        void OnEnable()
        {
            hide = serializedObject.FindProperty("hide");
            UnscaledDeltaTime = serializedObject.FindProperty("UnscaledDeltaTime");
            FPSLog = serializedObject.FindProperty("FPSLog");
            LockFrameRate = serializedObject.FindProperty("LockFrameRate");
            autoAdjustQualityLevel = serializedObject.FindProperty("autoAdjustQualityLevel");
            adjustMinFPS = serializedObject.FindProperty("adjustMinFPS");
            adjustMaxFPS = serializedObject.FindProperty("adjustMaxFPS");
        }
        public override void OnInspectorGUI()
        {
            string[] Name = new string[9];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    Name[0] = "显示值";
                    Name[1] = "显示";
                    Name[2] = "FPS：";
                    Name[3] = "不受时间缩放影响";
                    Name[4] = "打印平均帧率日志";
                    Name[5] = "限制帧率";
                    Name[6] = "根据帧数自动切换画质(连续10s)";
                    Name[7] = "最小帧率";
                    Name[8] = "最大帧率";
                    break;
                default:
                    Name[0] = "Show Inspector";
                    Name[1] = "Display";
                    Name[2] = "FPS：";
                    Name[3] = "Unscaled DeltaTime";
                    Name[4] = "Log";
                    Name[5] = "Limit FrameRate";
                    Name[6] = "Auto Adjust QualityLevel";
                    Name[7] = "Min FrameRate";
                    Name[8] = "Max FrameRate";
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
                FPSShow fps = target as FPSShow;
                hide.boolValue = !EditorGUILayout.ToggleLeft(Name[1], !hide.boolValue);
                EditorGUILayout.IntField(Name[2], (int)fps.m_FPS);
                UnscaledDeltaTime.boolValue = EditorGUILayout.Toggle(Name[3], UnscaledDeltaTime.boolValue);
                FPSLog.boolValue = EditorGUILayout.Toggle(Name[4], FPSLog.boolValue);
                LockFrameRate.intValue = EditorGUILayout.DelayedIntField(Name[5], LockFrameRate.intValue.Clamp(-1));
                autoAdjustQualityLevel.boolValue = EditorGUILayout.ToggleLeft(Name[6], autoAdjustQualityLevel.boolValue);
                if (fps.autoAdjustQualityLevel)
                {
                    adjustMinFPS.intValue = EditorGUILayout.DelayedIntField(Name[7], adjustMinFPS.intValue.Clamp(0));
                    adjustMaxFPS.intValue = EditorGUILayout.DelayedIntField(Name[7], adjustMaxFPS.intValue.Clamp(adjustMinFPS.intValue + 1));
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
