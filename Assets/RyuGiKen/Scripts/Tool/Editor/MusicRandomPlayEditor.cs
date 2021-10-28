using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(MusicRandomPlay))]
    public class MusicRandomPlayEditor : Editor
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
                    ButtonName[0] = "切换音乐并播放";
                    ButtonName[1] = "停止播放";
                    break;
                default:
                    ButtonName[0] = "Switch And Play";
                    ButtonName[1] = "Stop";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as MusicRandomPlay).SwitchAndPlay();
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                (target as MusicRandomPlay).StopPlay();
            }
        }
    }
}