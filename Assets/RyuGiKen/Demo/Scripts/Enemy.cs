using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RyuGiKen;
namespace RyuGiKen.TrackArrow
{
    /// <summary>
    /// 敌方
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        public bool isDie { get { return HP <= 0; } }
        [Tooltip("生命值")] public float HP;
        public float HP_Max = 1;
        [Tooltip("自动跟踪瞄准点")] public Transform[] AimPos;
        void Awake()
        {
            HP = HP_Max;
        }
        private void Start()
        {
            if (AimPos.Length < 1)
                AimPos = new Transform[] { this.transform };
        }
        private void LateUpdate()
        {
            this.transform.localEulerAngles = new Vector3(ValueAdjust.Lerp(this.transform.localEulerAngles.x, isDie ? 90 : 0, Time.deltaTime, 90), this.transform.localEulerAngles.y, 0);
        }
        /// <summary>
        /// 被击中
        /// </summary>
        public void BeHit()
        {
            HP--;
            if (HP <= 0)
                StartCoroutine(Revive(5));
        }
        /// <summary>
        /// 复活
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        IEnumerator Revive(float time = 1f)
        {
            yield return new WaitForSeconds(time);
            HP = HP_Max;
        }
    }
}
