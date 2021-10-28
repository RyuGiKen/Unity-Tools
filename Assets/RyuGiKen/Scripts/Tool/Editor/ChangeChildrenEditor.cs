using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RyuGiKen.Tools
{
    [CustomEditor(typeof(ChangeChildren))]
    public class ChangeChildrenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[6];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "子对象批量重命名";
                    ButtonName[1] = "子对象倒转排序";
                    ButtonName[2] = "子对象按名称排序";
                    ButtonName[3] = "批量移除子对象";
                    ButtonName[4] = "批量移除隐藏子对象";
                    ButtonName[5] = "批量移除隐藏子孙对象";
                    break;
                default:
                    ButtonName[0] = "Rename Children";
                    ButtonName[1] = "Inverted Order";
                    ButtonName[2] = "Sort Children By Name";
                    ButtonName[3] = "Destroy Children";
                    ButtonName[4] = "Destroy Inactive Children";
                    ButtonName[5] = "Destroy Inactive Descendants";
                    break;
            }
            if (GUILayout.Button(ButtonName[0]))
            {
                (target as ChangeChildren).Rename();
            }
            if (GUILayout.Button(ButtonName[1]))
            {
                (target as ChangeChildren).InvertedOrder();
            }
            if (GUILayout.Button(ButtonName[2]))
            {
                (target as ChangeChildren).SortByName();
            }
            if (GUILayout.Button(ButtonName[3]))
            {
                (target as ChangeChildren).DestroyChildren();
            }
            if (GUILayout.Button(ButtonName[4]))
            {
                (target as ChangeChildren).DestroyInactiveChildren();
            }
            if (GUILayout.Button(ButtonName[5]))
            {
                (target as ChangeChildren).DestroyInactiveDescendants();
            }
        }
    }
}