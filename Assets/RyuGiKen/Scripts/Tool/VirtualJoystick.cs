using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
namespace RyuGiKen.Tools
{
    public enum AxisOption
    {
        Both,
        OnlyHorizontal,
        OnlyVertical
    }
    public enum Shape
    {
        Circle,
        Square,
    }
    /// <summary>
    /// 虚拟摇杆
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class VirtualJoystick : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        public RectTransform rectTransform;
        public AxisOption axis;
        public Shape shape;
        public Image BackgroundX;
        public Image BackgroundY;
        public Image BackgroundXY;
        public Image m_Handle;
        [Range(-1, 1)] public float ValueX;
        [Range(-1, 1)] public float ValueY;
        public float offset = 25;
        protected override void Awake()
        {
            base.Awake();
            rectTransform = this.GetComponent<RectTransform>();
        }
        protected override void Start()
        {
            base.Start();
        }
        private void LateUpdate()
        {
            BackgroundX.SetActive(axis == AxisOption.OnlyHorizontal);
            BackgroundY.SetActive(axis == AxisOption.OnlyVertical);
            BackgroundXY.SetActive(axis == AxisOption.Both);
        }
        public Vector2 m_Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }
        public Vector2 GetValue()
        {
            Vector2 result = new Vector2(ValueX.Clamp(-1, 1), ValueY.Clamp(-1, 1));
            switch (axis)
            {
                case AxisOption.OnlyHorizontal:
                    result.y = 0;
                    break;
                case AxisOption.OnlyVertical:
                    result.x = 0;
                    break;
            }
            return result;
        }
        public void SetX(float x)
        {
            SetValue(x, ValueY);
        }
        public void SetY(float y)
        {
            SetValue(ValueX, y);
        }
        public void SetValue(float x, float y)
        {
            switch (axis)
            {
                default:
                case AxisOption.Both:
                    ValueX = x.Clamp(-1, 1);
                    ValueY = y.Clamp(-1, 1);
                    break;
                case AxisOption.OnlyHorizontal:
                    ValueX = x.Clamp(-1, 1);
                    ValueY = 0;
                    break;
                case AxisOption.OnlyVertical:
                    ValueX = 0;
                    ValueY = y.Clamp(-1, 1);
                    break;
            }
            UpdateVisuals();
        }
        public void SetValue(Vector2 value)
        {
            SetValue(value.x, value.y);
        }
        [Serializable]
        public class JoystickEvent : UnityEvent<Vector2> { }
        [SerializeField]
        private JoystickEvent m_OnValueChanged = new JoystickEvent();
        public JoystickEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }
        //[SerializeField] private Vector2 m_Offset = Vector2.zero;
        //private DrivenRectTransformTracker m_Tracker;
        private bool m_DelayedUpdateVisuals = false;
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                UpdateCachedReferences();
                // Update rects in next update since other things might affect them even if value didn't change.
                m_DelayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }
#endif

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            //if (executing == CanvasUpdate.Prelayout)
            //    onValueChanged.Invoke(Value);
