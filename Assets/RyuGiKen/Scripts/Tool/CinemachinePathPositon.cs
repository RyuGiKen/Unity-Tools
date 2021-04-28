using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using RyuGiKen;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 按曲线摆放
    /// </summary>
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
[ExecuteInEditMode]
#endif
    [DisallowMultipleComponent]
    //[RequireComponent(typeof(CinemachineSmoothPath))]
    public class CinemachinePathPositon : MonoBehaviour
    {
        /// <summary>
        /// 放置模式
        /// </summary>
        public enum Mode
        {
            [Tooltip("数量")] Count,
            [Tooltip("间隔")] Step
        }
        /// <summary>
        /// 计数模式
        /// </summary>
        public enum CountMode
        {
            [Tooltip("总数")] Sum,
            [Tooltip("平均")] Average
        }
        //public CinemachineSmoothPath m_Path;
        public CinemachineSmoothPath[] Paths;
        [Tooltip("实时更新")] public bool UpdateAlways;
        [Tooltip("放置模式")] public Mode mode;
        [Tooltip("计数模式")] public CountMode countMode;//间隔模式固定为总数
        CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
        [Tooltip("放置预制体")] public Transform[] ChildPrefab;
        [Tooltip("间隔")] public float Step = 0;//平均数量模式无用
        [Tooltip("放置量")] public int Count = 0;
        [Tooltip("预制体循环序号")] int[] ChildIndex;
        [Tooltip("曲线总长度")] public float MaxDistance = 0;
        void Awake()
        {
            //if (m_Path == null) m_Path = GetComponent<CinemachineSmoothPath>();
        }
        void Start()
        {

        }

        void Update()
        {

        }
        void LateUpdate()
        {
            Clamp();
            UpdateMaxDistance();
            if (UpdateAlways)
                BuildChildren();
        }
        /// <summary>
        /// 限位
        /// </summary>
        void Clamp()
        {
            if (Count < 0)
                Count = 0;
            if (Step <= 0)
                Step = ValueAdjust.Clamp(Step, 1);
        }
        /// <summary>
        /// 移除子对象
        /// </summary>
        [ContextMenu("移除子对象")]
        void ClearChildren()
        {
            for (int i = this.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// 调整子对象数量
        /// </summary>
        [ContextMenu("调整子对象数量")]
        void BuildChildren()
        {
            if (ChildPrefab.Length < 1 || Paths == null || Paths.Length < 1 || Paths[0] == null)
                return;
            int MaxChildCount = 0;
            int CurChildIndex = 0;
            switch (mode)
            {
                case Mode.Count:
                    if (ChildIndex == null || ChildIndex.Length != Count)
                    {
                        ChildIndex = new int[Count];
                        for (int i = 0; i < Count; i++)
                        {
                            ChildIndex[i] = CurChildIndex;
                            CurChildIndex++;
                            if (CurChildIndex >= ChildPrefab.Length)
                                CurChildIndex = 0;
                        }
                    }
                    foreach (CinemachineSmoothPath path in Paths)
                    {
                        if (path == null)
                            continue;
                        AdjustChildCount();
                        MaxChildCount = path.Looped ? this.transform.childCount : (this.transform.childCount <= 1 ? 1 : this.transform.childCount - 1);
                        switch (countMode)
                        {
                            case CountMode.Average:
                                break;
                            case CountMode.Sum:
                                switch (m_PositionUnits)
                                {
                                    case CinemachinePathBase.PositionUnits.PathUnits:
                                        Step = (path.m_Waypoints.Length - 1) * 1f / MaxChildCount;
                                        break;
                                    default:
                                    case CinemachinePathBase.PositionUnits.Distance:
                                        Step = path.PathLength * 1f / MaxChildCount;
                                        break;
                                    case CinemachinePathBase.PositionUnits.Normalized:
                                        Step = 1f / MaxChildCount;
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case Mode.Step:
                    Count = 0;
                    Clamp();
                    m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
                    countMode = CountMode.Sum;
                    foreach (CinemachineSmoothPath path in Paths)
                    {
                        if (path == null)
                            continue;
                        for (float i = 0; i <= path.PathLength; i += Step)
                        {
                            if (i <= path.PathLength - (path.Looped ? Step : 0))
                            {
                                Count++;
                            }
                        }
                    }
                    if (ChildIndex == null || ChildIndex.Length != Count)
                    {
                        ChildIndex = new int[Count];
                        for (int i = 0; i < Count; i++)
                        {
                            ChildIndex[i] = CurChildIndex;
                            CurChildIndex++;
                            if (CurChildIndex >= ChildPrefab.Length)
                                CurChildIndex = 0;
                        }
                    }
                    AdjustChildCount();
                    break;
            }
            UpdateChildren();
        }
        void AdjustChildCount()
        {
            int AdjustCount = 0;
            switch (countMode)
            {
                case CountMode.Sum:
                    AdjustCount = Count - this.transform.childCount;
                    break;
                case CountMode.Average:
                    AdjustCount = Count * Paths.Length - this.transform.childCount;
                    break;
            }
            if (AdjustCount > 0)//子对象数量不足，需补足
            {
                for (int i = 0; i < AdjustCount; i++)
                {
                    try
                    {
                        Instantiate(ChildPrefab[ChildIndex[this.transform.childCount - 1]], this.transform);
                    }
                    catch { }
                }
            }
            else if (AdjustCount < 0)//子对象数量过量，需减少
            {

                int NewCount = this.transform.childCount - Math.Abs(AdjustCount);
                for (int i = this.transform.childCount - 1; i >= NewCount; i--)
                {
                    DestroyImmediate(this.transform.GetChild(i).gameObject);
                }
            }
            switch (countMode)
            {
                case CountMode.Sum:
                    Count = this.transform.childCount;
                    break;
                case CountMode.Average:
                    Count = this.transform.childCount / Paths.Length;
                    break;
            }
        }
        /// <summary>
        /// 刷新曲线总长度
        /// </summary>
        void UpdateMaxDistance()
        {
            float distance = 0;
            foreach (CinemachineSmoothPath path in Paths)
            {
                distance += path.PathLength;
            }
            MaxDistance = distance;
        }
        /// <summary>
        /// 刷新子对象位置
        /// </summary>
        [ContextMenu("刷新子对象位置")]
        void UpdateChildren()
        {
            float distanceAlongPath = 0;
            int Index = 0;
            switch (mode)
            {
                case Mode.Count:
                    int[] MaxChildCount = new int[Paths.Length];
                    float MaxStep = MaxDistance / this.transform.childCount;
                    float MinStep = MaxDistance / (this.transform.childCount + Paths.Length);
                    float AverageStep = Step = (MinStep + MaxStep) * 0.5f;
                    for (int i = 0; i < Paths.Length; i++)
                    {
                        switch (countMode)
                        {
                            case CountMode.Average:
                                MaxChildCount[i] = Paths[i].Looped ? Count : (Count <= 1 ? 1 : Count - 1);
                                break;
                            case CountMode.Sum:
                                int AverageCount = (int)(Paths[i].PathLength / AverageStep);
                                MaxChildCount[i] = Paths[i].Looped ? AverageCount : (AverageCount <= 1 ? 1 : AverageCount - 1);
                                break;
                        }
                    }
                    Index = 0;
                    for (int i = 0; i < Paths.Length; i++)
                    {
                        for (int k = 0; k <= MaxChildCount[i]; k++)
                        {
                            if (Index >= this.transform.childCount)
                                break;
                            Transform trans = this.transform.GetChild(Index);
                            Index++;
                            distanceAlongPath = 0;
                            switch (m_PositionUnits)
                            {
                                case CinemachinePathBase.PositionUnits.PathUnits:
                                    distanceAlongPath = (Paths[i].m_Waypoints.Length - 1) * k * 1f / MaxChildCount[i];
                                    break;
                                case CinemachinePathBase.PositionUnits.Distance:
                                    distanceAlongPath = Paths[i].PathLength * k * 1f / MaxChildCount[i];
                                    break;
                                case CinemachinePathBase.PositionUnits.Normalized:
                                    distanceAlongPath = k * 1f / MaxChildCount[i];
                                    break;
                            }
                            SetCartPosition(trans, Paths[i], distanceAlongPath);
                        }
                    }
                    break;
                case Mode.Step:
                    m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;
                    Index = -1;
                    foreach (CinemachineSmoothPath path in Paths)
                    {
                        if (path == null)
                            continue;
                        for (float i = 0; i <= path.PathLength; i += Step)
                        {
                            distanceAlongPath = i;
                            if (i <= path.PathLength - (path.Looped ? Step : 0))
                            {
                                Index++;
                                if (Index >= this.transform.childCount)
                                    break;
                                Transform trans = this.transform.GetChild(Index);
                                SetCartPosition(trans, path, distanceAlongPath);
                            }
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 设置位于曲线的位置
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="distanceAlongPath"></param>
        void SetCartPosition(Transform trans, CinemachineSmoothPath path, float distanceAlongPath)
        {
            if (path != null)
            {
                float m_Position;
                m_Position = path.StandardizeUnit(distanceAlongPath, m_PositionUnits);
                trans.position = path.EvaluatePositionAtUnit(m_Position, m_PositionUnits);
                trans.rotation = path.EvaluateOrientationAtUnit(m_Position, m_PositionUnits);
            }
        }
    }
}
