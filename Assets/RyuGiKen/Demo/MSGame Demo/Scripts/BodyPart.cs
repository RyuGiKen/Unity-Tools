using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.MSGame
{
    /// <summary>
    /// 身体部分
    /// </summary>
    public class BodyPart : Object
    {
        /// <summary>
        /// 部位类型
        /// </summary>
        public enum PartType
        {
            /// <summary>
            /// 其他
            /// </summary>
            Other,
            /// <summary>
            /// 头
            /// </summary>
            Head,
            /// <summary>
            /// 躯干
            /// </summary>
            Truncus,
            /// <summary>
            /// 下体
            /// </summary>
            Pelvic,
            /// <summary>
            /// 背包
            /// </summary>
            Backpack,
            /// <summary>
            /// 左肩甲
            /// </summary> 
            LeftShoulderArmor,
            /// <summary>
            /// 右肩甲
            /// </summary>
            RightShoulderArmor,
            /// <summary>
            /// 左臂
            /// </summary>
            LeftArm,
            /// <summary>
            /// 右臂
            /// </summary>
            RightArm,
            /// <summary>
            /// 左手
            /// </summary>
            LeftHand,
            /// <summary>
            /// 右手
            /// </summary>
            RightHand,
            /// <summary>
            /// 左大腿
            /// </summary>
            LeftThigh,
            /// <summary>
            /// 右大腿
            /// </summary>
            RightThigh,
            /// <summary>
            /// 左小腿
            /// </summary>
            LeftCalf,
            /// <summary>
            /// 右小腿
            /// </summary>
            RightCalf,
            /// <summary>
            /// 左脚
            /// </summary>
            LeftFoot,
            /// <summary>
            /// 右脚
            /// </summary>
            RightFoot,
        }
        [Tooltip("部位")] public PartType Type;
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
