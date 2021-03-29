using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.MSGame
{
    /// <summary>
    /// 场景对象
    /// </summary>
    [DisallowMultipleComponent]
    public class Object : MonoBehaviour
    {
        [Tooltip("可破坏")] public bool Destructible = true;
        [Tooltip("生命值")] public int HP = 0;
        [Tooltip("最大生命值")] public int HP_Max = 1;
        /// <summary>
        /// 被破坏
        /// </summary>
        public bool Broken { get { return (HP <= 0 || !this.gameObject.activeInHierarchy); } }
        public Renderer[] renderers;
        internal virtual void Awake()
        {
            Reset();
        }
        internal virtual void Reset()
        {
            ResetHP();
            renderers = this.GetComponentsInChildren<Renderer>(true);
        }
        /// <summary>
        /// 重置血量
        /// </summary>
        public virtual void ResetHP()
        {
            HP = HP_Max;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        [ContextMenu("初始化")]
        public void Initialize()
        {
            Reset();
        }
        internal virtual void FixedUpdate()
        {
            if (HP <= 0)
            {
                Destroy();
            }
        }
        internal virtual void Update()
        {

        }
        internal virtual void LateUpdate()
        {

        }
        internal virtual void OnCollisionEnter(Collision collision)
        {

        }
        internal virtual void OnTriggerEnter(Collider other)
        {

        }
        /// <summary>
        /// 被破坏
        /// </summary>
        public virtual void Destroy()
        {
            this.gameObject.SetActive(false);
        }
    }
}