#endif
        }

        /// <summary>
        /// See ICanvasElement.LayoutComplete
        /// </summary>
        public virtual void LayoutComplete() { }

        /// <summary>
        /// See ICanvasElement.GraphicUpdateComplete
        /// </summary>
        public virtual void GraphicUpdateComplete() { }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            Set(m_Value, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            //m_Tracker.Clear();
            base.OnDisable();
        }

        /// <summary>
        /// Update the rect based on the delayed update visuals.
        /// Got around issue of calling sendMessage from onValidate.
        /// </summary>
        protected virtual void Update()
        {
            if (m_DelayedUpdateVisuals)
            {
                m_DelayedUpdateVisuals = false;
                Set(m_Value, false);
                UpdateVisuals();
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            //m_Value = ClampValue(m_Value);
            //float oldNormalizedValue = NormalizedValue;

            //if (m_FillImage)
            //    oldNormalizedValue = NormalizedAngleToNormalizedValue(m_FillImage.fillAmount);

            UpdateVisuals();

            //if (oldNormalizedValue != NormalizedValue)
            //{
            //    UISystemProfilerApi.AddMarker("VirtualJoystick.value", this);
            //    onValueChanged.Invoke(m_Value);
            //}
        }

        void UpdateCachedReferences()
        {

        }

        Vector2 ClampValue(Vector2 input)
        {
            input.x = input.x.Clamp(-1, 1);
            input.y = input.y.Clamp(-1, 1);
            return input;
        }
        protected virtual void Set(Vector2 input, bool sendCallback = true)
        {
            Vector2 newValue = ClampValue(input);

            if (m_Value == newValue)
                return;

            m_Value = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("VirtualJoystick.value", this);
                m_OnValueChanged.Invoke(newValue);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }
        public float radius
        {
            get
            {
                float min = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height);
                if (offset >= min * 0.5f || offset < 0)
                    offset = 0;
                return min * 0.5f - offset;
            }
        }
        /// <summary>
        /// 显示刷新
        /// </summary>
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif
            //if (!Mathf.Approximately(rectTransform.rect.width / rectTransform.rect.height, 1))
            //{
            //
            //}
            //m_Tracker.Clear();
            //m_Tracker.Add(this, m_FillImage.rectTransform, DrivenTransformProperties.Anchors);

            if (m_Handle)
            {
                RectTransform handleParent = (m_Handle.transform.parent as RectTransform);
                handleParent.localRotation = Quaternion.identity;
                m_Handle.transform.localRotation = Quaternion.identity;
                //m_Handle.preserveAspect = true;
                m_Handle.rectTransform.anchorMin = m_Handle.rectTransform.anchorMax = Vector2.one * 0.5f;
                m_Handle.rectTransform.pivot = Vector2.one * 0.5f;

                Vector2 value = GetValue();
                switch (shape)
                {
                    default:
                    case Shape.Circle:
                        m_Handle.rectTransform.anchoredPosition = (value.magnitude >= 1 ? value.normalized : value) * radius;
                        break;
                    case Shape.Square:
                        m_Handle.rectTransform.anchoredPosition = value * radius;
                        break;
                }
            }
            //m_Tracker.Add(this, m_Handle.rectTransform, DrivenTransformProperties.Anchors);
        }
        /// <summary>
        /// 更新拖动位置
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="cam"></param>
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            //RectTransform clickRect = (this.rectTransform.parent) as RectTransform;
            RectTransform clickRect = m_Handle.rectTransform;
            if (clickRect && clickRect.rect.size.magnitude > 0)
            {
                //Vector2 position = Vector2.zero;
                //if (!UnityEngine.UI.MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                //    return;
                /*if (RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.pointerCurrentRaycast.screenPosition, cam, out Vector2 localCursor))
                {
                    Vector2 temp = localCursor + clickRect.anchoredPosition;
                    if (localCursor.magnitude > this.rectTransform.rect.size.magnitude * 2)
                        return;
                    switch (shape)
                    {
                        default:
                        case Shape.Circle:
                            ValueAdjust.RectToPolar(temp, out float angle, out float distance);
                            m_Value = temp.normalized * distance.Clamp(0, radius) / radius;
                            break;
                        case Shape.Square:
                            m_Value = temp / radius;
                            break;
                    }
                }*/
                Vector2 temp = m_Handle.transform.parent.InverseTransformPoint(eventData.position);
                switch (shape)
                {
                    default:
                    case Shape.Circle:
                        ValueAdjust.RectToPolar(temp, out float angle, out float distance);
                        m_Value = temp.normalized * distance.Clamp(0, radius) / radius;
                        break;
                    case Shape.Square:
                        m_Value = temp / radius;
                        break;
                }
            }
        }
        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            //m_Offset = Vector2.zero;
            if (RectTransformUtility.RectangleContainsScreenPoint(m_Handle.rectTransform, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                UpdateOffset(eventData, eventData.enterEventCamera);
            }
            else
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }
        void UpdateOffset(PointerEventData eventData, Camera cam)
        {
            //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Handle.rectTransform.parent as RectTransform, eventData.pointerPressRaycast.screenPosition, cam, out Vector2 localMousePos))
            //{
            //    m_Offset = localMousePos;
            //}
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;
            UpdateDrag(eventData, eventData.enterEventCamera);
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
    }
}