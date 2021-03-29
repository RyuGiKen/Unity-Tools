using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.MSGame
{
    /// <summary>
    /// 子弹
    /// </summary>
    public class Bullet : Object
    {
        [Tooltip("命中伤害")] public int Damage = 1;
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
    }
}
