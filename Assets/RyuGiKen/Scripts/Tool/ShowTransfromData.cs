using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 实时显示Transform属性
    /// </summary>
    [AddComponentMenu("RyuGiKen/实时显示Transform属性")]
    [RequireComponent(typeof(Transform))]
    public class ShowTransfromData : MonoBehaviour
    {
        [SerializeField] Transform _Transform;
        public Transform m_Transform
        {
            get
            {
                Reset();
                return _Transform;
            }
        }
        private void Awake()
        {
            Reset();
        }
        private void Reset()
        {
            if (!_Transform)
                _Transform = this.GetComponent<Transform>();
        }
        private void LateUpdate()
        {

        }
    }
}
