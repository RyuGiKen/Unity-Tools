using System;
using UnityEngine;
using RyuGiKen;
using UnityStandardAssets.Vehicles.Car;
#pragma warning disable 649
namespace RyuGiKen.DriftCar
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UnityStandardAssets.Vehicles.Car.CarController))]
    public class CarController : MonoBehaviour
    {
        public UnityStandardAssets.Vehicles.Car.CarController carController;
        [Tooltip("速度单位类型")] public SpeedType m_SpeedType = SpeedType.KPH;
        public Rigidbody m_Rigidbody;
        public float CurrentSpeed { get { return GetSpeed(); } }
        [Tooltip("最高转速")] public static float TopRevs = 8000;
        [Tooltip("引擎转速(计算值)")] public float EngineRevs;//引擎转速=车轮转速*速比
        [Tooltip("车轮转速")] public float WheelRevs;
        [Tooltip("转速(显示值)")] public float Revs;
        [Tooltip("转速[0,1]")] [Range(0, 1)] public float RevsPercent;
        [Tooltip("油门[0,1]")] [Range(0, 1)] public float AccelInput;
        [Tooltip("刹车[0,1]")] [Range(0, 1)] public float BrakeInput;
        [Tooltip("转向角")] [Range(-60, 60)] public float m_SteerAngle;
        [Tooltip("速度方向")] public Vector3 Speed;
        [Tooltip("局部速度方向")] public Vector3 LocalSpeed;
        Vector3 LastPosition;
        float LastSpeed;
        float angle;
        private void Awake()
        {
            if (Application.isPlaying)
                Reset();
        }
        private void Start()
        {

        }
        private void Reset()
        {
            m_Rigidbody = this.GetComponent<Rigidbody>();
            carController = this.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>();
        }
        /// <summary>
        /// 计算转速
        /// </summary>
        private void CalculateRevs()
        {
            float CurrentFactor = 1;
            float wheelRevs;

            //wheelRevs = (carController.m_WheelColliders[0].rpm + carController.m_WheelColliders[1].rpm + carController.m_WheelColliders[2].rpm + carController.m_WheelColliders[3].rpm) / 4;
            wheelRevs = Speed.magnitude * 60f / carController.m_WheelColliders[0].radius;
            if (wheelRevs > WheelRevs && RevsPercent > 0.8f)
            {
                WheelRevs = ValueAdjust.Lerp(WheelRevs, wheelRevs, Time.deltaTime, 200);
            }
            else if (wheelRevs > WheelRevs && RevsPercent > 0.6f)
            {
                WheelRevs = ValueAdjust.Lerp(WheelRevs, wheelRevs, Time.deltaTime, 800);
            }
            else
            {
                WheelRevs = wheelRevs;
            }
            //计算引擎转速（用于显示和声音）
            EngineRevs = WheelRevs * CurrentFactor;
            if (EngineRevs > Revs)
            {
                if (RevsPercent > 0.8f)
                {
                    Revs = ValueAdjust.Lerp(Revs, EngineRevs, Time.deltaTime, 200);
                }
                else if (RevsPercent > 0.6f)
                {
                    Revs = ValueAdjust.Lerp(Revs, EngineRevs, Time.deltaTime, 1000);
                }
                else
                {
                    //Revs = Mathf.Lerp(Revs, EngineRevs, Time.deltaTime * 4f);
                    Revs = ValueAdjust.Lerp(Revs, EngineRevs, Time.deltaTime, 5000);
                }
            }
            else
            {
                Revs = ValueAdjust.Lerp(Revs, 0, Time.deltaTime, 2000);
                //Revs = Mathf.Lerp(Revs, 0, Time.deltaTime * 0.5f);
            }
            Revs = ValueAdjust.Clamp(Revs, 0, TopRevs);
            RevsPercent = ValueAdjust.Clamp(Revs / TopRevs);
        }
        private void FixedUpdate()
        {
            carController.Move(m_SteerAngle / carController.m_MaximumSteerAngle, AccelInput, -BrakeInput, 0);
        }
        public float GetSpeed()
        {
            return ValueAdjust.ConvertSpeed(SpeedType.MPS, m_SpeedType, m_Rigidbody.velocity.magnitude);
        }
        public float GetSpeed(SpeedType type)
        {
            return ValueAdjust.ConvertSpeed(SpeedType.MPS, type, m_Rigidbody.velocity.magnitude);
        }
        public void LateUpdate()
        {
            //LocalSpeed = transform.InverseTransformDirection(m_Rigidbody.velocity);
            Speed = (this.transform.position - LastPosition) / Time.deltaTime;
            LocalSpeed = transform.InverseTransformDirection(Speed);
            carController.Skidding = Vector3.Angle(this.transform.forward, Speed) > 2;
            if (ValueAdjust.JudgeRange(Speed.magnitude, LastSpeed, 1f))
            {
                AccelInput = ValueAdjust.Lerp(AccelInput, 0, Time.deltaTime);
                BrakeInput = ValueAdjust.Lerp(BrakeInput, 0, Time.deltaTime);
            }
            else
            {
                if (Speed.magnitude > LastSpeed)
                {
                    AccelInput = ValueAdjust.Lerp(AccelInput, 1, Time.deltaTime);
                }
                else
                {
                    AccelInput = ValueAdjust.Lerp(AccelInput, 0, Time.deltaTime);
                }
                BrakeInput = ValueAdjust.Lerp(BrakeInput, 1 - AccelInput, Time.deltaTime);
            }
            CalculateRevs();
            carController.Revs = RevsPercent;
            LastPosition = this.transform.position;
            LastSpeed = Speed.magnitude;
        }
    }
}
