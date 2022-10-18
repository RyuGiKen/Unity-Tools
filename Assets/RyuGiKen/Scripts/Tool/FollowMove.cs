using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 跟随移动
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/跟随移动")]
    public class FollowMove : MonoBehaviour
    {
        public enum FollowMode
        {
            /// <summary>
            /// 全局坐标
            /// </summary>
            WorldPositon,
            /// <summary>
            /// 局部坐标，跟随缩放
            /// </summary>
            LocalPosion,
            /// <summary>
            /// 局部坐标，忽略缩放
            /// </summary>
            LocalPosionIgnoreScale
        }
        [Tooltip("目标跟随对象")] public Transform target;
        [Tooltip("目标初始缩放")] public Vector3 targetScale;
        public bool X = true;
        public bool Y = true;
        public bool Z = true;
        [Tooltip("跟随模式")][SerializeField] FollowMode space = FollowMode.WorldPositon;
        [Tooltip("跟随移动对象")] public Transform[] follower;
        [Tooltip("全局位置坐标差")] public Vector3[] startPosDifferenceValue_World;
        [Tooltip("局部位置坐标差")] public Vector3[] startPosDifferenceValue_Self;
        [Tooltip("跟随旋转")] public bool FollowRotation;
        [Tooltip("旋转差")][SerializeField] Quaternion[] followerRotation;
        void Start()
        {
            if (target != null && follower[0] != null)
            {
                followerRotation = new Quaternion[follower.Length];
                targetScale = target.localScale;
                startPosDifferenceValue_World = new Vector3[follower.Length];
                startPosDifferenceValue_Self = new Vector3[follower.Length];
                for (int i = 0; i < follower.Length; i++)
                {
                    startPosDifferenceValue_World[i] = follower[i].position - target.position;
                    startPosDifferenceValue_Self[i] = target.InverseTransformPoint(follower[i].position);
                    followerRotation[i] = Quaternion.Slerp(follower[i].rotation, target.rotation, 0);
                }
            }
        }
        void Update()
        {
            if (target != null && follower[0] != null)
            {
                for (int i = 0; i < follower.Length; i++)
                {
                    switch (space)
                    {
                        default:
                        case FollowMode.WorldPositon:
                            Vector3 pos = target.position + startPosDifferenceValue_World[i];
                            if (!X)
                                pos.x = follower[i].position.x;
                            if (!Y)
                                pos.y = follower[i].position.y;
                            if (!Z)
                                pos.z = follower[i].position.z;
                            follower[i].position = pos;
                            break;
                        case FollowMode.LocalPosion:
                            follower[i].position = target.TransformPoint(startPosDifferenceValue_Self[i]);
                            break;
                        case FollowMode.LocalPosionIgnoreScale:
                            Vector3 LocalPositon = new Vector3(startPosDifferenceValue_Self[i].x / target.localScale.x * targetScale.x, startPosDifferenceValue_Self[i].y / target.localScale.y * targetScale.y, startPosDifferenceValue_Self[i].z / target.localScale.z * targetScale.z);
                            follower[i].position = target.TransformPoint(LocalPositon);
                            break;
                    }
                    if (FollowRotation)
                    {
                        follower[i].rotation = target.rotation * followerRotation[i];
                    }
                }
            }
        }
    }
}