using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(CinemachinePathPositon))]
    public class CinemachinePathPositonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[3];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "批量移除子对象";
                    ButtonName[1] = "调整子对象数量";
                    ButtonName[2] = "刷新子对象位置";
                    break;
                default:
                    ButtonName[0] = "Destroy Children";
                    ButtonName[1] = "Adjust Child Count";
                    ButtonName[2] = "Update Children";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as CinemachinePathPositon).ClearChildren();
            }
            if (!(target as CinemachinePathPositon).UpdateAlways && GUILayout.Button(ButtonName[1]))
            {
                (target as CinemachinePathPositon).BuildChildren();
            }
            if (!(target as CinemachinePathPositon).UpdateAlways && GUILayout.Button(ButtonName[2]))
            {
                (target as CinemachinePathPositon).UpdateChildren();
            }
        }
    }
}
