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
    /// 子对象批量修改
    /// </summary>
    [AddComponentMenu("RyuGiKen/子对象批量修改")]
    public class ChangeChildren : MonoBehaviour
    {
        [Tooltip("重命名前缀")] public string NamePrefix;
        /// <summary>
        /// 子对象批量重命名
        /// </summary>
        [ContextMenu("子对象批量重命名")]
        public void Rename()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                this.transform.GetChild(i).name = NamePrefix + i.ToString("D" + this.transform.childCount.ToString().Length);
            }
        }
        /// <summary>
        /// 子对象倒转排序
        /// </summary>
        [ContextMenu("子对象倒转排序")]
        public void InvertedOrder()
        {
            for (int i = 0; i < this.transform.childCount - 1; i++)
            {
                this.transform.GetChild(this.transform.childCount - 1).SetSiblingIndex(i);
            }
        }
        /// <summary>
        /// 子对象按名称排序
        /// </summary>
        [ContextMenu("子对象按名称排序")]
        public void SortByName()
        {
            foreach (Transform item in GetTransforms(gameObject))
            {
                item.SetAsLastSibling();
            }
        }
        /// <summary>
        /// 返回 children transforms, 根据名称排列
        /// </summary>
        public static Transform[] GetTransforms(GameObject parentGameObject)
        {
            if (parentGameObject != null)
            {
                List<Component> components = new List<Component>(parentGameObject.GetComponentsInChildren(typeof(Transform)));
                List<Transform> transforms = components.ConvertAll(c => (Transform)c);

                transforms.Remove(parentGameObject.transform);
                transforms.Sort(delegate (Transform a, Transform b)
                {
                    return a.name.CompareTo(b.name);
                });
                return transforms.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 批量移除子对象
        /// </summary>
        [ContextMenu("批量移除子对象")]
        public void DestroyChildren()
        {
            GameObject[] Objects = ObjectAdjust.GetChildren(this.gameObject);
            for (int i = Objects.Length - 1; i >= 0; i--)
            {
                if (Objects[i])
                {
                    if (Application.isPlaying)
                        Object.Destroy(Objects[i]);
                    else
                        Object.DestroyImmediate(Objects[i]);
                }
            }
        }
        /// <summary>
        /// 批量移除隐藏子对象
        /// </summary>
        [ContextMenu("批量移除隐藏子对象")]
        public void DestroyInactiveChildren()
        {
            GameObject[] Objects = ObjectAdjust.GetChildren(this.gameObject);
            for (int i = Objects.Length - 1; i >= 0; i--)
            {
                if (Objects[i] && !Objects[i].activeInHierarchy)
                {
                    if (Application.isPlaying)
                        Object.Destroy(Objects[i]);
                    else
                        Object.DestroyImmediate(Objects[i]);
                }
            }
        }
        /// <summary>
        /// 批量移除隐藏子孙对象
        /// </summary>
        [ContextMenu("批量移除隐藏子孙对象")]
        public void DestroyInactiveDescendants()
        {
            GameObject[] Objects = ObjectAdjust.GetDescendants(this.gameObject);
            for (int i = Objects.Length - 1; i >= 0; i--)
            {
                if (Objects[i] && !Objects[i].activeInHierarchy)
                {
                    if (Application.isPlaying)
                        Object.Destroy(Objects[i]);
                    else
                        Object.DestroyImmediate(Objects[i]);
                }
            }
        }
    }
}
namespace RyuGiKenEditor.Tools
{
#if UNITY_EDITOR
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
#endif
}
