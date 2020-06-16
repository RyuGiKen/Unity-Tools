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
        [Tooltip("目标跟随对象")] public Transform target;
        [Tooltip("跟随移动对象")] public Transform[] follower;
        [Tooltip("全局位置坐标差")] public Vector3[] startPosDifferenceValue;
        [Tooltip("跟随旋转")] public bool FollowRotation;
        [Tooltip("旋转差")] [SerializeField] Quaternion[] followerRotation;
        void Start()
        {
            if (target != null && follower[0] != null)
            {
                followerRotation = new Quaternion[follower.Length];
                startPosDifferenceValue = new Vector3[follower.Length];
                for (int i = 0; i < follower.Length; i++)
                {
                    startPosDifferenceValue[i] = follower[i].position - target.position;
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
                    follower[i].position = target.position + startPosDifferenceValue[i];
                    if (FollowRotation)
                    {
                        follower[i].rotation = target.rotation * followerRotation[i];
                    }
                }
            }
        }
    }
}