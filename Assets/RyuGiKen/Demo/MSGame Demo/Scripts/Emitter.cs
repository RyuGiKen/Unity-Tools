using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.MSGame
{
    /// <summary>
    /// 发射器
    /// </summary>
    public class Emitter : Weapon
    {
        [Tooltip("发射子弹预制体")] public Bullet bulletPrefab;
        [Tooltip("允许装填")] public bool CanReload = true;
        [Tooltip("连续装填")] public bool ContinualReload = true;

        [Tooltip("当前装弹量")] public int Capasity = 1;
        [Tooltip("最大装弹量")] public int Capasity_Max = 1;

        [Tooltip("当前装填时间")] public float ReloadTime = 0;
        [Tooltip("最大装填时间")] public float ReloadTime_Max = 1;
        /// <summary>
        /// 装填速度
        /// </summary>
        public float ReloadSpeed { get { return ReloadTime_Max; } }
        internal override void Awake()
        {
            base.Awake();
        }
        internal override void Reset()
        {
            base.Reset();
        }
        internal override void FixedUpdate()
        {
            base.FixedUpdate();
            if (CanReload && Time.deltaTime > 0)
            {
                ReloadTime -= Time.fixedUnscaledDeltaTime;
                if (ReloadTime < 0)
                {
                    ReloadFinish();
                    ReloadTime = ReloadTime_Max;
                }
            }
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
        /// 装填完成
        /// </summary>
        internal virtual void ReloadFinish()
        {
            if (ContinualReload)
            {
                Capasity = ValueAdjust.Clamp(Capasity + 1, 0, Capasity_Max);
            }
            else
            {
                Capasity = Capasity_Max;
            }
        }
        /// <summary>
        /// 重新装填
        /// </summary>
        public virtual void Reload()
        {
            ReloadTime = ReloadTime_Max;
        }
        /// <summary>
        /// 发射
        /// </summary>
        public virtual void Shot()
        {

        }
    }
}
