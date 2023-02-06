using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RyuGiKen;
using RyuGiKen.Tools;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 实时显示Transform属性
    /// </summary>
    [AddComponentMenu("RyuGiKen/实时显示Transform属性")]
    [RequireComponent(typeof(Transform))]
    [DisallowMultipleComponent]
    public class ShowTransfromData : MonoBehaviour
    {
        [SerializeField] Transform _Transform;
        public Transform m_Transform
        {
            get
            {
                Reset();
                return _Transform;
            }
        }
        private void Awake()
        {
            Reset();
        }
        private void Reset()
        {
            if (!_Transform)
                _Transform = this.GetComponent<Transform>();
        }
    }
}
namespace RyuGiKenEditor.Tools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ShowTransfromData))]
    public class ShowTransfromDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Transform transform = (target as ShowTransfromData).transform;

            if (transform)
            {
                EditorGUILayout.Vector3Field("Posiition", transform.position);
                EditorGUILayout.Vector3Field("localPosition", transform.localPosition);
                EditorGUILayout.Vector3Field("eulerAngles", transform.eulerAngles);
                EditorGUILayout.Vector3Field("localEulerAngles", transform.localEulerAngles);
                EditorGUILayout.Vector4Field("rotation", transform.rotation.ToVector4());
                EditorGUILayout.Vector4Field("localRotation", transform.localRotation.ToVector4());
                EditorGUILayout.Vector3Field("lossyScale", transform.lossyScale);
                EditorGUILayout.Vector3Field("localScale", transform.localScale);
                EditorGUILayout.Space();
                EditorGUILayout.Vector3Field("up", transform.up);
                EditorGUILayout.Vector3Field("right", transform.right);
                EditorGUILayout.Vector3Field("forward", transform.forward);
                if (transform is RectTransform)
                {
                    RectTransform rectTransform = transform as RectTransform;
                    EditorGUILayout.Space();
                    EditorGUILayout.Vector2Field("anchoredPosition", rectTransform.anchoredPosition);
                    EditorGUILayout.Vector3Field("anchoredPosition3D", rectTransform.anchoredPosition3D);
                    EditorGUILayout.Vector2Field("offsetMin", rectTransform.offsetMin);
                    EditorGUILayout.Vector2Field("offsetMax", rectTransform.offsetMax);
                    EditorGUILayout.Vector2Field("sizeDelta", rectTransform.sizeDelta);
                    EditorGUILayout.Vector2Field("anchorMin", rectTransform.anchorMin);
                    EditorGUILayout.Vector2Field("anchorMax", rectTransform.anchorMax);
                    //EditorGUILayout.Vector4Field("rect：", new Vector4(rectTransform.rect.x, rectTransform.rect.y, rectTransform.rect.width, rectTransform.rect.height));
                    EditorGUILayout.RectField("rect：", rectTransform.rect);
                }
                if (transform.TryGetComponent(out Collider collider))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Vector3Field("bounds.min", collider.bounds.min);
                    EditorGUILayout.Vector3Field("bounds.max", collider.bounds.max);
                }
                else if (transform.TryGetComponent(out Collider2D collider2D))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Vector3Field("bounds.min", collider2D.bounds.min);
                    EditorGUILayout.Vector3Field("bounds.max", collider2D.bounds.max);
                }
            }
        }
    }
#endif
}
