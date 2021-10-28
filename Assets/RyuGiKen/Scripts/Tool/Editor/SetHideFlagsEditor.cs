using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(SetHideFlags))]
    public class SetHideFlagsEditor : Editor
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
                    ButtonName[0] = "设置目标的HideFlags";
                    ButtonName[1] = "设置所有对象的HideFlags";
                    break;
                default:
                    ButtonName[0] = "Set Target HideFlags";
                    ButtonName[1] = "Set All Objects HideFlags";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as SetHideFlags).SetTarget();
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                (target as SetHideFlags).SetAll();
            }
        }
    }
}