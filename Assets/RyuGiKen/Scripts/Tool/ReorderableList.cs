using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
namespace RyuGiKen
{
    [Serializable]
    public class ReorderableListBase
    {
        public bool Draggable = false;
        public static implicit operator bool(ReorderableListBase exists) { return exists != null; }
    }
    public class ReorderableList<T> : ReorderableListBase
    {
        public List<T> items;
        public int Count { get { return items != null ? items.Count : 0; } }
        public T this[int index]
        {
            get
            {
                try
                {
                    return items[index];
                }
                catch
                {
                    return default(T);
                }
            }
        }
        public ReorderableList()
        {
            this.items = new List<T>();
        }
        public ReorderableList(T[] array)
        {
            this.items = array.ToList();
        }
        public ReorderableList(List<T> list)
        {
            this.items = list;
        }
    }
    public static partial class Extension
    {
        /// <summary>
        /// 转数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reorderableList"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this ReorderableList<T> reorderableList)
        {
            return reorderableList ? reorderableList.items.ToArray() : null;
        }
        /// <summary>
        /// 转列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reorderableList"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this ReorderableList<T> reorderableList)
        {
            return reorderableList ? reorderableList.items : null;
        }
    }
    [Serializable] public class ReorderableListBoolean : ReorderableList<bool> { }
    [Serializable] public class ReorderableListInteger : ReorderableListInteger32 { }
    [Serializable] public class ReorderableListInteger16 : ReorderableList<short> { }
    [Serializable] public class ReorderableListInteger32 : ReorderableList<int> { }
    [Serializable] public class ReorderableListInteger64 : ReorderableList<long> { }
    [Serializable] public class ReorderableListUInteger : ReorderableList<uint> { }
    [Serializable] public class ReorderableListFloat : ReorderableList<float> { }
    [Serializable] public class ReorderableListDouble : ReorderableList<double> { }
    [Serializable] public class ReorderableListDecimal : ReorderableList<decimal> { }
    [Serializable] public class ReorderableListString : ReorderableList<string> { }
#if UNITY_EDITOR || UNITY_STANDALONE
    [Serializable] public class ReorderableListVector2 : ReorderableList<Vector2> { }
    [Serializable] public class ReorderableListVector2Int : ReorderableList<Vector2Int> { }
    [Serializable] public class ReorderableListVector3 : ReorderableList<Vector3> { }
    [Serializable] public class ReorderableListVector3Int : ReorderableList<Vector3Int> { }
    [Serializable] public class ReorderableListVector4 : ReorderableList<Vector4> { }
    [Serializable] public class ReorderableListQuaternion : ReorderableList<Quaternion> { }
    [Serializable] public class ReorderableListColor : ReorderableList<Color> { }
    [Serializable] public class ReorderableListColor32 : ReorderableList<Color32> { }
#endif
    [Serializable] public class ReorderableListHSVColor : ReorderableList<HSVColor> { }
#if UNITY_EDITOR || UNITY_STANDALONE
    [Serializable] public class ReorderableListObject : ReorderableList<UnityEngine.Object> { }
    [Serializable] public class ReorderableListScriptableObject : ReorderableList<ScriptableObject> { }
    [Serializable] public class ReorderableListGameObject : ReorderableList<GameObject> { }
    [Serializable] public class ReorderableListComponent : ReorderableList<Component> { }
    [Serializable] public class ReorderableListTransform : ReorderableList<Transform> { }
    [Serializable] public class ReorderableListBehaviour : ReorderableList<Behaviour> { }
    [Serializable] public class ReorderableListMonoBehaviour : ReorderableList<MonoBehaviour> { }
    [Serializable] public class ReorderableListGraphic : ReorderableList<Graphic> { }
    [Serializable] public class ReorderableListText : ReorderableList<Text> { }
    [Serializable] public class ReorderableListImage : ReorderableList<Image> { }
    [Serializable] public class ReorderableListRenderer : ReorderableList<Renderer> { }
    [Serializable] public class ReorderableListCamera : ReorderableList<Camera> { }
#endif
    [Serializable] public class ReorderableListValueInRange : ReorderableList<ValueInRange> { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReorderableListBase), true)]
    public class ReorderableListPropertyDrawer : PropertyDrawer
    {
        protected const int minSpacing = 2;
        //protected ReorderableList list;
        protected virtual ReorderableList CreateReorderableList(SerializedProperty property, SerializedProperty listProperty)
        {
            bool Draggable = property.FindPropertyRelative("Draggable").boolValue;
            return new ReorderableList(property.serializedObject, listProperty, Draggable, true, true, true);
        }
        protected ReorderableList GetReorderableList(SerializedProperty property)
        {
            //if (list == null)
            //{
            var listProperty = property.FindPropertyRelative("items");

            ReorderableList list = CreateReorderableList(property, listProperty);

            list.drawHeaderCallback += delegate (Rect rect)
            {
                EditorGUI.LabelField(rect, property.displayName);
            };
            list.drawElementCallback = delegate (Rect rect, int index, bool isActive, bool isFocused)
            {
                DrawListItems(rect, listProperty.GetArrayElementAtIndex(index));
            };
            //}
            return list;
        }
        protected virtual float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight(property) + minSpacing;
        }
        protected virtual void DrawListItems(Rect rect, SerializedProperty property)
        {
            EditorGUI.PropertyField(rect, property, true);
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetReorderableList(property).GetHeight() + minSpacing;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = GetReorderableList(property);
            var listProperty = property.FindPropertyRelative("items");
            var height = 0f;
            for (var i = 0; i < listProperty.arraySize; i++)
            {
                height = Mathf.Max(height, GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
            }
            list.elementHeight = height;
            list.DoList(position);
            //list.DoLayoutList();
        }
    }
    [CustomPropertyDrawer(typeof(ReorderableListQuaternion), true)]
    public class ReorderableListQuaternionPropertyDrawer : ReorderableListPropertyDrawer
    {
        protected override float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUIUtility.singleLineHeight + minSpacing;
        }
        protected override void DrawListItems(Rect rect, SerializedProperty property)
        {
            Vector4 value = EditorGUI.Vector4Field(rect, GUIContent.none, property.quaternionValue.ToVector4());
            property.quaternionValue = new Quaternion(value.x, value.y, value.z, value.w);
        }
    }
    [CustomPropertyDrawer(typeof(ReorderableListVector4), true)]
    public class ReorderableListVector4PropertyDrawer : ReorderableListPropertyDrawer
    {
        protected override float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUIUtility.singleLineHeight + minSpacing;
        }
        protected override void DrawListItems(Rect rect, SerializedProperty property)
        {
            property.vector4Value = EditorGUI.Vector4Field(rect, GUIContent.none, property.vector4Value);
        }
    }
#endif
}
