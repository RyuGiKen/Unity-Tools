using RyuGiKen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// ÕÛµþÃæ°å
    /// </summary>
    [ExecuteAlways]
    public class FoldoutPanel : MonoBehaviour
    {
        public RectTransform rectTransform;
        public LayoutGroup ParentLayoutGroup;
        [Tooltip("ÕÛµþ")] public bool isFold = true;
        public RectTransform Title;
        public RectTransform Content;
        public LayoutGroup ContentLayoutGroup;
        public List<GameObject> SetFoldoutObjectsActive;
        public List<GameObject> SetFoldoutObjectsInactive;
        protected void Awake()
        {
            if (!rectTransform)
                rectTransform = GetComponent<RectTransform>();
        }
        protected void Start()
        {

        }
        protected void Update()
        {
            UpdataSize();
        }
        protected void LateUpdate()
        {

        }
        public void UpdataSize()
        {
            Vector2 size = rectTransform.rect.size;
            size.y = Title.rect.height;
            if (!isFold)
                size.y += Content.rect.height;
            Vector2 pos = Content.anchoredPosition;
            pos.y = -Title.rect.height;
            Content.anchoredPosition = pos;

            if (!Mathf.Approximately(rectTransform.rect.size.y, size.y))
            {
                rectTransform.sizeDelta = size;
                Refresh(isFold);
            }
        }
        public void SwitchFoldout()
        {
            isFold = !isFold;
            Refresh(isFold);
        }
        public void UpdatePanelLayout()
        {
            LayoutRebuilder.MarkLayoutForRebuild(Content);
        }
        public void UpdateParentLayout()
        {
            if (!ParentLayoutGroup)
                return;
            //ParentLayoutGroup.CalculateLayoutInputVertical();
            //ParentLayoutGroup.SetLayoutVertical();
            LayoutRebuilder.MarkLayoutForRebuild(ParentLayoutGroup.GetComponent<RectTransform>());
        }
        public void Refresh(bool value)
        {
            if (isFold != value)
                isFold = value;

            Content.SetActive(!isFold);
            SetFoldoutObjectsActive.SetActive(isFold);
            SetFoldoutObjectsInactive.SetActive(!isFold);
        }
    }
}
