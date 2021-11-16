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
            if (ShowInspector)
            {
                base.OnInspectorGUI();
            }
            else
            {
                FPSShow fps = target as FPSShow;
                fps.hide = !EditorGUILayout.ToggleLeft(Name[1], !fps.hide);
                EditorGUILayout.IntField(Name[2], (int)fps.m_FPS);
                fps.UnscaledDeltaTime = EditorGUILayout.Toggle(Name[3], fps.UnscaledDeltaTime);
                fps.FPSLog = EditorGUILayout.Toggle(Name[4], fps.FPSLog);
                fps.LockFrameRate = EditorGUILayout.IntField(Name[5], fps.LockFrameRate);
                fps.autoAdjustQualityLevel = EditorGUILayout.ToggleLeft(Name[6], fps.autoAdjustQualityLevel);
                if (fps.autoAdjustQualityLevel)
                {
                    fps.adjustMinFPS = EditorGUILayout.IntField(Name[7], fps.adjustMinFPS);
                    fps.adjustMaxFPS = EditorGUILayout.IntField(Name[7], fps.adjustMaxFPS.Clamp(fps.adjustMinFPS));
                }
            }
        }
    }
}
