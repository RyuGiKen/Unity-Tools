using System.Collections;
using System.Collections.Generic;
using RyuGiKen;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// ������ť
    /// </summary>
    [DisallowMultipleComponent]
    public class TriggerButton : MonoBehaviour
    {
        public TriggerButtonPointer pointer;
        public Image m_Image;
        /// <summary>
        /// ��ѡ�񣨷ǽ���״̬���ƣ�
        /// </summary>
        public bool canSelect = true;
        /// <summary>
        /// ���������ظ�����(s)
        /// </summary>
        public float RepeatedLimit = 1;
        /// <summary>
        /// �������
        /// </summary>
        public bool hightlight;
        /// <summary>
        /// ���
        /// </summary>
        public event MyEvent OnClick;
        [Tooltip("ѡ��Ľ��Ȱٷֱ�")][Range(0, 1)] public float SelectProgress;
        /// <summary>
        /// ѡ���ٶȣ������ʱ���٣�
        /// </summary>
        public Vector2 SelectSpeedRange = new Vector2(0.05f, 0.25f);//Լ10s
        /// <summary>
        /// ȡ��ѡ���ٶ�(s)
        /// </summary>
        public float UnSelectSpeed = 1;
        /// <summary>
        /// ѡ����Ȳ�ֵ����
        /// </summary>
        public bool ScaleInProgress;
        /// <summary>
        /// ѡ����Ȳ�ֵ���Ż���Image������Ϊthis
        /// </summary>
        public bool ScaleInProgressUseImage;
        [Range(0, 1)] public float ChangeStyleThresholdValueInProgress = 1;
        /// <summary>
        /// (ͨ��������)
        /// </summary>
        [Tooltip("���ŷ�Χ")] public Vector2 ScaleRange = Vector2.one * float.NaN;
        /// <summary>
        /// ����ʾ���ֻ��ˢ�½���ʱ��ʾ
        /// </summary>
        public bool ShowMaskOnlyInProgress = true;
        /// <summary>
        /// ���Ȳ�ֵ��ɫ
        /// </summary>
        public bool LerpColorInProgress;
        /// <summary>
        /// ѡ�����ʾ���
        /// </summary>
        public ProgressMaskImage[] SelectProgressMask;
        [Tooltip("�ο���ť")] public Button ReferenceButton;
        /// <summary>
        /// ����Button������ʾ״̬
        /// </summary>
        public bool interactable = true;
        public bool canClick = true;
        [Tooltip("ͨ��ͼ��")] public Sprite NormalSprite;
        [Tooltip("����ͼ��")] public Sprite HighlightedSprite;
        [Tooltip("����ͼ��")] public Sprite ForbiddenSprite;
        [Tooltip("ͨ����ɫ")] public Color NormalColor = Color.white;
        [Tooltip("������ɫ")] public Color HighlightedColor = new HSVColor(0, 0, 0.95f);
        [Tooltip("ͨ����ɫ")] public Color NormalColorInProgress = new Color(1, 1, 1, 0.95f);
        [Tooltip("������ɫ")] public Color HighlightedColorInProgress = Color.white;
        [Tooltip("������ɫ")] public Color ForbiddenColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
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
        /// ˢ�½���
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
        /// ˢ�½�����ʾ
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
        /// �����������
        /// </summary>
        public void OnClickEvent()
        {
            if (interactable && canClick)
                OnClick();
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="trigger">[0��1]</param>
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
        /// ���
        /// </summary>
        public virtual void Click()
        {
            Debug.Log("Click " + this);
        }
        /// <summary>
        /// �л���ɫ
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
