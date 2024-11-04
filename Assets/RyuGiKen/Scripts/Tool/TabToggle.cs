using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 配合ToggleGroup使用的选项卡切换
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("RyuGiKen/选项卡切换")]
    [RequireComponent(typeof(Toggle))]
    public class TabToggle : MonoBehaviour
    {
        [HideInInspector] public Toggle toggle;
        /// <summary>
        /// 用于伸缩等效果
        /// </summary>
        public RectTransform[] panels;
        /// <summary>
        /// 跟随激活
        /// </summary>
        public GameObject[] SetToggleCheckmarkObjectsActive;
        /// <summary>
        /// 跟随隐藏
        /// </summary>
        public GameObject[] SetToggleCheckmarkObjectsInactive;
        protected void Awake()
        {
            toggle = this.GetComponent<Toggle>();
        }
        protected void Start()
        {
            Refresh();
        }
        public void Refresh(bool value)
        {
            if (toggle.isOn != value)
                toggle.isOn = value;

            panels?.SetActive(value);
            SetToggleCheckmarkObjectsActive.SetActive(value);
            SetToggleCheckmarkObjectsInactive.SetActive(!value);
        }
        public void Refresh()
        {
            Refresh(toggle.isOn);
        }
    }
}
