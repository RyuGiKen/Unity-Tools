﻿using UnityEngine;
using UnityEngine.Serialization;
using RyuGiKen;
using RyuGiKen.Tools;
using Cinemachine;
namespace RyuGiKen.DriftCar
{
    /// <summary>
    /// 按曲线移动
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class CinemachineCarDrift : MonoBehaviour
    {
        [Tooltip("路径")] public CinemachineSmoothPathForCarDrift m_Path;
        [Tooltip("更新间隔")] public CinemachineDollyCart.UpdateMethod m_UpdateMethod = CinemachineDollyCart.UpdateMethod.Update;
        [Tooltip("移动速度(m/s)")] public float m_Speed_MPS;
        [Tooltip("移动速度(km/h)")] public float m_Speed_KPH;
        public float m_Distance;
        public float m_PathUnits;
        public float m_CurrentRoadWidth;
        public float m_CurrentYaw;
        [Tooltip("轮子碰撞体")] [SerializeField] WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [Tooltip("轮子物体")] [SerializeField] GameObject[] m_WheelMeshes = new GameObject[4];
        void FixedUpdate()
        {
            switch (m_UpdateMethod)
            {
                case CinemachineDollyCart.UpdateMethod.FixedUpdate:
                    if (!Application.isPlaying)
                        SetCartPosition(m_Distance);
                    else
                        //SetCartPosition(m_Position + m_Speed * Time.fixedDeltaTime);
                        SetCartPosition(this.transform.position, m_WheelMeshes[0].transform.forward, m_Speed_KPH, Time.fixedDeltaTime);
                    break;
            }
        }
        void Update()
        {
            switch (m_UpdateMethod)
            {
                case CinemachineDollyCart.UpdateMethod.Update:
                case CinemachineDollyCart.UpdateMethod.LateUpdate:
                    if (!Application.isPlaying)
                        SetCartPosition(m_Distance);
                    else
                        //SetCartPosition(m_Position + m_Speed * Time.deltaTime);
                        SetCartPosition(this.transform.position, m_WheelMeshes[0].transform.forward, m_Speed_KPH, Time.deltaTime);
                    break;
            }
        }
        void LateUpdate()
        {
            switch (m_UpdateMethod)
            {
                case CinemachineDollyCart.UpdateMethod.Update:
                case CinemachineDollyCart.UpdateMethod.LateUpdate:
                    if (!Application.isPlaying)
                        SetCartPosition(m_Distance);
                    else
                        //SetCartPosition(m_Position + m_Speed * Time.deltaTime);
                        SetCartPosition(this.transform.position, m_WheelMeshes[0].transform.forward, m_Speed_KPH, Time.deltaTime);
                    break;
            }
        }
        void SetCartPosition(float distanceAlongPath)
        {
            if (m_Path != null)
            {
                m_Distance = m_Path.StandardizeUnit(distanceAlongPath, CinemachinePathBase.PositionUnits.Distance);
                this.transform.position = m_Path.EvaluatePositionAtUnit(m_Distance, CinemachinePathBase.PositionUnits.Distance);
                //this.transform.rotation = m_Path.EvaluateOrientationAtUnit(m_Distance, CinemachinePathBase.PositionUnits.Distance);
                m_PathUnits = m_Path.FindClosestPoint(this.transform.position, 0, -1, 2);
                this.transform.rotation = m_Path.EvaluateOrientation(m_PathUnits, true);
                m_Speed_KPH = m_Path.EvaluateSpeed(m_PathUnits, out int indexA, out int indexB);
                m_Speed_MPS = ValueAdjust.ConvertSpeed(SpeedType.KPH, SpeedType.MPS, m_Speed_KPH);
                m_CurrentRoadWidth = m_Path.EvaluateWidth(m_PathUnits);
                m_CurrentYaw = m_Path.EvaluateYaw(m_PathUnits);
            }
        }
        void SetCartPosition(Vector3 nowPosition, Vector3 direction, float speed, float step)
        {
            if (m_Path != null)
            {
                m_PathUnits = m_Path.FindClosestPoint(nowPosition + direction.normalized * speed * step, 0, -1, 2);
                m_Speed_KPH = m_Path.EvaluateSpeed(m_PathUnits, out int indexA, out int indexB);
                m_Speed_MPS = ValueAdjust.ConvertSpeed(SpeedType.KPH, SpeedType.MPS, m_Speed_KPH);
                Vector3 pos = m_Path.EvaluatePosition(m_PathUnits);
                m_CurrentRoadWidth = m_Path.EvaluateWidth(m_PathUnits);
                Vector3 right = ForwardToRight(m_Path.EvaluateTangent(m_PathUnits));
                if (!ValueAdjust.InLine(this.transform.position, pos + right * m_CurrentRoadWidth, pos - right * m_CurrentRoadWidth, step))
                    this.transform.position = pos;
                this.transform.rotation = m_Path.EvaluateOrientation(m_PathUnits, true);// * Quaternion.Euler(Vector3.up * Random.Range(-1f, 1f));
                m_CurrentYaw = m_Path.EvaluateYaw(m_PathUnits);
                Vector3 nextPos = m_Path.EvaluatePosition(m_Path.FindClosestPoint(this.transform.position + direction.normalized * speed * step, 0, 1, 2));
                UpdateWheels(this.transform.DirectionToLocalEulerAngles(nextPos - this.transform.position).y);
            }
        }
        void UpdateWheels(float SteerAngle)
        {
            m_WheelColliders[0].steerAngle = m_WheelColliders[1].steerAngle = SteerAngle;
            for (int i = 0; i < 4; i++)
            {
                if (Application.isPlaying)
                {
                    m_WheelColliders[i].GetWorldPose(out Vector3 position, out Quaternion quat);
                    m_WheelMeshes[i].transform.position = position;
                    m_WheelMeshes[i].transform.rotation = quat;
                }
                else
                {
                    if (i < 2)
                        m_WheelMeshes[i].transform.localEulerAngles = Vector3.up * SteerAngle;
                }
            }
        }
        Vector3 ForwardToRight(Vector3 direction)
        {
            Vector3 result = Vector3.zero;
            result = new Vector3(direction.z, direction.y, -direction.x);
            return result.normalized;
        }
    }
}