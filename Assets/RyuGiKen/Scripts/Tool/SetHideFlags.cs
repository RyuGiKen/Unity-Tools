using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RyuGiKen.Tools;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 设置HideFlags
    /// 默认值None用于恢复Hierarchy窗口的隐藏对象。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/设置HideFlags")]
    public class SetHideFlags : MonoBehaviour
    {
        public HideFlags m_HideFlags;
        public GameObject TargetObject;
        /// <summary>
        /// 设置目标的HideFlags
        /// </summary>
        [ContextMenu("设置目标的HideFlags")]
        public void SetTarget()
        {
            if (TargetObject)
                TargetObject.hideFlags = m_HideFlags;
            else
                this.gameObject.hideFlags = m_HideFlags;
        }
        /// <summary>
        /// 设置所有对象的HideFlags
        /// </summary>
        [ContextMenu("设置所有对象的HideFlags")]
        public void SetAll()
        {
            GameObject[] Objects = Resources.FindObjectsOfTypeAll<GameObject>();
            Debug.Log("找到" + Objects.Length + "个对象");
            foreach (GameObject go in Objects)
            {
                go.hideFlags = m_HideFlags;
            }
        }
    }
}
namespace RyuGiKenEditor.Tools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SetHideFlags), true)]
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
#endif
}
