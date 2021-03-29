using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.MSGame
{
    /// <summary>
    /// MS
    /// </summary>
    public class MobileSuit : Controller
    {
        public MobileSuitBody Body;
        [System.Serializable]
        public struct MobileSuitBody
        {
            [Tooltip("头")] public BodyPart Head;
            [Tooltip("躯干")] public BodyPart Truncus;
            [Tooltip("下体")] public BodyPart Pelvic;
            [Tooltip("背包")] public BodyPart Backpack;

            [Tooltip("左肩甲")] public BodyPart LeftShoulderArmor;
            [Tooltip("右肩甲")] public BodyPart RightShoulderArmor;

            [Tooltip("左臂")] public BodyPart LeftArm;
            [Tooltip("右臂")] public BodyPart RightArm;

            [Tooltip("左手")] public BodyPart LeftHand;
            [Tooltip("右手")] public BodyPart RightHand;

            [Tooltip("左大腿")] public BodyPart LeftThigh;
            [Tooltip("右大腿")] public BodyPart RightThigh;

            [Tooltip("左小腿")] public BodyPart LeftCalf;
            [Tooltip("右小腿")] public BodyPart RightCalf;

            [Tooltip("左脚")] public BodyPart LeftFoot;
            [Tooltip("右脚")] public BodyPart RightFoot;
        }

        [Tooltip("武器")] public List<Weapon> Weapons;
        internal override void Awake()
        {
            base.Awake();
        }
        internal override void Reset()
        {
            base.Reset();
            Weapons = GetComponentsInChildren<Weapon>(true).ToList();
            ResetBodyPart();
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
        public override void Move(Vector3 direction)
        {
            base.Move(direction);
        }
        public void ResetBodyPart()
        {
            foreach (BodyPart part in Parts)
            {
                switch (part.Type)
                {
                    case BodyPart.PartType.Head:
                        if (!Body.Head)
                            Body.Head = part;
                        break;
                    case BodyPart.PartType.Truncus:
                        if (!Body.Truncus)
                            Body.Truncus = part;
                        break;
                    case BodyPart.PartType.Pelvic:
                        if (!Body.Pelvic)
                            Body.Pelvic = part;
                        break;
                    case BodyPart.PartType.Backpack:
                        if (!Body.Backpack)
                            Body.Backpack = part;
                        break;
                    case BodyPart.PartType.LeftShoulderArmor:
                        if (!Body.LeftShoulderArmor)
                            Body.LeftShoulderArmor = part;
                        break;
                    case BodyPart.PartType.RightShoulderArmor:
                        if (!Body.RightShoulderArmor)
                            Body.RightShoulderArmor = part;
                        break;
                    case BodyPart.PartType.LeftArm:
                        if (!Body.LeftArm)
                            Body.LeftArm = part;
                        break;
                    case BodyPart.PartType.RightArm:
                        if (!Body.RightArm)
                            Body.RightArm = part;
                        break;
                    case BodyPart.PartType.LeftHand:
                        if (!Body.LeftHand)
                            Body.LeftHand = part;
                        break;
                    case BodyPart.PartType.RightHand:
                        if (!Body.RightHand)
                            Body.RightHand = part;
                        break;
                    case BodyPart.PartType.LeftThigh:
                        if (!Body.LeftThigh)
                            Body.LeftThigh = part;
                        break;
                    case BodyPart.PartType.RightThigh:
                        if (!Body.RightThigh)
                            Body.RightThigh = part;
                        break;
                    case BodyPart.PartType.LeftCalf:
                        if (!Body.LeftCalf)
                            Body.LeftCalf = part;
                        break;
                    case BodyPart.PartType.RightCalf:
                        if (!Body.RightCalf)
                            Body.RightCalf = part;
                        break;
                    case BodyPart.PartType.LeftFoot:
                        if (!Body.LeftFoot)
                            Body.LeftFoot = part;
                        break;
                    case BodyPart.PartType.RightFoot:
                        if (!Body.RightFoot)
                            Body.RightFoot = part;
                        break;
                }
            }
        }
        /// <summary>
        /// 获取特定部位
        /// </summary>
        /// <param name="Type">类型</param>
        /// <returns></returns>
        public BodyPart GetPart(BodyPart.PartType Type)
        {
            foreach (BodyPart part in Parts)
            {
                if (part.Type == Type)
                {
                    return part;
                }
            }
            return null;
        }
    }
}
