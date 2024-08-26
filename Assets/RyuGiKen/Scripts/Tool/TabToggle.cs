using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// ���ToggleGroupʹ�õ�ѡ��л�
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/ѡ��л�")]
    [RequireComponent(typeof(Toggle))]
    public class TabToggle : MonoBehaviour
    {
        [HideInInspector] public Toggle toggle;
        /// <summary>
        /// ����������Ч��
        /// </summary>
        public RectTransform[] panels;
        /// <summary>
        /// ���漤��
        /// </summary>
        public GameObject[] SetToggleCheckmarkObjectsActive;
        /// <summary>
        /// ��������
        /// </summary>
        public GameObject[] SetToggleCheckmarkObjectsInactive;
        protected void Awake()
        {
            toggle = this.GetComponent<Toggle>();
        }
        protected void Start()
        {
            Refresh(toggle.isOn);
        }
        public void Refresh(bool value)
        {
            panels?.SetActive(value);
            //if (SetToggleCheckmarkActive)
            //    toggle.graphic.SetActive(value);
            SetToggleCheckmarkObjectsActive.SetActive(value);
            SetToggleCheckmarkObjectsInactive.SetActive(!value);
        }
    }
}
