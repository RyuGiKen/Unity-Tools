using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using SliderEvent = UnityEngine.UI.Slider.SliderEvent;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 圆形滚动条
    /// </summary>
    [AddComponentMenu("RyuGiKen/圆形滚动条")]
    [RequireComponent(typeof(RectTransform))]
    public class CircleSlider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        public Image m_Background;
        public Image m_FillImage;
        public Image m_Handle;
        [Space]
        [SerializeField] float m_MinValue = 0;
        public float MinValue
        {
            get
            {
                float min = Mathf.Min(m_MinValue, m_MaxValue);
                return m_WholeNumbers ? Mathf.CeilToInt(min) : min;
            }
            set
            {
                if (m_MinValue != value)
                    m_MinValue = value > MaxValue ? MinValue : ((m_WholeNumbers ? Mathf.CeilToInt(value) : value));
            }
        }
        [SerializeField] float m_MaxValue = 1;
        public float MaxValue
        {
            get
            {
                float max = Mathf.Max(m_MinValue, m_MaxValue);
                return m_WholeNumbers ? Mathf.FloorToInt(max) : max;
            }
            set
            {
                if (m_MaxValue != value)
                    m_MaxValue = value < MinValue ? MaxValue : ((m_WholeNumbers ? Mathf.FloorToInt(value) : value));
            }
        }
        public bool m_WholeNumbers = false;
        [SerializeField] float m_Value = 0;
        public virtual float Value
        {
            get
            {
                if (m_WholeNumbers)
                    return ClampValue(m_Value);
                return m_Value;
            }
            set
            {
                if (m_Value != value)
                    Set(value);
            }
        }
        [Space]
        public bool fillClockwise = false;

        [SerializeField][Range(0, 359.999f)] float m_MinAngle = 0;
        public float MinAngle
        {
            get
            {
                float min = Mathf.Min(m_MinAngle, m_MaxAngle);
                return /*m_WholeNumbers ? Mathf.CeilToInt(min) : */min;
            }
            set
            {
                if (m_MinAngle != value)
                    m_MinAngle = value > m_MaxAngle ? m_MinAngle : ValueAdjust.SetRange(value, 0, 360, 360);
            }
        }
        [SerializeField][Range(0, 359.999f)] float m_MaxAngle = 360;
        public float MaxAngle
        {
            get
            {
                float max = Mathf.Max(m_MinAngle, m_MaxAngle);
                return /*m_WholeNumbers ? Mathf.FloorToInt(max) : */max;
            }
            set
            {
                if (m_MaxAngle != value)
                    m_MaxAngle = value < m_MinAngle ? m_MaxAngle : ValueAdjust.SetRange(value, 0, 360, 360);
            }
        }
        public Vector2 AngleRange
        {
            get
            {
                return new Vector2(MinAngle, MaxAngle);
            }
            set
            {
                if (m_MinAngle != value.x || m_MaxAngle != value.y)
                    ValueAdjust.FindMinAndMax(ValueAdjust.SetRange(value.x, 0, 360, 360), ValueAdjust.SetRange(value.y, 0, 360, 360), out m_MinAngle, out m_MaxAngle);
            }
        }
        [Space]
        public Image.FillMethod fillMethod = Image.FillMethod.Radial360;
        public int fillOrigin = 1;
        public enum HandleFilledType
        {
            /// <summary>
            /// 填充
            /// </summary>
            Filled,
            /// <summary>
            /// 局部跟随旋转
            /// </summary>
            LocalRotation,
            /// <summary>
            /// 全局跟随旋转
            /// </summary>
            WorldRotation,
        }
        public HandleFilledType HandleFilled = HandleFilledType.Filled;
        [SerializeField][Range(0, 359.999f)] float m_FillAngleRange = 30;
        public float FillAngleRange
        {
            get
            {
                return m_FillAngleRange.Abs().Clamp(0, MaxAngle - MinAngle);
            }
            set
            {
                if (m_FillAngleRange != value)
                    m_FillAngleRange = ValueAdjust.SetRange(value, 0, 360, 360).Clamp(0, MaxAngle - MinAngle);
            }
        }
        [SerializeField] float m_Angle;
        /// <summary>
        /// 角度[0,360]
        /// </summary>
        public float Angle
        {
            get
            {
                m_Angle = NormalizedValueToAngle(NormalizedValue);
                return m_Angle;
            }
            set
            {
                if (m_Angle != value)
                {
                    m_Angle = value;
                    this.Value = NormalizedValueToValue(AngleToNormalizedValue(value));
                }
            }
        }
        /// <summary>
        /// 基于[0，360]的归一化角度[0，1]
        /// </summary>
        public float NormalizedAngle
        {
            get
            {
                return NormalizedValueToNormalizedAngle(NormalizedValue);
            }
            set
            {
                Angle = NormalizedAngleToAngle(value);
            }
        }
        /// <summary>
        /// Set the value of the slider without invoking onValueChanged callback.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        public virtual void SetValueWithoutNotify(float input)
        {
            Set(input, false);
        }
        /// <summary>
        /// 值归一化[0，1]
        /// </summary>
        public float NormalizedValue
        {
            get
            {
                return ValueToNormalizedValue(Value);
            }
            set
            {
                if (ValueToNormalizedValue(Value) != value)
                    this.Value = NormalizedValueToValue(value);
            }
        }
        public RectTransform rectTransform { get { return this.GetComponent<RectTransform>(); } }
        [Space]

        [SerializeField]
        private SliderEvent m_OnValueChanged = new SliderEvent();

        /// <summary>
        /// Callback executed when the value of the slider is changed.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     public void Start()
        ///     {
        ///         //Adds a listener to the main slider and invokes a method when the value changes.
        ///         mainSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        ///     }
        ///
        ///     // Invoked when the value of the slider changes.
        ///     public void ValueChangeCheck()
        ///     {
        ///         Debug.Log(mainSlider.value);
        ///     }
        /// }
        /// </code>
        /// </example>
        public SliderEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        /// <summary>
        /// 鼠标位移差
        /// </summary>
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals = false;
        public float NormalizedValueToValue(float normalizedValue)
        {
            return Mathf.Lerp(MinValue, MaxValue, normalizedValue);
        }
        public float ValueToNormalizedValue(float value)
        {
            if (Mathf.Approximately(MinValue, MaxValue))
                return 0;
            return Mathf.InverseLerp(MinValue, MaxValue, value);
        }
        public float NormalizedValueToNormalizedAngle(float normalizedValue)
        {
            return AngleToNormalizedAngle(NormalizedValueToAngle(normalizedValue));
        }
        public float NormalizedValueToAngle(float normalizedValue)
        {
            if (fillClockwise)
                return Mathf.Lerp(MaxAngle, MinAngle, normalizedValue);
            else
                return Mathf.Lerp(MinAngle, MaxAngle, normalizedValue);
        }
        public float NormalizedAngleToNormalizedValue(float normalizedAngle)
        {
            return AngleToNormalizedValue(NormalizedAngleToAngle(normalizedAngle));
        }
        public float ValueToAngle(float value)
        {
            return NormalizedValueToAngle(ValueToNormalizedValue(value));
        }
        public float AngleToValue(float angle)
        {
            return NormalizedValueToValue(AngleToNormalizedValue(angle));
        }
        public float AngleToNormalizedValue(float angle)
        {
            return fillClockwise ? Mathf.InverseLerp(MaxAngle, MinAngle, angle) : Mathf.InverseLerp(MinAngle, MaxAngle, angle);
        }
        public float AngleToNormalizedAngle(float angle)
        {
            return ValueAdjust.SetRange(angle, 0, 360, 360) / 360f;
        }
        public float NormalizedAngleToAngle(float normalizedAngle)
        {
            return normalizedAngle * 360f;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_WholeNumbers)
            {
                m_MinValue = Mathf.Round(m_MinValue);
                m_MaxValue = Mathf.Round(m_MaxValue);
            }

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

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(Value);
#endif
        }

        /// <summary>
        /// See ICanvasElement.LayoutComplete
        /// </summary>
        public virtual void LayoutComplete()
        { }

        /// <summary>
        /// See ICanvasElement.GraphicUpdateComplete
        /// </summary>
        public virtual void GraphicUpdateComplete()
        { }

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
            m_Tracker.Clear();
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
            m_Value = ClampValue(m_Value);
            float oldNormalizedValue = NormalizedValue;

            if (m_FillImage)
                oldNormalizedValue = NormalizedAngleToNormalizedValue(m_FillImage.fillAmount);

            UpdateVisuals();

            if (oldNormalizedValue != NormalizedValue)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                onValueChanged.Invoke(m_Value);
            }
        }

        void UpdateCachedReferences()
        {

        }

        float ClampValue(float input)
        {
            float newValue = Mathf.Clamp(input, MinValue, MaxValue);
            if (m_WholeNumbers)
                newValue = Mathf.RoundToInt(newValue).Clamp(MinValue, MaxValue);
            return newValue;
        }

        /// <summary>
        /// Set the value of the slider.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        /// <param name="sendCallback">If the OnValueChanged callback should be invoked.</param>
        /// <remarks>
        /// Process the input to ensure the value is between min and max value. If the input is different set the value and send the callback is required.
        /// </remarks>
        protected virtual void Set(float input, bool sendCallback = true)
        {
            // Clamp the input
            float newValue = ClampValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_Value == newValue)
                return;

            m_Value = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
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

        /// <summary>
        /// 显示刷新
        /// </summary>
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif
            m_Tracker.Clear();

            m_Tracker.Add(this, m_FillImage.rectTransform, DrivenTransformProperties.Anchors);

            m_FillImage.type = m_Background.type = Image.Type.Filled;
            switch (fillMethod)
            {
                default:
                    fillMethod = Image.FillMethod.Radial360;
                    break;
                case Image.FillMethod.Radial360:
                    break;
            }
            float BackgroundFill = AngleToNormalizedAngle((AngleRange.y - AngleRange.x).Abs());
            if (m_FillImage)
            {
                m_FillImage.type = Image.Type.Filled;
                m_FillImage.fillAmount = NormalizedValue * BackgroundFill;
                m_FillImage.fillClockwise = fillClockwise;
                m_FillImage.rectTransform.localEulerAngles = AdjustVecterZ(m_FillImage.rectTransform.localEulerAngles, fillClockwise ? MaxAngle : MinAngle);
                AdjustRectTransformOffset(m_FillImage.rectTransform);
                m_FillImage.fillMethod = fillMethod;
                m_FillImage.fillOrigin = fillOrigin;
                m_FillImage.preserveAspect = true;
            }
            if (m_Background)
            {
                m_Background.type = Image.Type.Filled;
                m_Background.fillAmount = BackgroundFill;
                m_Background.fillClockwise = false;
                m_Background.rectTransform.localEulerAngles = AdjustVecterZ(m_Background.rectTransform.localEulerAngles, MinAngle);
                AdjustRectTransformOffset(m_Background.rectTransform);
                m_Background.fillMethod = fillMethod;
                m_Background.fillOrigin = fillOrigin;
                m_Background.preserveAspect = true;
            }
            if (m_Handle)
            {
                RectTransform handleParent = (m_Handle.transform.parent as RectTransform);
                m_Handle.preserveAspect = true;
                AdjustRectTransformOffset(handleParent);
                switch (HandleFilled)
                {
                    case HandleFilledType.Filled:
                        m_Handle.type = Image.Type.Filled;
                        m_Handle.fillAmount = AngleToNormalizedAngle(FillAngleRange);
                        m_Handle.fillClockwise = false;
                        m_Handle.rectTransform.localEulerAngles = AdjustVecterZ(m_Handle.rectTransform.localEulerAngles, ValueAdjust.MappingRange(Angle, MinAngle, MaxAngle, MinAngle, MaxAngle - FillAngleRange));
                        AdjustRectTransformOffset(m_Handle.rectTransform);
                        handleParent.localRotation = Quaternion.identity;
                        m_Handle.fillMethod = fillMethod;
                        m_Handle.fillOrigin = fillOrigin;
                        break;
                    case HandleFilledType.LocalRotation:
                    case HandleFilledType.WorldRotation:
                        m_Handle.type = Image.Type.Simple;
                        if (m_Handle.rectTransform.anchorMin.JudgeRange(Vector2.zero, Vector2.one * 0.01f) && m_Handle.rectTransform.anchorMax.JudgeRange(Vector2.one, Vector2.one * 0.01f))
                        {
                            m_Handle.rectTransform.anchoredPosition = new Vector2(handleParent.rect.width * 0.5f, 0);
                        }
                        if (Mathf.Approximately(m_Handle.rectTransform.sizeDelta.magnitude, 0))
                        {
                            m_Handle.rectTransform.sizeDelta = handleParent.rect.width * 0.5f * Vector2.one;
                        }
                        m_Handle.rectTransform.anchorMin = m_Handle.rectTransform.anchorMax = Vector2.one * 0.5f;
                        handleParent.localEulerAngles = AdjustVecterZ(handleParent.localEulerAngles, Angle + 90 * (fillOrigin - 1));
                        switch (HandleFilled)
                        {
                            case HandleFilledType.LocalRotation:
                                m_Handle.transform.localRotation = Quaternion.identity;
                                break;
                            case HandleFilledType.WorldRotation:
                                m_Handle.transform.rotation = Quaternion.identity;
                                break;
                        }
                        break;
                }
            }

            m_Tracker.Add(this, m_Handle.rectTransform, DrivenTransformProperties.Anchors);
        }
        Vector3 AdjustVecterZ(Vector3 value, float z)
        {
            value.z = z;
            return value;
        }
        void AdjustRectTransformOffset(RectTransform rectTransform)
        {
            if (rectTransform.anchorMin.JudgeRange(Vector2.one * 0.5f, Vector2.one * 0.01f) && rectTransform.anchorMax.JudgeRange(Vector2.one * 0.5f, Vector2.one * 0.01f))
            {
                rectTransform.anchoredPosition = Vector2.zero;
                if (rectTransform.sizeDelta.magnitude < 1)
                    rectTransform.sizeDelta = this.rectTransform.sizeDelta;
            }
            else //if (!rectTransform.offsetMin.JudgeRange(Vector2.zero, Vector2.one * 0.01f) || !rectTransform.offsetMax.JudgeRange(Vector2.zero, Vector2.one * 0.01f))
            {
                rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
                if (Mathf.Approximately(rectTransform.anchorMin.magnitude, 0) && Mathf.Approximately(rectTransform.anchorMax.magnitude, 0))
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                }
                else if (!Mathf.Approximately(rectTransform.anchorMin.magnitude, 0) && !ValueAdjust.JudgeRange(rectTransform.anchorMax, Vector2.one, Vector2.one * 0.001f))
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                }
            }
        }
        /// <summary>
        /// 更新拖动位置
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="cam"></param>
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = (this.rectTransform.parent) as RectTransform;
            if (clickRect && clickRect.rect.size.magnitude > 0)
            {
                //Vector2 position = Vector2.zero;
                //if (!UnityEngine.UI.MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                //    return;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.pointerCurrentRaycast.screenPosition, cam, out m_Offset))
                {
                    //localCursor -= clickRect.rect.position;
                    //float val = Mathf.Clamp01((localCursor - m_Offset) / clickRect.rect.size);
                    //NormalizedValue = val;
                    ValueAdjust.RectToPolar(m_Offset, out float angle, out float distance);
                    Angle = ValueAdjust.SetRange(angle - 90 * (fillOrigin - 1), 0, 360, 360);
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
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Handle.rectTransform.parent as RectTransform, eventData.pointerPressRaycast.screenPosition, cam, out Vector2 localMousePos))
            {
                m_Offset = localMousePos;
            }
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
#if UNITY_EDITOR
    [CustomEditor(typeof(CircleSlider), true)]
    [CanEditMultipleObjects]
    public class CircleSliderEditor : SelectableEditor
    {
        SerializedProperty m_Background;
        SerializedProperty m_FillRect;
        SerializedProperty m_HandleRect;
        SerializedProperty m_MinValue;
        SerializedProperty m_MaxValue;
        SerializedProperty m_Value;
        SerializedProperty m_OnValueChanged;
        SerializedProperty m_WholeNumbers;

        SerializedProperty m_MinAngle;
        SerializedProperty m_MaxAngle;
        SerializedProperty m_FillAngleRange;
        SerializedProperty fillClockwise;
        SerializedProperty fillOrigin;
        SerializedProperty HandleFilled;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Background = serializedObject.FindProperty("m_Background");
            m_FillRect = serializedObject.FindProperty("m_FillImage");
            m_HandleRect = serializedObject.FindProperty("m_Handle");

            m_MinValue = serializedObject.FindProperty("m_MinValue");
            m_MaxValue = serializedObject.FindProperty("m_MaxValue");
            m_Value = serializedObject.FindProperty("m_Value");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
            m_WholeNumbers = serializedObject.FindProperty("m_WholeNumbers");

            m_MinAngle = serializedObject.FindProperty("m_MinAngle");
            m_MaxAngle = serializedObject.FindProperty("m_MaxAngle");
            m_FillAngleRange = serializedObject.FindProperty("m_FillAngleRange");
            fillClockwise = serializedObject.FindProperty("fillClockwise");
            fillOrigin = serializedObject.FindProperty("fillOrigin");
            HandleFilled = serializedObject.FindProperty("HandleFilled");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Background);
            EditorGUILayout.PropertyField(m_FillRect);
            EditorGUILayout.PropertyField(m_HandleRect);

            if (m_FillRect.objectReferenceValue != null || m_HandleRect.objectReferenceValue != null)
            {
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                float newMin = EditorGUILayout.FloatField("Min Value", m_MinValue.floatValue);
                if (EditorGUI.EndChangeCheck() && newMin <= m_MaxValue.floatValue)
                {
                    m_MinValue.floatValue = newMin;
                }

                EditorGUI.BeginChangeCheck();
                float newMax = EditorGUILayout.FloatField("Max Value", m_MaxValue.floatValue);
                if (EditorGUI.EndChangeCheck() && newMax >= m_MinValue.floatValue)
                {
                    m_MaxValue.floatValue = newMax;
                }

                EditorGUILayout.PropertyField(m_WholeNumbers);
                EditorGUILayout.Slider(m_Value, (target as CircleSlider).MinValue, (target as CircleSlider).MaxValue);
                EditorGUILayout.Space();

                EditorGUI.BeginChangeCheck();
                newMin = EditorGUILayout.Slider("Min Angle", m_MinAngle.floatValue, 0, 360);
                if (EditorGUI.EndChangeCheck() && newMin <= m_MaxAngle.floatValue)
                {
                    m_MinAngle.floatValue = newMin.Clamp(0, 359.999f);
                }

                EditorGUI.BeginChangeCheck();
                newMax = EditorGUILayout.Slider("Max Angle", m_MaxAngle.floatValue, 0, 360);
                if (EditorGUI.EndChangeCheck() && newMax >= m_MinAngle.floatValue)
                {
                    m_MaxAngle.floatValue = newMax.Clamp(0, 359.999f);
                }

                EditorGUILayout.Slider("Angle", (target as CircleSlider).Angle, (target as CircleSlider).MinAngle, (target as CircleSlider).MaxAngle);
                EditorGUILayout.Slider(m_FillAngleRange, 0, (target as CircleSlider).MaxAngle - (target as CircleSlider).MinAngle);
                EditorGUILayout.PropertyField(fillClockwise);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(HandleFilled);
                if (EditorGUI.EndChangeCheck())
                {
                    CircleSlider.HandleFilledType type = (CircleSlider.HandleFilledType)HandleFilled.enumValueIndex;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        CircleSlider slider = obj as CircleSlider;
                        slider.HandleFilled = type;
                    }
                }
                EditorGUI.BeginChangeCheck();
                if (EditorGUI.EndChangeCheck())
                {
                    fillOrigin.intValue = (int)(Image.Origin360)EditorGUILayout.EnumPopup("Fill Origin", (Image.Origin360)fillOrigin.intValue);
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
