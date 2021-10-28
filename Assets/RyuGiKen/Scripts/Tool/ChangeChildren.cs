using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
