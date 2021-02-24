using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        void SetTarget()
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
        void SetAll()
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
