using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
#if UNITY_EDITOR
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
#endif
}
