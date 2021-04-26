using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Editor;
using Cinemachine.Utility;
using RyuGiKen;
namespace RyuGiKen.DriftCar
{
    [AddComponentMenu("RyuGiKen/车辆轨迹曲线")]
    [SaveDuringPlay]
    [DisallowMultipleComponent]
    public class CinemachineSmoothPathForCarDrift : CinemachinePathBase
    {
        [Tooltip("循环")] public bool m_Looped;
        /// <summary>
        /// 路径点
        /// </summary>
        [Serializable]
        public class Waypoint
        {
            [Tooltip("位置")] public Vector3 position;
            [Tooltip("翻滚")] public float roll;
            [Tooltip("宽度")] public float width;
            [Tooltip("偏航")] public float yaw;
            [Tooltip("设定速度（<0时为不限）")] public float speed;
            /// <summary>Representation as Vector4</summary>
            internal Vector4 AsVector4
            {
                get { return new Vector4(position.x, position.y, position.z, roll); }
            }
            public Waypoint()
            {
                position = Vector3.zero;
                roll = yaw = 0;
                speed = -1;
                width = 1;
            }
            internal static Waypoint FromVector4(Vector4 v)
            {
                Waypoint wp = new Waypoint();
                wp.position = new Vector3(v[0], v[1], v[2]);
                wp.roll = v[3];
                return wp;
            }
        }
        [Tooltip("路径点")] public Waypoint[] m_Waypoints = new Waypoint[0];
        /// <summary>
        /// 初点
        /// </summary>
        public override float MinPos { get { return 0; } }
        /// <summary>
        /// 末点
        /// </summary>
        public override float MaxPos
        {
            get
            {
                int count = m_Waypoints.Length - 1;
                if (count < 1)
                    return 0;
                return m_Looped ? count + 1 : count;
            }
        }
        /// <summary>
        /// 循环
        /// </summary>
        public override bool Looped { get { return m_Looped; } }
        /// <summary>
        /// 采样间隔
        /// </summary>
        public override int DistanceCacheSampleStepsPerSegment { get { return m_Resolution; } }

        private void OnValidate() { InvalidateDistanceCache(); }

