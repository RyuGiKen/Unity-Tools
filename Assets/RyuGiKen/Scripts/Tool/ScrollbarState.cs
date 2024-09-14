using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    [ExecuteAlways]
    [RequireComponent(typeof(Scrollbar))]
    public class ScrollbarState : MonoBehaviour
    {
        public Scrollbar scrollbar;
        public Text HandleText;
        public enum Mode
        {
            /// <summary>
            /// 分级调节数值，需要设置scrollbar.numberOfSteps
            /// </summary>
            StepValue,
            /// <summary>
            /// 无级调节数值
            /// </summary>
            SteplessValue,
            /// <summary>
            /// 分级切换
            /// </summary>
            Strings,
        }
        public Mode mode;
        public Vector2 ValueRange = Vector2.up;
        public string[] Names;
        protected virtual void Reset()
        {
            if (!scrollbar)
                this.TryGetComponent(out scrollbar);
            if (scrollbar)
                mode = scrollbar.numberOfSteps > 1 ? Mode.StepValue : Mode.SteplessValue;
        }
        protected virtual void Start()
        {

        }
        protected virtual void Update()
        {
            switch (mode)
            {
                case Mode.StepValue:
                    if (scrollbar.numberOfSteps <= 1)
                        scrollbar.numberOfSteps = 2;
                    break;
                case Mode.SteplessValue:
                    scrollbar.numberOfSteps = 0;
                    break;
                case Mode.Strings:
                    scrollbar.numberOfSteps = Names.Length;
                    break;
            }
        }
        protected virtual void LateUpdate()
        {

        }
        public virtual float GetValue()
        {
            float value = scrollbar.value;
            //if (scrollbar.numberOfSteps > 1)
            //    value = Mathf.Round(value * (scrollbar.numberOfSteps - 1)) / (scrollbar.numberOfSteps - 1);
            //return value;
            return Mathf.Lerp(ValueRange.x, ValueRange.y, value);
        }
        public virtual void SetValue(float value)
        {
            //scrollbar.value = value;
            scrollbar.value = Mathf.InverseLerp(ValueRange.x, ValueRange.y, value);
        }
    }
}
