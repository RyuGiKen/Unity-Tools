using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RyuGiKen;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 模拟触发器
    /// </summary>
    public class TriggerButtonPointer : MonoBehaviour
    {
        public enum PointerMode
        {
            /// <summary>
            /// 射线
            /// </summary>
            Raycaster,
            /// <summary>
            /// 
            /// </summary>
            Collider2D
        }
        /// <summary>
        /// 触碰计时触发
        /// </summary>
        public bool TriggerByTime;
        public PointerMode Mode;
        public Collider2D collider;
        public GraphicRaycaster graphicRaycaster;
        public Vector2Int ListCount = Vector2Int.zero;
        public bool canTriggerObj = true;
        public TriggerButton TriggerObj;
        public TriggerButton LastClickObj;
        public List<RaycastResult> RaycastList = new List<RaycastResult>();
        public List<Collider2D> ColliderList = new List<Collider2D>();
        protected virtual void Start()
        {
            TriggerObj = LastClickObj = null;
        }
        protected virtual void Reset()
        {
            //graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
            collider = this.GetComponent<Collider2D>();
        }
        protected virtual void Update()
        {
            switch (Mode)
            {
                case PointerMode.Raycaster:
                    collider?.SetEnable(false);
                    CheckRaycaster();
                    break;
                case PointerMode.Collider2D:
                    collider?.SetEnable(true);
                    CheckCollider();
                    break;
            }
        }
        public virtual void OnClick()
        {
            //if (TriggerObj != LastClickObj)
            TriggerObj?.OnClickEvent();
            LastClickObj = TriggerObj;
        }
        public virtual void CheckCollider()
        {
            ListCount.y = ColliderList.Count;
            TriggerButton result = null;
            foreach (Collider2D item in ColliderList)
            {
                if (!item)
                    continue;
                if (result)
                    break;
                TriggerButton ui1 = item.gameObject.transform.GetComponent<TriggerButton>();
                TriggerButton ui2 = item.gameObject.transform.GetComponentInParent<TriggerButton>();
                if (ui1)
                {
                    result = ui1;
                }
                else if (ui2)
                {
                    result = ui2;
                }
            }
            TriggerObj = result;
        }
        public virtual void CheckRaycaster()
        {
            RaycastList = GraphicRaycaster(graphicRaycaster, this.transform.position);
            ListCount.x = RaycastList.Count;
            TriggerButton result = null;
            foreach (RaycastResult item in RaycastList)
            {
                if (result)
                    break;
                TriggerButton ui1 = item.gameObject.transform.GetComponent<TriggerButton>();
                TriggerButton ui2 = item.gameObject.transform.GetComponentInParent<TriggerButton>();
                if (ui1)
                {
                    result = ui1;
                }
                else if (ui2)
                {
                    result = ui2;
                }
            }
            TriggerObj = result;
        }
        public static List<RaycastResult> GraphicRaycaster(GraphicRaycaster graphicRaycaster, Vector2 pos)
        {
            if (!graphicRaycaster)
                return null;
            var mPointerEventData = new PointerEventData(null);
            mPointerEventData.position = pos;
            List<RaycastResult> results = new List<RaycastResult>();

            graphicRaycaster.Raycast(mPointerEventData, results);
            return results;
        }
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            //Debug.Log("Stay " + collision.name);
            if (ColliderList.IndexOf(collision) < 0)
            {
                ColliderList.Add(collision);
            }
        }
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log("Enter " + collision.name);
            if (ColliderList.IndexOf(collision) < 0)
            {
                ColliderList.Add(collision);
            }
        }
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            //Debug.Log("Exit " + collision.name);
            ColliderList.Remove(collision);
        }
    }
}
