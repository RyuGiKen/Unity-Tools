using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using RyuGiKen;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 绘制记录
    /// </summary>
    public class DrawData : MonoBehaviour
    {
        public static DrawData instance;
        [Tooltip("绘制点父级")] public RectTransform DataImageParent;
        [Tooltip("绘制点")] public RectTransform DataImage_Prefab;
        [Tooltip("绘制点")] public List<RectTransform> DataImages;
        [Tooltip("记录")] public List<float> RecordData = new List<float>();
        [Tooltip("最大记录量")] public int MaxRecord = 1;
        [Tooltip("数据取值范围")] public Vector2 DataRange = new Vector2(0, 120);
        [Tooltip("范围")] public Text[] RangeText = new Text[2];
        [Tooltip("参考线")] List<RectTransform> ReferenceLine = new List<RectTransform>();
        [Tooltip("参考值")] public float[] ReferenceValue;
        private void Start()
        {
            instance = this;
            UpdateDataRange();
            SetMaxRecord((int)(DataImageParent.sizeDelta.x * 1f / DataImage_Prefab.sizeDelta.x));
        }
        /// <summary>
        /// 设置绘制记录量
        /// </summary>
        /// <param name="count"></param>
        public void SetMaxRecord(int count)
        {
            ClearList(DataImages);
            DataImage_Prefab.sizeDelta = new Vector2(DataImageParent.sizeDelta.x * 1f / count, 0);
            for (int i = 0; (i + 1) * DataImage_Prefab.sizeDelta.x < DataImageParent.sizeDelta.x; i++)
            {
                RectTransform go = Instantiate(DataImage_Prefab, DataImageParent);
                go.gameObject.SetActive(true);
                go.offsetMax = new Vector2((i + 1) * DataImage_Prefab.sizeDelta.x, DataImageParent.sizeDelta.y);
                go.offsetMin = new Vector2(i * DataImage_Prefab.sizeDelta.x, 0);
                DataImages.Add(go);
            }
            MaxRecord = DataImages.Count;
            ClearList(ReferenceLine);
            UpdateReferenceLine();
        }
        /// <summary>
        /// 刷新范围
        /// </summary>
        [ContextMenu("刷新范围")]
        void UpdateDataRange()
        {
            SetDataRange(DataRange.x, DataRange.y, ReferenceValue);
        }
        /// <summary>
        /// 设置范围
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="referenceValue"></param>
        public void SetDataRange(float min, float max, float[] referenceValue = null)
        {
            DataRange = new Vector2(min, max);
            RangeText[0].text = ShowNum(DataRange.x);
            RangeText[1].text = ShowNum(DataRange.y);
            ReferenceValue = referenceValue;
            UpdateReferenceLine();
        }
        public static string ShowNum(float num)
        {
            string result = num.ToString();
            if (((int)num).ToString() == num.ToString())
            {
                result = num.ToString("F0");
            }
            else if (Mathf.Abs(num) > 0.1f)
            {
                result = num.ToString("F3");
            }
            else if (Mathf.Abs(num) > 0.001f)
            {
                result = num.ToString("F3");
            }
            else
            {
                result = num.ToString("F8");
            }
            return result;
        }
        /// <summary>
        /// 清除列表
        /// </summary>
        void ClearList<T>(List<T> list) where T : Component
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Destroy(list[i].gameObject);
            }
            list.Clear();
        }
        /// <summary>
        /// 刷新参考线
        /// </summary>
        void UpdateReferenceLine()
        {
            if (ReferenceValue.Length != ReferenceLine.Count)
            {
                ClearList(ReferenceLine);
                for (int i = 0; i < ReferenceValue.Length; i++)
                {
                    RectTransform go = Instantiate(DataImage_Prefab, DataImageParent);
                    go.name = "参考线 " + ReferenceValue[i].ToString();
                    ReferenceLine.Add(go);
                }
            }
            if (DataImageParent.gameObject.activeInHierarchy)
                for (int i = 0; i < ReferenceValue.Length; i++)
                {
                    float value = ValueAdjust.ToPercent01(ReferenceValue[i], DataRange.x, DataRange.y) * DataImageParent.sizeDelta.y;
                    ReferenceLine[i].offsetMax = new Vector2(DataImageParent.sizeDelta.x, value + 1);
                    ReferenceLine[i].offsetMin = new Vector2(0, value - 1);
                    Color newColor = new HSVColor(360f * i / ReferenceValue.Length, 1, 1, 1).ToColor();
                    if (ReferenceLine[i].GetComponent<Image>())
                        ReferenceLine[i].GetComponent<Image>().color = newColor;
                }
        }
        /// <summary>
        /// 移除多余记录
        /// </summary>
        void ClampDataCount()
        {
            if (RecordData.Count > MaxRecord)
            {
                RecordData.RemoveRange(0, RecordData.Count - MaxRecord);
            }
        }
        /// <summary>
        /// 清空绘制
        /// </summary>
        public void Clear()
        {
            RecordData.Clear();
        }
        /// <summary>
        /// 增加记录
        /// </summary>
        /// <param name="data"></param>
        public void Add(float data)
        {
            if (RecordData == null)
                RecordData = new List<float>();
            RecordData.Add(data);
        }
        /// <summary>
        /// 刷新数据绘制
        /// </summary>
        private void OnDraw()
        {
            if (DataImageParent.gameObject.activeInHierarchy)
                for (int i = 0; i < DataImages.Count; i++)
                {
                    if (RecordData.Count - DataImages.Count + i <= 0)
                    {
                        //DataImages[i].sizeDelta = new Vector2(DataImage_Prefab.sizeDelta.x, 0);
                        DataImages[i].localScale = new Vector3(1, 0, 1);
                    }
                    else
                    {
                        float data = RecordData[RecordData.Count - DataImages.Count + i];
                        //DataImages[i].sizeDelta = new Vector2(DataImage_Prefab.sizeDelta.x, RecordData[RecordData.Count - DataImages.Count + i] * 3);
                        DataImages[i].localScale = new Vector3(1, ValueAdjust.ToPercent01(data, DataRange.x, DataRange.y, 1, false), 1);
                    }
                }
        }
        private void LateUpdate()
        {
            ClampDataCount();
            OnDraw();
        }
    }
}
