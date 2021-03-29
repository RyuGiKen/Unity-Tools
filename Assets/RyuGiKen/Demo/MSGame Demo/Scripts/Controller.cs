using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.MSGame
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    public class Controller : Object
    {
        [Tooltip("喷口")] public List<Booster> Boosters;
        public Rigidbody m_Rigidbody;
        [Tooltip("油门")] [Range(0, 1)] public float Throttle;
        [Tooltip("推力")] public float BoosterForce;
        /// <summary>
        /// 部位
        /// </summary>
        public BodyPart[] Parts { get { return GetComponentsInChildren<BodyPart>(true); } }
        [Tooltip("推力方向")] public Vector3 MoveDirection;
        internal override void Awake()
        {
            base.Awake();
        }
        internal override void Reset()
        {
            base.Reset();
            m_Rigidbody = GetComponent<Rigidbody>();
            if (!m_Rigidbody)
                m_Rigidbody = this.gameObject.AddComponent<Rigidbody>();
            Boosters = GetComponentsInChildren<Booster>().ToList();
        }
        public override void ResetHP()
        {
            base.ResetHP();
            HP_Max = 0;
            foreach (BodyPart part in Parts)
            {
                part.ResetHP();
                HP_Max += part.HP_Max;
            }
            HP = HP_Max;
        }
        internal override void FixedUpdate()
        {
            base.FixedUpdate();
            int m_HP = 0;
            foreach (BodyPart part in Parts)
            {
                m_HP += part.HP;
            }
            HP = m_HP;
            float m_BoosterForce = 0;
            foreach (Booster booster in Boosters)
            {
                m_BoosterForce += booster.OutputForce;
            }
            BoosterForce = m_BoosterForce;
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
        public virtual void Move(Vector3 direction)
        {
            MoveDirection = direction;
            if (this.Broken)
                return;
            direction = direction.normalized;
            foreach (Booster booster in Boosters)
            {
                booster.Move(direction, Throttle);
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        public virtual void Stop()
        {
            if (m_Rigidbody.velocity.magnitude > 0.1f)
            {
                Throttle = m_Rigidbody.velocity.magnitude;
                Move(-m_Rigidbody.velocity);
            }
        }
    }
}
