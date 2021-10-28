using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace WindowsAPI
{
    [CustomEditor(typeof(OpenKey))]
    public class OpenKeyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[2];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "调出屏幕键盘";
                    ButtonName[1] = "隐藏屏幕键盘";
                    break;
                default:
                    ButtonName[0] = "Open Keyboard";
                    ButtonName[1] = "Hide Keyboard";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as OpenKey).OpenKeyClick();
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                (target as OpenKey).HideKey();
            }
        }
    }
}