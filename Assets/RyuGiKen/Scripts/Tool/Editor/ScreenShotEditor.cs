using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(ScreenShot))]
    public class ScreenShotEditor : Editor
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
                    ButtonName[0] = "截图(场景视图)";
                    ButtonName[1] = "截图(游戏视图)";
                    break;
                default:
                    ButtonName[0] = "ScreenShot (Scene View)";
                    ButtonName[1] = "ScreenShot (Game View)";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                ScreenShot.PrintScreen((target as ScreenShot).fileName, false);
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                ScreenShot.PrintScreen((target as ScreenShot).fileName, true);
            }
        }
    }
}
