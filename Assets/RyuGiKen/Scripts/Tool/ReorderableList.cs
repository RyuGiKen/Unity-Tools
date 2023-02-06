using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR || UNITY_STANDALONE
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using RyuGiKen;
namespace RyuGiKen
{
    [System.Serializable]
    public abstract class ReorderableListBase
    {
        //public bool Draggable = true;
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
        public ReorderableList(params T[] items)
        {
            this.items = items.ToList();
        }
        public ReorderableList(int length)
        {
            if (length > 0)
                this.items = new T[length].ToList();
            else
                this.items = new List<T>();
        }
        public ReorderableList(uint length)
        {
            this.items = new T[(int)length].ToList();
        }
        //public ReorderableList(T[] array)
        //{
        //    this.items = array != null ? array.ToList() : new List<T>();
        //}
        public ReorderableList(List<T> list)
        {
            this.items = list;
        }
    }
    public static partial class ReorderableListExtension
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
        /// <summary>
        /// 获取随机项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="index1"></param>
        /// <returns></returns>
        public static T GetRandomOne<T>(this List<ReorderableList<T>> items, int index1)
        {
            try
            {
                return items[index1].items.GetRandomItem();
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// 获取项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public static T GetItem<T>(this List<ReorderableList<T>> items, int index1, int index2)
        {
            try
            {
                return items[index1].items[index2];
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// 获取总长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int GetLength<T>(this List<ReorderableList<T>> items)
        {
            if (items == null)
                return 0;
            int result = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i])
                    result += items[i].Count;
            }
            return result;
        }
    }
    [System.Serializable]
    public class ReorderableListBoolean : ReorderableList<bool>
    {
        public ReorderableListBoolean() { this.items = new List<bool>(); }
        public ReorderableListBoolean(bool[] array) { this.items = array != null ? array.ToList() : new List<bool>(); }
        public ReorderableListBoolean(List<bool> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListInteger : ReorderableListInteger32
    {
        public ReorderableListInteger() { this.items = new List<int>(); }
        public ReorderableListInteger(int[] array) { this.items = array != null ? array.ToList() : new List<int>(); }
        public ReorderableListInteger(List<int> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListInteger16 : ReorderableList<short>
    {
        public ReorderableListInteger16() { this.items = new List<short>(); }
        public ReorderableListInteger16(short[] array) { this.items = array != null ? array.ToList() : new List<short>(); }
        public ReorderableListInteger16(List<short> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListInteger32 : ReorderableList<int>
    {
        public ReorderableListInteger32() { this.items = new List<int>(); }
        public ReorderableListInteger32(int[] array) { this.items = array != null ? array.ToList() : new List<int>(); }
        public ReorderableListInteger32(List<int> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListInteger64 : ReorderableList<long>
    {
        public ReorderableListInteger64() { this.items = new List<long>(); }
        public ReorderableListInteger64(long[] array) { this.items = array != null ? array.ToList() : new List<long>(); }
        public ReorderableListInteger64(List<long> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListUInteger : ReorderableList<uint>
    {
        public ReorderableListUInteger() { this.items = new List<uint>(); }
        public ReorderableListUInteger(uint[] array) { this.items = array != null ? array.ToList() : new List<uint>(); }
        public ReorderableListUInteger(List<uint> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListFloat : ReorderableList<float>
    {
        public ReorderableListFloat() { this.items = new List<float>(); }
        public ReorderableListFloat(float[] array) { this.items = array != null ? array.ToList() : new List<float>(); }
        public ReorderableListFloat(List<float> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListDouble : ReorderableList<double>
    {
        public ReorderableListDouble() { this.items = new List<double>(); }
        public ReorderableListDouble(double[] array) { this.items = array != null ? array.ToList() : new List<double>(); }
        public ReorderableListDouble(List<double> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListDecimal : ReorderableList<decimal>
    {
        public ReorderableListDecimal() { this.items = new List<decimal>(); }
        public ReorderableListDecimal(decimal[] array) { this.items = array != null ? array.ToList() : new List<decimal>(); }
        public ReorderableListDecimal(List<decimal> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListString : ReorderableList<string>
    {
        public ReorderableListString() { this.items = new List<string>(); }
        public ReorderableListString(string[] array) { this.items = array != null ? array.ToList() : new List<string>(); }
        public ReorderableListString(List<string> list) { this.items = list; }
    }
#if UNITY_EDITOR || UNITY_STANDALONE
    [System.Serializable]
    public class ReorderableListVector2 : ReorderableList<Vector2>
    {
        public ReorderableListVector2() { this.items = new List<Vector2>(); }
        public ReorderableListVector2(Vector2[] array) { this.items = array != null ? array.ToList() : new List<Vector2>(); }
        public ReorderableListVector2(List<Vector2> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListVector2Int : ReorderableList<Vector2Int>
    {
        public ReorderableListVector2Int() { this.items = new List<Vector2Int>(); }
        public ReorderableListVector2Int(Vector2Int[] array) { this.items = array != null ? array.ToList() : new List<Vector2Int>(); }
        public ReorderableListVector2Int(List<Vector2Int> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListVector3 : ReorderableList<Vector3>
    {
        public ReorderableListVector3() { this.items = new List<Vector3>(); }
        public ReorderableListVector3(Vector3[] array) { this.items = array != null ? array.ToList() : new List<Vector3>(); }
        public ReorderableListVector3(List<Vector3> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListVector3Int : ReorderableList<Vector3Int>
    {
        public ReorderableListVector3Int() { this.items = new List<Vector3Int>(); }
        public ReorderableListVector3Int(Vector3Int[] array) { this.items = array != null ? array.ToList() : new List<Vector3Int>(); }
        public ReorderableListVector3Int(List<Vector3Int> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListVector4 : ReorderableList<Vector4>
    {
        public ReorderableListVector4() { this.items = new List<Vector4>(); }
        public ReorderableListVector4(Vector4[] array) { this.items = array != null ? array.ToList() : new List<Vector4>(); }
        public ReorderableListVector4(List<Vector4> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListQuaternion : ReorderableList<Quaternion>
    {
        public ReorderableListQuaternion() { this.items = new List<Quaternion>(); }
        public ReorderableListQuaternion(Quaternion[] array) { this.items = array != null ? array.ToList() : new List<Quaternion>(); }
        public ReorderableListQuaternion(List<Quaternion> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListColor : ReorderableList<Color>
    {
        public ReorderableListColor() { this.items = new List<Color>(); }
        public ReorderableListColor(Color[] array) { this.items = array != null ? array.ToList() : new List<Color>(); }
        public ReorderableListColor(List<Color> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListColor32 : ReorderableList<Color32>
    {
        public ReorderableListColor32() { this.items = new List<Color32>(); }
        public ReorderableListColor32(Color32[] array) { this.items = array != null ? array.ToList() : new List<Color32>(); }
        public ReorderableListColor32(List<Color32> list) { this.items = list; }
    }
#endif
    [System.Serializable]
    public class ReorderableListHSVColor : ReorderableList<HSVColor>
    {
        public ReorderableListHSVColor() { this.items = new List<HSVColor>(); }
        public ReorderableListHSVColor(HSVColor[] array) { this.items = array != null ? array.ToList() : new List<HSVColor>(); }
        public ReorderableListHSVColor(List<HSVColor> list) { this.items = list; }
    }
#if UNITY_EDITOR || UNITY_STANDALONE
    [System.Serializable]
    public class ReorderableListObject : ReorderableList<Object>
    {
        public ReorderableListObject() { this.items = new List<Object>(); }
        public ReorderableListObject(Object[] array) { this.items = array != null ? array.ToList() : new List<Object>(); }
        public ReorderableListObject(List<Object> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListScriptableObject : ReorderableList<ScriptableObject>
    {
        public ReorderableListScriptableObject() { this.items = new List<ScriptableObject>(); }
        public ReorderableListScriptableObject(ScriptableObject[] array) { this.items = array != null ? array.ToList() : new List<ScriptableObject>(); }
        public ReorderableListScriptableObject(List<ScriptableObject> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListGameObject : ReorderableList<GameObject>
    {
        public ReorderableListGameObject() { this.items = new List<GameObject>(); }
        public ReorderableListGameObject(GameObject[] array) { this.items = array != null ? array.ToList() : new List<GameObject>(); }
        public ReorderableListGameObject(List<GameObject> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListComponent : ReorderableList<Component>
    {
        public ReorderableListComponent() { this.items = new List<Component>(); }
        public ReorderableListComponent(Component[] array) { this.items = array != null ? array.ToList() : new List<Component>(); }
        public ReorderableListComponent(List<Component> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListTransform : ReorderableList<Transform>
    {
        public ReorderableListTransform() { this.items = new List<Transform>(); }
        public ReorderableListTransform(Transform[] array) { this.items = array != null ? array.ToList() : new List<Transform>(); }
        public ReorderableListTransform(List<Transform> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListRectTransform : ReorderableList<RectTransform>
    {
        public ReorderableListRectTransform() { this.items = new List<RectTransform>(); }
        public ReorderableListRectTransform(RectTransform[] array) { this.items = array != null ? array.ToList() : new List<RectTransform>(); }
        public ReorderableListRectTransform(List<RectTransform> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListBehaviour : ReorderableList<Behaviour>
    {
        public ReorderableListBehaviour() { this.items = new List<Behaviour>(); }
        public ReorderableListBehaviour(Behaviour[] array) { this.items = array != null ? array.ToList() : new List<Behaviour>(); }
        public ReorderableListBehaviour(List<Behaviour> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListMonoBehaviour : ReorderableList<MonoBehaviour>
    {
        public ReorderableListMonoBehaviour() { this.items = new List<MonoBehaviour>(); }
        public ReorderableListMonoBehaviour(MonoBehaviour[] array) { this.items = array != null ? array.ToList() : new List<MonoBehaviour>(); }
        public ReorderableListMonoBehaviour(List<MonoBehaviour> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListGraphic : ReorderableList<Graphic>
    {
        public ReorderableListGraphic() { this.items = new List<Graphic>(); }
        public ReorderableListGraphic(Graphic[] array) { this.items = array != null ? array.ToList() : new List<Graphic>(); }
        public ReorderableListGraphic(List<Graphic> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListText : ReorderableList<Text>
    {
        public ReorderableListText() { this.items = new List<Text>(); }
        public ReorderableListText(Text[] array) { this.items = array != null ? array.ToList() : new List<Text>(); }
        public ReorderableListText(List<Text> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListImage : ReorderableList<Image>
    {
        public ReorderableListImage() { this.items = new List<Image>(); }
        public ReorderableListImage(Image[] array) { this.items = array != null ? array.ToList() : new List<Image>(); }
        public ReorderableListImage(List<Image> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListRenderer : ReorderableList<Renderer>
    {
        public ReorderableListRenderer() { this.items = new List<Renderer>(); }
        public ReorderableListRenderer(Renderer[] array) { this.items = array != null ? array.ToList() : new List<Renderer>(); }
        public ReorderableListRenderer(List<Renderer> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListCamera : ReorderableList<Camera>
    {
        public ReorderableListCamera() { this.items = new List<Camera>(); }
        public ReorderableListCamera(Camera[] array) { this.items = array != null ? array.ToList() : new List<Camera>(); }
        public ReorderableListCamera(List<Camera> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListCollider : ReorderableList<Collider>
    {
        public ReorderableListCollider() { this.items = new List<Collider>(); }
        public ReorderableListCollider(Collider[] array) { this.items = array != null ? array.ToList() : new List<Collider>(); }
        public ReorderableListCollider(List<Collider> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListCollider2D : ReorderableList<Collider2D>
    {
        public ReorderableListCollider2D() { this.items = new List<Collider2D>(); }
        public ReorderableListCollider2D(Collider2D[] array) { this.items = array != null ? array.ToList() : new List<Collider2D>(); }
        public ReorderableListCollider2D(List<Collider2D> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListTexture : ReorderableList<Texture>
    {
        public ReorderableListTexture() { this.items = new List<Texture>(); }
        public ReorderableListTexture(Texture[] array) { this.items = array != null ? array.ToList() : new List<Texture>(); }
        public ReorderableListTexture(List<Texture> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListSprite : ReorderableList<Sprite>
    {
        public ReorderableListSprite() { this.items = new List<Sprite>(); }
        public ReorderableListSprite(Sprite[] array) { this.items = array != null ? array.ToList() : new List<Sprite>(); }
        public ReorderableListSprite(List<Sprite> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListAudioClip : ReorderableList<AudioClip>
    {
        public ReorderableListAudioClip() { this.items = new List<AudioClip>(); }
        public ReorderableListAudioClip(AudioClip[] array) { this.items = array != null ? array.ToList() : new List<AudioClip>(); }
        public ReorderableListAudioClip(List<AudioClip> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListMaterial : ReorderableList<Material>
    {
        public ReorderableListMaterial() { this.items = new List<Material>(); }
        public ReorderableListMaterial(Material[] array) { this.items = array != null ? array.ToList() : new List<Material>(); }
        public ReorderableListMaterial(List<Material> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListAnimationClip : ReorderableList<AnimationClip>
    {
        public ReorderableListAnimationClip() { this.items = new List<AnimationClip>(); }
        public ReorderableListAnimationClip(AnimationClip[] array) { this.items = array != null ? array.ToList() : new List<AnimationClip>(); }
        public ReorderableListAnimationClip(List<AnimationClip> list) { this.items = list; }
    }
#endif
    [System.Serializable]
    public class ReorderableListValueRange : ReorderableList<ValueRange>
    {
        public ReorderableListValueRange() { this.items = new List<ValueRange>(); }
        public ReorderableListValueRange(ValueRange[] array) { this.items = array != null ? array.ToList() : new List<ValueRange>(); }
        public ReorderableListValueRange(List<ValueRange> list) { this.items = list; }
    }
    [System.Serializable]
    public class ReorderableListValueInRange : ReorderableList<ValueInRange>
    {
        public ReorderableListValueInRange() { this.items = new List<ValueInRange>(); }
        public ReorderableListValueInRange(ValueInRange[] array) { this.items = array != null ? array.ToList() : new List<ValueInRange>(); }
        public ReorderableListValueInRange(List<ValueInRange> list) { this.items = list; }
    }
}
namespace RyuGiKenEditor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReorderableListBase), true)]
    public class ReorderableListPropertyDrawer : PropertyDrawer
    {
        protected const int minSpacing = 2;
        protected Dictionary<Tuple<SerializedObject, string>, ReorderableList> dictionary;
        protected virtual ReorderableList CreateReorderableList(SerializedProperty property, SerializedProperty listProperty)
        {
            return CreateReorderableList(property, listProperty, true);
        }
        protected virtual ReorderableList CreateReorderableList(SerializedProperty property, SerializedProperty listProperty, bool canEdit)
        {
            return new ReorderableList(property.serializedObject, listProperty, canEdit, true, canEdit, canEdit);
        }
        protected ReorderableList GetReorderableList(SerializedProperty property)
        {
            var listProperty = property.FindPropertyRelative("items");

            if (dictionary == null)
                dictionary = new Dictionary<Tuple<SerializedObject, string>, ReorderableList>();

            ReorderableList list = null;
            Tuple<SerializedObject, string> key = new Tuple<SerializedObject, string>(property.serializedObject, property.propertyPath + property.displayName);
            //dictionary.TryGetValue(property, out list);
            list = dictionary.GetValueInDictionary(key);
            if (list == null)
            {
                list = CreateReorderableList(property, listProperty);
                dictionary.Add(key, list);

                list.drawHeaderCallback = delegate (Rect rect)
                {
                    EditorGUI.LabelField(rect, property.displayName);
                };
                list.drawElementCallback = delegate (Rect rect, int index, bool isActive, bool isFocused)
                {
                    DrawListItems(rect, listProperty.GetArrayElementAtIndex(index));
                };
                list.onAddCallback = (ReorderableList List) =>
                {
                    if (List != null && List.count > 1)
                        InsertItemAtIndex(List, List.index);
                    else
                        ReorderableList.defaultBehaviours.DoAddButton(List);
                };
                //list.onChangedCallback = (ReorderableList List) =>
                //{
                //    Debug.Log("Changed");
                //};
                //list.onRemoveCallback = (ReorderableList List) =>
                //{
                //    //ReorderableList.defaultBehaviours.DoRemoveButton(List);
                //    try
                //    {
                //        List.serializedProperty.DeleteArrayElementAtIndex(List.index);
                //    }
                //    catch { }
                //};
                //list.onSelectCallback += (ReorderableList List) =>
                //{
                //    Debug.Log("Select " + list.index + " " + dictionary.Count);
                //};
            }
            return list;
        }
        protected virtual void InsertItemAtIndex(ReorderableList list, int index)
        {
            list.serializedProperty.InsertArrayElementAtIndex(index);
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
