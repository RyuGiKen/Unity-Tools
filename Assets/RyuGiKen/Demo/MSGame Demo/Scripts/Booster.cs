using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.MSGame
{
    /// <summary>
    /// 喷口
    /// </summary>
    public class Booster : Object
    {
        public Controller m_Controller;
        Rigidbody m_Rigidbody { get { return m_Controller ? m_Controller.m_Rigidbody : null; } }
        [Tooltip("可动幅度")] public Vector2Int MovableRange = Vector2Int.zero;
        [Tooltip("额定推力")] public float RatedPower;
        [Tooltip("输出推力")] public float OutputForce;
        [Tooltip("推力方向")] Vector3 Direction;
        [Tooltip("方向转局部旋转角")] [SerializeField] Vector3 LocalEulerAngles;
        Transform BoosterModel;
        internal override void Awake()
        {
            base.Awake();
            if (this.transform.childCount > 0)
                BoosterModel = this.transform.GetChild(0);
        }
        internal override void Reset()
        {
            base.Reset();
            m_Controller = this.GetComponentInParent<Controller>();
            Direction = this.transform.forward;
        }
        internal override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        internal override void Update()
        {
            base.Update();
        }
        internal override void LateUpdate()
        {
            base.LateUpdate();
        }
        internal override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
        }
        internal override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="direction">全局方向</param>
        /// <param name="throttle">油门</param>
        public void Move(Vector3 direction, float throttle)
        {
            if (this.Broken)
                return;
            Direction = direction.normalized;
            throttle = Mathf.Clamp01(throttle);
            Vector3 LocalDirection = this.transform.InverseTransformDirection(Direction * -1);//局部方向
            LocalEulerAngles = Vector3.zero;
            if (LocalDirection.z > 0)
            {
                LocalEulerAngles.x = Mathf.Atan2(LocalDirection.y, LocalDirection.z) / Mathf.PI * 180;
                LocalEulerAngles.y = Mathf.Atan2(LocalDirection.x, LocalDirection.z) / Mathf.PI * 180;

                if ((int)Math.Abs(LocalEulerAngles.x) <= MovableRange.x && (int)Math.Abs(LocalEulerAngles.y) <= MovableRange.y)
                {
                    if (BoosterModel)
                        BoosterModel.localRotation = Quaternion.Euler(LocalEulerAngles);
                }
                else
                {
                    if (BoosterModel)
                        BoosterModel.localRotation = Quaternion.Lerp(BoosterModel.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime);
                    throttle = 0;
                }
            }
            else
            {
                if (BoosterModel)
                    BoosterModel.localRotation = Quaternion.Lerp(BoosterModel.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime);
                throttle = 0;
            }
            OutputForce = throttle * RatedPower;
            if (m_Rigidbody)
            {
                m_Rigidbody.AddForce(Direction * OutputForce * Time.fixedDeltaTime);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(this.transform.position, this.transform.position + Direction * -0.01f * OutputForce);
        }
    }
}