        private void Reset()
        {
            m_Looped = false;
            m_Waypoints = new Waypoint[2]
            {
                new Waypoint { position = new Vector3(0, 0, -5) },
                new Waypoint { position = new Vector3(0, 0, 5) }
            };
            m_Appearance = new Appearance();
            InvalidateDistanceCache();
        }
        /// <summary>
        /// 如果路径改变的方式影响了距离或其他缓存的路径元素，则调用此函数
        /// </summary>
        public override void InvalidateDistanceCache()
        {
            base.InvalidateDistanceCache();
            m_ControlPoints1 = null;
            m_ControlPoints2 = null;
        }
        Waypoint[] m_ControlPoints1;
        Waypoint[] m_ControlPoints2;
        bool m_IsLoopedCache;
        /// <summary>
        /// 更新路径点
        /// </summary>
        void UpdateControlPoints()
        {
            int numPoints = (m_Waypoints == null) ? 0 : m_Waypoints.Length;
            if (numPoints > 1
                && (Looped != m_IsLoopedCache
                    || m_ControlPoints1 == null || m_ControlPoints1.Length != numPoints
                    || m_ControlPoints2 == null || m_ControlPoints2.Length != numPoints))
            {
                Vector4[] p1 = new Vector4[numPoints];
                Vector4[] p2 = new Vector4[numPoints];
                Vector4[] K = new Vector4[numPoints];
                for (int i = 0; i < numPoints; ++i)
                    K[i] = m_Waypoints[i].AsVector4;
                if (Looped)
                    SplineHelpers.ComputeSmoothControlPointsLooped(ref K, ref p1, ref p2);
                else
                    SplineHelpers.ComputeSmoothControlPoints(ref K, ref p1, ref p2);

                m_ControlPoints1 = new Waypoint[numPoints];
                m_ControlPoints2 = new Waypoint[numPoints];
                for (int i = 0; i < numPoints; ++i)
                {
                    m_ControlPoints1[i] = Waypoint.FromVector4(p1[i]);
                    m_ControlPoints2[i] = Waypoint.FromVector4(p2[i]);
                }
                m_IsLoopedCache = Looped;
            }
        }
        /// <summary>
        /// 限位
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="indexA">前点</param>
        /// <param name="indexB">后点</param>
        /// <returns></returns>
        public float GetBoundingIndices(float pos, out int indexA, out int indexB)
        {
            pos = StandardizePos(pos);
            int numWaypoints = m_Waypoints.Length;
            if (numWaypoints < 2)
                indexA = indexB = 0;
            else
            {
                indexA = Mathf.FloorToInt(pos);
                if (indexA >= numWaypoints)
                {
                    // Only true if looped
                    pos -= MaxPos;
                    indexA = 0;
                }
                indexB = indexA + 1;
                if (indexB == numWaypoints)
                {
                    if (Looped)
                        indexB = 0;
                    else
                    {
                        --indexB;
                        --indexA;
                    }
                }
            }
            return pos;
        }
        /// <summary>
        /// 根据距离获取路径上的位置
        /// </summary>
        /// <param name="pos">距离</param>
        /// <returns>全局位置</returns>
        public override Vector3 EvaluatePosition(float pos)
        {
            Vector3 result = Vector3.zero;
            if (m_Waypoints.Length > 0)
            {
                UpdateControlPoints();
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (indexA == indexB)
                    result = m_Waypoints[indexA].position;
                else
                    result = SplineHelpers.Bezier3(pos - indexA,
                        m_Waypoints[indexA].position, m_ControlPoints1[indexA].position,
                        m_ControlPoints2[indexA].position, m_Waypoints[indexB].position);
            }
            return transform.TransformPoint(result);
        }
        /// <summary>
        /// 根据序号获取路径上的方向
        /// </summary>
        /// <param name="pos">序号</param>
        /// <returns>全局方向</returns>
        public override Vector3 EvaluateTangent(float pos)
        {
            Vector3 result = transform.rotation * Vector3.forward;
            if (m_Waypoints.Length > 1)
            {
                UpdateControlPoints();
                pos = GetBoundingIndices(pos, out int indexA, out int indexB);
                if (!Looped && indexA == m_Waypoints.Length - 1)
                    --indexA;
                result = SplineHelpers.BezierTangent3(pos - indexA,
                    m_Waypoints[indexA].position, m_ControlPoints1[indexA].position,
                    m_ControlPoints2[indexA].position, m_Waypoints[indexB].position);
            }
            return transform.TransformDirection(result);
        }
        /// <summary>
        /// 根据序号获取路径上的旋转
        /// </summary>
        /// <param name="pos">序号</param>
        /// <returns>旋转方向</returns>
        public override Quaternion EvaluateOrientation(float pos)
        {
            Quaternion transformRot = transform.rotation;
            Vector3 transformUp = transformRot * Vector3.up;
            Quaternion result = transformRot;
            if (m_Waypoints.Length > 0)
            {
                float roll = 0;
                pos = GetBoundingIndices(pos, out int indexA, out int indexB);
                if (indexA == indexB)
                {
                    roll = m_Waypoints[indexA].roll;
                }
                else
                {
                    UpdateControlPoints();
                    roll = SplineHelpers.Bezier1(pos - indexA,
                        m_Waypoints[indexA].roll, m_ControlPoints1[indexA].roll,
                        m_ControlPoints2[indexA].roll, m_Waypoints[indexB].roll);
                }

                Vector3 fwd = EvaluateTangent(pos);
                if (!fwd.AlmostZero())
                {
                    Quaternion q = Quaternion.LookRotation(fwd, transformUp);
                    result = q * RollAroundForward(roll);
                }
            }
            return result;
        }
        /// <summary>
        /// 根据序号获取路径上的旋转
        /// </summary>
        /// <param name="pos">序号</param>
        /// <param name="includeYaw">计算偏航</param>
        /// <returns>旋转方向</returns>
        public Quaternion EvaluateOrientation(float pos, bool includeYaw = false)
        {
            Quaternion transformRot = transform.rotation;
            Vector3 transformUp = transformRot * Vector3.up;
            Quaternion result = transformRot;
            if (m_Waypoints.Length > 0)
            {
                float roll = 0, yaw = 0;
                pos = GetBoundingIndices(pos, out int indexA, out int indexB);
                if (indexA == indexB)
                {
                    roll = m_Waypoints[indexA].roll;
                    if (includeYaw)
                        yaw = m_Waypoints[indexA].yaw;
                }
                else
                {
                    UpdateControlPoints();
                    roll = SplineHelpers.Bezier1(pos - indexA,
                        m_Waypoints[indexA].roll, m_ControlPoints1[indexA].roll,
                        m_ControlPoints2[indexA].roll, m_Waypoints[indexB].roll);
                    if (includeYaw)
                        yaw = SplineHelpers.Bezier1(pos - indexA,
                            m_Waypoints[indexA].yaw, m_ControlPoints1[indexA].yaw,
                            m_ControlPoints2[indexA].yaw, m_Waypoints[indexB].yaw);
                }

                Vector3 fwd = EvaluateTangent(pos);
                if (!fwd.AlmostZero())
                {
                    Quaternion q = Quaternion.LookRotation(fwd, transformUp);
                    if (includeYaw)
                        result = q * RollAroundForward(roll) * RollAroundUp(yaw);
                    else
                        result = q * RollAroundForward(roll);
                }
            }
            return result;
        }
        /// <summary>
        /// 根据序号获取路径上的设置速度
        /// </summary>
        /// <param name="pos">序号</param>
        /// <returns>速度</returns>
        public float EvaluateSpeed(float pos)
        {
            float result = 0;
            if (IsAllSpeedNull())
                return 0;
            else if (m_Waypoints.Length > 0)
            {
                UpdateControlPoints();
                pos = GetBoundingIndices(pos, out int indexA, out int indexB);
                if (indexA == indexB)
                    result = m_Waypoints[indexA].speed;
                else
                    result = Mathf.Lerp(m_Waypoints[indexA].speed, m_Waypoints[indexB].speed, ValueAdjust.ToPercent01(pos, indexA, indexB));
                //result = SplineHelpers.Bezier1(pos - indexA,
                //    m_Waypoints[indexA].speed, m_ControlPoints1[indexA].speed,
                //    m_ControlPoints2[indexA].speed, m_Waypoints[indexB].speed);
            }
            return result;
        }
        /// <summary>
        /// 是否所有速度点都没设
        /// </summary>
        /// <returns></returns>
        bool IsAllSpeedNull()
        {
            bool result = true;
            for (int i = 0; i < m_Waypoints.Length; i++)
            {
                if (m_Waypoints[i].speed > 0)
                    result = false;
            }
            return result;
        }
        /// <summary>
        /// 根据序号获取路径上的速度，忽略<0
        /// </summary>
        /// <param name="pos">序号</param>
        /// <param name="indexA">前点</param>
        /// <param name="indexB">后点</param>
        /// <returns>速度</returns>
        public float EvaluateSpeed(float pos, out int indexA, out int indexB)
        {
            float result = 0;
            pos = StandardizePos(pos);
            int numWaypoints = m_Waypoints.Length;
            if (numWaypoints < 2)
                indexA = indexB = 0;
            else if (IsAllSpeedNull())
            {
                indexA = indexB = 0;
                return 0;
            }
            else
            {
                indexA = Mathf.FloorToInt(pos);
                if (indexA >= numWaypoints)
                {
                    // Only true if looped
                    pos -= MaxPos;
                    indexA = 0;
                }
                while (m_Waypoints[indexA].speed < 0)
                {
                    indexA--;
                    if (indexA < 0)
                    {
                        if (Looped)
                            indexA = numWaypoints - 1;
                        else
                        {
                            indexA = 0;
                            break;
                        }
                    }
                }
                indexB = indexA + 1;
                if (indexB == numWaypoints)
                {
                    if (Looped)
                        indexB = 0;
                    else
                    {
                        --indexB;
                        --indexA;
                    }
                }
                while (m_Waypoints[indexB].speed < 0)
                {
                    indexB++;
                    if (indexB >= numWaypoints)
                    {
                        if (Looped)
                            indexB -= numWaypoints;
                        else
                        {
                            indexB = numWaypoints - 1;
                            break;
                        }
                    }
                }
            }
            if (m_Waypoints[indexA].speed < 0 || m_Waypoints[indexB].speed < 0)
                result = 0;
            else
                result = Mathf.Lerp(m_Waypoints[indexA].speed, m_Waypoints[indexB].speed, indexA > indexB ? ValueAdjust.ToPercent01(pos, indexA, indexB + numWaypoints) : ValueAdjust.ToPercent01(pos, indexA, indexB));
            return result;
        }
        /// <summary>
        /// 根据序号获取路径上的宽度
        /// </summary>
        /// <param name="pos">序号</param>
        /// <returns>速度</returns>
        public float EvaluateWidth(float pos)
        {
            float result = 0;
            if (m_Waypoints.Length > 0)
            {
                UpdateControlPoints();
                pos = GetBoundingIndices(pos, out int indexA, out int indexB);
                if (indexA == indexB)
                    result = m_Waypoints[indexA].width;
                else
                    result = Mathf.Lerp(m_Waypoints[indexA].width, m_Waypoints[indexB].width, ValueAdjust.ToPercent01(pos, indexA, indexB));
                //result = SplineHelpers.Bezier1(pos - indexA,
                //    m_Waypoints[indexA].width, m_ControlPoints1[indexA].width,
                //    m_ControlPoints2[indexA].width, m_Waypoints[indexB].width);

            }
            return result;
        }
        /// <summary>
        /// 与Quaternion.AngleAxis(angle, Vector3.forward)的调用结果一致
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Quaternion RollAroundForward(float angle)
        {
            float halfAngle = angle * 0.5F * Mathf.Deg2Rad;
            return new Quaternion(
                0,
                0,
                Mathf.Sin(halfAngle),
                Mathf.Cos(halfAngle));
        }
        /// <summary>
        /// 与Quaternion.AngleAxis(angle, Vector3.up)的调用结果一致
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Quaternion RollAroundUp(float angle)
        {
            return Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
}