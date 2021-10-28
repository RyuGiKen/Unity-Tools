using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// Transfrom复制
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TransformSync : MonoBehaviour
    {
        public List<Transform> Object_A;
        public List<Transform> Object_B;

        [Tooltip("局部位置")] public bool localPosition;
        [Tooltip("全局位置")] public bool position;

        [Tooltip("局部旋转")] public bool localEulerAngles;
        [Tooltip("全局旋转")] public bool rotation;

        [Tooltip("局部缩放")] public bool localScale;
        [Tooltip("全局缩放")] public bool lossyScale;

        private void LateUpdate()
        {
            if (localPosition && position)
            {
                localPosition = false;
            }
            if (localEulerAngles && rotation)
            {
                localEulerAngles = false;
            }
            if (localScale && lossyScale)
            {
                localScale = false;
            }
        }
        [ContextMenu("获取A0的所有子孙")]
        public void GetDescendants_A0()
        {
            Transform A0 = Object_A[0];
            Transform[] descendants = Object_A[0].GetDescendants();
            Object_A.Clear();
            Object_A.Add(A0);
            Object_A.AddList(descendants.ToList());
            Object_A.ClearRepeatingItem();
        }
        [ContextMenu("获取A的所有子孙")]
        public void GetDescendants_A()
        {
            Transform A0 = Object_A[0];
            List<Transform> descendants = new List<Transform>();
            foreach (Transform obj in Object_A)
            {
                descendants.AddList(obj.GetDescendants().ToList());
            }
            Object_A.Clear();
            Object_A.Add(A0);
            Object_A.AddList(descendants);
            Object_A.ClearRepeatingItem();
        }
        [ContextMenu("获取B0的所有子孙")]
        public void GetDescendants_B0()
        {
            Transform B0 = Object_B[0];
            Transform[] descendants = Object_B[0].GetDescendants();
            Object_B.Clear();
            Object_B.Add(B0);
            Object_B.AddList(descendants.ToList());
            Object_B.ClearRepeatingItem();
        }
        [ContextMenu("获取B的所有子孙")]
        public void GetDescendants_B()
        {
            Transform B0 = Object_B[0];
            List<Transform> descendants = new List<Transform>();
            foreach (Transform obj in Object_B)
            {
                descendants.AddList(obj.GetDescendants().ToList());
            }
            Object_B.Clear();
            Object_B.Add(B0);
            Object_B.AddList(descendants);
            Object_B.ClearRepeatingItem();
        }
        [ContextMenu("移除重复项")]
        public void ClearRepeating()
        {
            Object_A.ClearRepeatingItem();
            Object_B.ClearRepeatingItem();
        }
        [ContextMenu("重复项置空")]
        public void SetRepeatingNull()
        {
            Object_A.SetRepeatingItemNull();
            Object_B.SetRepeatingItemNull();
        }
        [ContextMenu("A to B")]
        public void AtoB()
        {
            Sync(Object_A.ToArray(), Object_B.ToArray(), localPosition, position, localEulerAngles, rotation, localScale, lossyScale);
        }
        [ContextMenu("B to A")]
        public void BtoA()
        {
            Sync(Object_B.ToArray(), Object_A.ToArray(), localPosition, position, localEulerAngles, rotation, localScale, lossyScale);
        }
        public static void Sync(Transform[] Get, Transform[] Set, bool localPosition, bool position, bool localEulerAngles, bool rotation, bool localScale, bool lossyScale)
        {
            for (int i = 0; i < Mathf.Min(Get.Length, Set.Length); i++)
            {
                ObjectAdjust.Sync(Get[i], Set[i], localPosition, position, localEulerAngles, rotation, localScale, lossyScale);
            }
        }
    }
}
