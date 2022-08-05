using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 网格布局比例尺寸
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridLayoutGroupPercentSize : MonoBehaviour
    {
        [Range(0, 1)] public float percentX = 0.5f;
        [Range(0, 1)] public float percentY = 0.5f;
        [SerializeField] Vector2Int Count;
        internal GridLayoutGroup gridLayoutGroup;
        private void LateUpdate()
        {
            if (!gridLayoutGroup)
                gridLayoutGroup = this.GetComponent<GridLayoutGroup>();

            if (gridLayoutGroup)
            {
                Vector2 size = gridLayoutGroup.GetComponent<RectTransform>().rect.size;
                Vector2 cellSize = gridLayoutGroup.cellSize;
                int ChildCount = gridLayoutGroup.transform.childCount;
                Count = Vector2Int.zero;
                RectOffset rectOffset = gridLayoutGroup.padding;
                switch (gridLayoutGroup.constraint)
                {
                    case GridLayoutGroup.Constraint.Flexible:
                        Count.x = Mathf.FloorToInt((size.x - rectOffset.horizontal) / cellSize.x);
                        Count.y = Mathf.CeilToInt(ChildCount * 1f / Count.x);

                        if (percentX > 0)
                            cellSize.x = (size.x - rectOffset.horizontal) * percentX;
                        if (percentY > 0)
                            cellSize.y = (size.y - rectOffset.vertical) * percentY;
                        break;
                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        Count.x = gridLayoutGroup.constraintCount;
                        Count.y = Mathf.CeilToInt(ChildCount * 1f / Count.x);

                        if (percentX > 0)
                            cellSize.x = (size.x - gridLayoutGroup.spacing.x * (gridLayoutGroup.constraintCount - 1) - rectOffset.horizontal) * percentX;
                        if (percentY > 0)
                            cellSize.y = (size.y - rectOffset.vertical) * percentY;
                        break;
                    case GridLayoutGroup.Constraint.FixedRowCount:
                        Count.y = gridLayoutGroup.constraintCount;
                        Count.x = Mathf.CeilToInt(ChildCount * 1f / Count.y);

                        if (percentX > 0)
                            cellSize.x = (size.x - rectOffset.horizontal) * percentX;
                        if (percentY > 0)
                            cellSize.y = (size.y - gridLayoutGroup.spacing.y * (gridLayoutGroup.constraintCount - 1) - rectOffset.vertical) * percentY;
                        break;
                }
                if (!Mathf.Approximately(cellSize.x, gridLayoutGroup.cellSize.x) || !Mathf.Approximately(cellSize.y, gridLayoutGroup.cellSize.y))
                    gridLayoutGroup.cellSize = cellSize;
            }
        }
    }
}