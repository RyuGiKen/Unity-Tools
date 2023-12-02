using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
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
            foreach (Transform item in GetTransformsByName(gameObject))
            {
                item.SetAsLastSibling();
            }
        }
        /// <summary>
        /// 返回 children transforms, 根据名称排列
        /// </summary>
        public static Transform[] GetTransformsByName(GameObject parentGameObject)
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
        /// 子对象按坐标排序
        /// </summary>
        [ContextMenu("子对象按X坐标排序")]
        public void SortByPositionX()
        {
            foreach (Transform item in GetTransformsByPosition(gameObject, Axis.X))
            {
                item.SetAsLastSibling();
            }
        }
        /// <summary>
        /// 子对象按坐标排序
        /// </summary>
        [ContextMenu("子对象按Y坐标排序")]
        public void SortByPositionY()
        {
            foreach (Transform item in GetTransformsByPosition(gameObject, Axis.Y))
            {
                item.SetAsLastSibling();
            }
        }
        /// <summary>
        /// 子对象按坐标排序
        /// </summary>
        [ContextMenu("子对象按Z坐标排序")]
        public void SortByPositionZ()
        {
            foreach (Transform item in GetTransformsByPosition(gameObject, Axis.Z))
            {
                item.SetAsLastSibling();
            }
        }
        /// <summary>
        /// 根据坐标排列
        /// </summary>
        public static Transform[] GetTransformsByPosition(GameObject parentGameObject, Axis axis)
        {
            if (parentGameObject != null)
            {
                List<Component> components = new List<Component>(parentGameObject.GetComponentsInChildren(typeof(Transform)));
                List<Transform> transforms = components.ConvertAll(c => (Transform)c);

                transforms.Remove(parentGameObject.transform);
                transforms.Sort(delegate (Transform a, Transform b)
                {
                    switch (axis)
                    {
                        default:
                        case Axis.X:
                            return a.position.x.CompareTo(b.position.x);
                            break;
                        case Axis.Y:
                            return a.position.y.CompareTo(b.position.y);
                            break;
                        case Axis.Z:
                            return a.position.z.CompareTo(b.position.z);
                            break;
                    }
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
    [CustomEditor(typeof(ChangeChildren), true)]
    public class ChangeChildrenEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string[] ButtonName = new string[9];
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    ButtonName[0] = "子对象批量重命名";
                    ButtonName[1] = "子对象倒转排序";
                    ButtonName[2] = "子对象按名称排序";
                    ButtonName[3] = "子对象按X坐标排序";
                    ButtonName[4] = "子对象按Y坐标排序";
                    ButtonName[5] = "子对象按Z坐标排序";
                    ButtonName[6] = "批量移除子对象";
                    ButtonName[7] = "批量移除隐藏子对象";
                    ButtonName[8] = "批量移除隐藏子孙对象";
                    break;
                default:
                    ButtonName[0] = "Rename Children";
                    ButtonName[1] = "Inverted Order";
                    ButtonName[2] = "Sort Children By Name";
                    ButtonName[3] = "Sort Children By Pos X";
                    ButtonName[4] = "Sort Children By Pos Y";
                    ButtonName[5] = "Sort Children By Pos Z";
                    ButtonName[6] = "Destroy Children";
                    ButtonName[7] = "Destroy Inactive Children";
                    ButtonName[8] = "Destroy Inactive Descendants";
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
                (target as ChangeChildren).SortByPositionX();
            }
            if (GUILayout.Button(ButtonName[4]))
            {
                (target as ChangeChildren).SortByPositionY();
            }
            if (GUILayout.Button(ButtonName[5]))
            {
                (target as ChangeChildren).SortByPositionZ();
            }
            if (GUILayout.Button(ButtonName[6]))
            {
                (target as ChangeChildren).DestroyChildren();
            }
            if (GUILayout.Button(ButtonName[7]))
            {
                (target as ChangeChildren).DestroyInactiveChildren();
            }
            if (GUILayout.Button(ButtonName[8]))
            {
                (target as ChangeChildren).DestroyInactiveDescendants();
            }
        }
    }
#endif
}
