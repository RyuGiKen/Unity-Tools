using System.Collections;
using System.Collections.Generic;
using RyuGiKen;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 触发按钮
    /// </summary>
    [DisallowMultipleComponent]
    public class TriggerButton : MonoBehaviour
    {
        public TriggerButtonPointer pointer;
        public Image m_Image;
        /// <summary>
        /// 可选择（非禁用状态限制）
        /// </summary>
        public bool canSelect = true;
        /// <summary>
        /// 被点击后的重复限制(s)
        /// </summary>
        public float RepeatedLimit = 1;
        /// <summary>
        /// 高亮标记
        /// </summary>
        public bool hightlight;
        /// <summary>
        /// 点击
        /// </summary>
        public event MyEvent OnClick;
        [Tooltip("选择的进度百分比")][Range(0, 1)] public float SelectProgress;
        /// <summary>
        /// 选择速度（不相等时变速）
        /// </summary>
        public Vector2 SelectSpeedRange = new Vector2(0.05f, 0.25f);//约10s
        /// <summary>
        /// 取消选择速度(s)
        /// </summary>
        public float UnSelectSpeed = 1;
        /// <summary>
        /// 选择进度插值缩放
        /// </summary>
        public bool ScaleInProgress;
        /// <summary>
        /// 选择进度插值缩放基于Image，否则为this
        /// </summary>
        public bool ScaleInProgressUseImage;
        [Range(0, 1)] public float ChangeStyleThresholdValueInProgress = 1;
        /// <summary>
        /// (通常，高亮)
        /// </summary>
        [Tooltip("缩放范围")] public Vector2 ScaleRange = Vector2.one * float.NaN;
        /// <summary>
        /// 进度示意框只在刷新进度时显示
        /// </summary>
        public bool ShowMaskOnlyInProgress = true;
        /// <summary>
        /// 进度插值颜色
        /// </summary>
        public bool LerpColorInProgress;
        /// <summary>
        /// 选择进度示意框
        /// </summary>
        public ProgressMaskImage[] SelectProgressMask;
        [Tooltip("参考按钮")] public Button ReferenceButton;
        /// <summary>
        /// 区分Button禁用显示状态
        /// </summary>
        public bool interactable = true;
        public bool canClick = true;
        [Tooltip("通常图形")] public Sprite NormalSprite;
        [Tooltip("高亮图形")] public Sprite HighlightedSprite;
        [Tooltip("禁用图形")] public Sprite ForbiddenSprite;
        [Tooltip("通常颜色")] public Color NormalColor = Color.white;
        [Tooltip("高亮颜色")] public Color HighlightedColor = new HSVColor(0, 0, 0.95f);
        [Tooltip("通常颜色")] public Color NormalColorInProgress = new Color(1, 1, 1, 0.95f);
        [Tooltip("高亮颜色")] public Color HighlightedColorInProgress = Color.white;
        [Tooltip("禁用颜色")] public Color ForbiddenColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        [System.Serializable]
        public struct ProgressMaskImage
        {
            public bool Direction;
            public Image Image;
        }
        protected void Reset()
        {
            ReferenceButton = this.GetComponent<Button>();
            m_Image = this.GetComponent<Image>();
            pointer = FindObjectOfType<TriggerButtonPointer>();
        }
        protected virtual void Awake()
        {
            OnClick = Click;
        }
        protected virtual void Start()
        {
            Trigger(0);
        }
        protected virtual void LateUpdate()
        {
            interactable = !ReferenceButton || (ReferenceButton && ReferenceButton.interactable);
            if (ReferenceButton && !ReferenceButton.interactable)
            {
                if (m_Image)
                {
                    if (NormalSprite)
                        m_Image.sprite = ForbiddenSprite ? ForbiddenSprite : NormalSprite;
                    m_Image.color = ForbiddenColor;
                }
                SelectProgress = 0;
                UpdateProgressMask();
            }
            else if (pointer)
            {
                bool hightlight = pointer.canTriggerObj && pointer.TriggerObj == this;
                if (pointer.TriggerByTime)
                {
                    this.hightlight = hightlight;
                    UpdateProgress();
                    Trigger(SelectProgress);
                    UpdateProgressMask();
                    if ((SelectProgress - 1) / SelectSpeedRange.y >= RepeatedLimit)
                    {
                        SelectProgress = 0;
                        pointer.LastClickObj = null;
                    }
                    else if (SelectProgress >= 1 && pointer.LastClickObj != this)
                    {
                        pointer.OnClick();
                    }
                }
                else
                {
                    //if (hightlight != this.hightlight)
                    {
                        Trigger(hightlight ? 1 : 0);
                    }
                }
                //if ((SelectProgress-1)/ SelectSpeedRange.y >= RepeatedLimit && Selected)
                //    Selected = false;
            }
        }
        /// <summary>
        /// 刷新进度
        /// </summary>
        public virtual void UpdateProgress()
        {
            if (hightlight && canSelect)
                SelectProgress += Time.deltaTime * ValueAdjust.MappingRange(SelectProgress, Vector2.up, SelectSpeedRange);
            else if (Mathf.Approximately(UnSelectSpeed, 0))
                SelectProgress = 0;
            else if (SelectProgress > 1)
                SelectProgress = 0;
            else
                SelectProgress = ValueAdjust.Lerp(SelectProgress, 0, Time.deltaTime, UnSelectSpeed);
        }
        /// <summary>
        /// 刷新进度显示
        /// </summary>
        public virtual void UpdateProgressMask()
        {
            if (SelectProgressMask.CheckArrayLength(1))
            {
                for (int i = 0; i < SelectProgressMask.Length; i++)
                {
                    if (SelectProgressMask[i].Image)
                    {
                        SelectProgressMask[i].Image.fillAmount = SelectProgressMask[i].Direction ? SelectProgress : (1 - SelectProgress);
                        SelectProgressMask[i].Image.enabled = ShowMaskOnlyInProgress ? (SelectProgress > 0 && SelectProgress < 1) : true;
                    }
                }
            }
        }
        /// <summary>
        /// 点击触发调用
        /// </summary>
        public void OnClickEvent()
        {
            if (interactable && canClick)
                OnClick();
        }
        /// <summary>
        /// 触碰
        /// </summary>
        /// <param name="trigger">[0，1]</param>
        public virtual void Trigger(float trigger)
        {
            //ChangeColor(trigger > 0);
            ChangeColor(ChangeStyleThresholdValueInProgress <= 0 ? (trigger > 0) : (trigger >= ChangeStyleThresholdValueInProgress));
            if (!float.IsNaN(ScaleRange.x) && !float.IsNaN(ScaleRange.y))
            {
                if (ScaleInProgressUseImage && m_Image)
                    m_Image.transform.localScale = Vector3.one * Mathf.Lerp(ScaleRange.x, ScaleRange.y, trigger);
                else
                    this.transform.localScale = Vector3.one * Mathf.Lerp(ScaleRange.x, ScaleRange.y, trigger);
            }
        }
        /// <summary>
        /// 点击
        /// </summary>
        public virtual void Click()
        {
            Debug.Log("Click " + this);
        }
        /// <summary>
        /// 切换颜色
        /// </summary>
        /// <param name="highlight"></param>
        public virtual void ChangeColor(bool highlight)
        {
            hightlight = highlight;
            if (m_Image)
            {
                if (HighlightedSprite && NormalSprite)
                    m_Image.sprite = highlight ? HighlightedSprite : NormalSprite;
                if (SelectProgress > 0)
                {
                    if (highlight)
                        m_Image.color = HighlightedColorInProgress;
                    else if (LerpColorInProgress && ChangeStyleThresholdValueInProgress > 0)
                        m_Image.color = Color.Lerp(NormalColorInProgress, HighlightedColorInProgress, SelectProgress / ChangeStyleThresholdValueInProgress);
                    else
                        m_Image.color = NormalColorInProgress;
                }
                else
                {
                    m_Image.color = highlight ? HighlightedColor : NormalColor;
                }
            }
        }
    }
}
