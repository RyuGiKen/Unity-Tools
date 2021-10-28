using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(TransformSync))]
    public class TransformSyncEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[8];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "获取A[0]的所有子孙";
                    ButtonName[1] = "获取A的所有子孙";
                    ButtonName[2] = "获取B[0]的所有子孙";
                    ButtonName[3] = "获取B的所有子孙";
                    ButtonName[4] = "移除重复项";
                    ButtonName[5] = "重复项置空";
                    ButtonName[6] = "复制A到B";
                    ButtonName[7] = "复制B到A";
                    break;
                default:
                    ButtonName[0] = "Get Descendants Form A[0]";
                    ButtonName[1] = "Get Descendants Form A";
                    ButtonName[2] = "Get Descendants Form B[0]";
                    ButtonName[3] = "Get Descendants Form B";
                    ButtonName[4] = "Remove Duplicates";
                    ButtonName[5] = "Set Repeating Null";
                    ButtonName[6] = "Sync A to B";
                    ButtonName[7] = "Sync B to A";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as TransformSync).GetDescendants_A0();
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                (target as TransformSync).GetDescendants_A();
            }
            if (GUILayout.Button(ButtonName[2]))
            {
                (target as TransformSync).GetDescendants_B0();
            }
            if (GUILayout.Button(ButtonName[3]))
            {
                (target as TransformSync).GetDescendants_B();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button(ButtonName[4]))
            {
                (target as TransformSync).ClearRepeating();
            }
            if (GUILayout.Button(ButtonName[5]))
            {
                (target as TransformSync).SetRepeatingNull();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button(ButtonName[6]))
            {
                (target as TransformSync).AtoB();
            }
            if (GUILayout.Button(ButtonName[7]))
            {
                (target as TransformSync).BtoA();
            }
        }
    }
}