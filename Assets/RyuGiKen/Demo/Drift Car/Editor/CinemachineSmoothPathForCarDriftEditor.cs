using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditorInternal;
using RyuGiKen;
using RyuGiKen.Tools;
using Cinemachine;
using Cinemachine.Editor;
namespace RyuGiKen.DriftCar
{
    [CustomEditor(typeof(CinemachineSmoothPathForCarDrift))]
    internal sealed class CinemachineSmoothPathForCarDriftEditor : BaseEditor<CinemachineSmoothPathForCarDrift>
    {
        private ReorderableList mWaypointList;

        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);
            excluded.Add(FieldPath(x => x.m_Waypoints));
        }

        void OnEnable()
        {
            mWaypointList = null;
        }


        // ReSharper disable once UnusedMember.Global - magic method called when doing Frame Selected
        public bool HasFrameBounds()
        {
            return Target.m_Waypoints != null && Target.m_Waypoints.Length > 0;
        }

        // ReSharper disable once UnusedMember.Global - magic method called when doing Frame Selected
        public Bounds OnGetFrameBounds()
        {
            Vector3[] wp;
            int selected = mWaypointList == null ? -1 : mWaypointList.index;
            if (selected >= 0 && selected < Target.m_Waypoints.Length)
                wp = new Vector3[1] { Target.m_Waypoints[selected].position };
            else
                wp = Target.m_Waypoints.Select(p => p.position).ToArray();
            return GeometryUtility.CalculateBounds(wp, Target.transform.localToWorldMatrix);
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (mWaypointList == null)
                SetupWaypointList();

            if (mWaypointList.index >= mWaypointList.count)
                mWaypointList.index = mWaypointList.count - 1;

            // Ordinary properties
            DrawRemainingPropertiesInInspector();

            // Path length
            EditorGUILayout.LabelField("曲线长度", Target.PathLength.ToString());

            // Waypoints
            EditorGUI.BeginChangeCheck();
            mWaypointList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        void SetupWaypointList()
        {
            mWaypointList = new ReorderableList(
                    serializedObject, FindProperty(x => x.m_Waypoints),
                    true, true, true, true);

            mWaypointList.drawHeaderCallback = (Rect rect) =>
            { EditorGUI.LabelField(rect, "路径点"); };
            mWaypointList.elementHeight = 45;
            mWaypointList.drawElementCallback
                = (Rect rect, int index, bool isActive, bool isFocused) =>
                { DrawWaypointEditor(rect, index); };

            mWaypointList.onAddCallback = (ReorderableList l) =>
            { InsertWaypointAtIndex(l.index); };
        }

        void DrawWaypointEditor(Rect rect, int index)
        {
            // Needed for accessing string names of fields
            CinemachineSmoothPathForCarDrift.Waypoint def = new CinemachineSmoothPathForCarDrift.Waypoint();
            SerializedProperty element = mWaypointList.serializedProperty.GetArrayElementAtIndex(index);

            float hSpace = 3;
            rect.width -= hSpace; rect.y += 1;
            Vector2 numberDimension = GUI.skin.label.CalcSize(new GUIContent("999"));
            Rect r = new Rect(rect.position, numberDimension);
            if (GUI.Button(r, new GUIContent(index.ToString(), "Go to the waypoint in the scene view")))
            {
                if (SceneView.lastActiveSceneView != null)
                {
                    mWaypointList.index = index;
                    SceneView.lastActiveSceneView.pivot = Target.EvaluatePosition(index);
                    SceneView.lastActiveSceneView.size = 4;
                    SceneView.lastActiveSceneView.Repaint();
                }
            }

            float floatFieldWidth = EditorGUIUtility.singleLineHeight * 2f;
            GUIContent rollLabel = new GUIContent("翻滚");
            GUIContent yawLabel = new GUIContent("偏航");
            GUIContent widthLabel = new GUIContent("宽度");
            GUIContent speedLabel = new GUIContent("时速");
            Vector2 labelDimension = GUI.skin.label.CalcSize(rollLabel);
            float rollWidth = labelDimension.x + floatFieldWidth;
            r.x += r.width + hSpace; r.width = rect.width - (r.width + hSpace + rollWidth) - (r.height + hSpace);
            EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.position), GUIContent.none);//位置

            r.x += r.width + hSpace; r.width = rollWidth;
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelDimension.x;

            var indent = EditorGUI.indentLevel;
            //EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.roll), rollLabel);
            //EditorGUIUtility.labelWidth = oldWidth;
            //EditorGUI.indentLevel = indent;

            r.y += EditorGUIUtility.singleLineHeight + hSpace;
            EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.yaw), yawLabel);

            r.x = rect.x + numberDimension.x * 2; r.width = rollWidth + labelDimension.x;
            EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.speed), speedLabel);

            r.x += r.width + hSpace * 2;
            EditorGUI.PropertyField(r, element.FindPropertyRelative(() => def.width), widthLabel);
        }
        /// <summary>
        /// 插入路径点
        /// </summary>
        /// <param name="indexA"></param>
        void InsertWaypointAtIndex(int indexA)
        {
            Vector3 pos = Vector3.right;
            float roll = 0;

            // Get new values from the current indexA (if any)
            int numWaypoints = Target.m_Waypoints.Length;
            if (indexA < 0)
                indexA = numWaypoints - 1;
            if (indexA >= 0)
            {
                int indexB = indexA + 1;
                if (Target.m_Looped && indexB >= numWaypoints)
                    indexB = 0;
                if (indexB >= numWaypoints)
                {
                    Vector3 delta = Vector3.right;
                    if (indexA > 0)
                        delta = Target.m_Waypoints[indexA].position - Target.m_Waypoints[indexA - 1].position;
                    pos = Target.m_Waypoints[indexA].position + delta;
                    roll = Target.m_Waypoints[indexA].roll;
                }
                else
                {
                    // Interpolate
                    pos = Target.transform.InverseTransformPoint(Target.EvaluatePosition(0.5f + indexA));
                    roll = Mathf.Lerp(Target.m_Waypoints[indexA].roll, Target.m_Waypoints[indexB].roll, 0.5f);
                }
            }
            Undo.RecordObject(Target, "增加路径点");
            var wp = new CinemachineSmoothPathForCarDrift.Waypoint();
            wp.position = pos;
            wp.roll = roll;
            var list = new List<CinemachineSmoothPathForCarDrift.Waypoint>(Target.m_Waypoints);
            list.Insert(indexA + 1, wp);
            Target.m_Waypoints = list.ToArray();
            Target.InvalidateDistanceCache();
            InspectorUtility.RepaintGameView();
            mWaypointList.index = indexA + 1; // select it
        }

        void OnSceneGUI()
        {
            if (mWaypointList == null)
                SetupWaypointList();

            if (UnityEditor.Tools.current == Tool.Move)
            {
                Color colorOld = Handles.color;
                var localToWorld = Target.transform.localToWorldMatrix;
                for (int i = 0; i < Target.m_Waypoints.Length; ++i)
                {
                    DrawSelectionHandle(i, localToWorld);
                    if (mWaypointList.index == i)
                        DrawPositionControl(i, localToWorld, Target.transform.rotation); // Waypoint is selected
                }
                Handles.color = colorOld;
            }
        }
        /// <summary>
        /// 绘制选中路径点
        /// </summary>
        /// <param name="i"></param>
        /// <param name="localToWorld"></param>
        void DrawSelectionHandle(int i, Matrix4x4 localToWorld)
        {
            if (Event.current.button != 1)
            {
                Vector3 pos = localToWorld.MultiplyPoint(Target.m_Waypoints[i].position);
                float size = HandleUtility.GetHandleSize(pos) * 0.2f;
                Handles.color = Color.white;
                if (Handles.Button(pos, Quaternion.identity, size, size, Handles.SphereHandleCap)
                    && mWaypointList.index != i)
                {
                    mWaypointList.index = i;
                    InspectorUtility.RepaintGameView();
                }
                // Label it
                Handles.BeginGUI();
                Vector2 labelSize = new Vector2(
                        EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight);
                Vector2 labelPos = HandleUtility.WorldToGUIPoint(pos);
                labelPos.y -= labelSize.y / 2;
                labelPos.x -= labelSize.x / 2;
                GUILayout.BeginArea(new Rect(labelPos, labelSize));
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                style.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(new GUIContent(i.ToString(), "Waypoint " + i), style);
                GUILayout.EndArea();
                Handles.EndGUI();
            }
        }

        void DrawPositionControl(int i, Matrix4x4 localToWorld, Quaternion localRotation)
        {
            CinemachineSmoothPathForCarDrift.Waypoint wp = Target.m_Waypoints[i];
            Vector3 pos = localToWorld.MultiplyPoint(wp.position);
            EditorGUI.BeginChangeCheck();
            Handles.color = Target.m_Appearance.pathColor;
            Quaternion rotation = (UnityEditor.Tools.pivotRotation == PivotRotation.Local)
                ? localRotation : Quaternion.identity;
            float size = HandleUtility.GetHandleSize(pos) * 0.1f;
            Handles.SphereHandleCap(0, pos, rotation, size, EventType.Repaint);
            pos = Handles.PositionHandle(pos, rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move Waypoint");
                wp.position = Matrix4x4.Inverse(localToWorld).MultiplyPoint(pos);
                Target.m_Waypoints[i] = wp;
                Target.InvalidateDistanceCache();
                InspectorUtility.RepaintGameView();
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
             | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(CinemachineSmoothPathForCarDrift))]
        static void DrawGizmos(CinemachineSmoothPathForCarDrift path, GizmoType selectionType)
        {
            var isActive = Selection.activeGameObject == path.gameObject;
            DrawPathGizmo(path, isActive ? path.m_Appearance.pathColor : path.m_Appearance.inactivePathColor, isActive);
        }
        public static void DrawPathGizmo(CinemachineSmoothPathForCarDrift path, Color pathColor, bool isActive)
        {
            // Draw the path
            Color colorOld = Gizmos.color;
            Gizmos.color = pathColor;
            float step = 1f / path.m_Resolution;
            float halfWidth = path.EvaluateWidth(path.MinPos);//path.m_Appearance.width * 0.5f;
            Vector3 lastPos = path.EvaluatePosition(path.MinPos);
            Vector3 lastW = (path.EvaluateOrientation(path.MinPos) * Vector3.right) * halfWidth;
            float tEnd = path.MaxPos + step / 2;
            for (float t = path.MinPos + step; t <= tEnd; t += step)
            {
                Vector3 p = path.EvaluatePosition(t);
                halfWidth = path.EvaluateWidth(t);
                if (!isActive || halfWidth == 0)
                {
                    Gizmos.DrawLine(p, lastPos);
                }
                else
                {
                    Quaternion q = path.EvaluateOrientation(t);
                    Vector3 w = (q * Vector3.right) * halfWidth;
                    Vector3 w2 = w * 1.2f;
                    Vector3 p0 = p - w2;
                    Vector3 p1 = p + w2;
                    Gizmos.color = ColorAdjust.ColorValueChange(pathColor, 0.5f);
                    Gizmos.DrawLine(p0, p1);
                    Gizmos.color = pathColor;
                    Gizmos.DrawLine(lastPos - lastW, p - w);
                    Gizmos.DrawLine(lastPos + lastW, p + w);
#if false
                    // Show the normals, for debugging
                    Gizmos.color = Color.red;
                    Vector3 y = (q * Vector3.up) * halfWidth;
                    Gizmos.DrawLine(p, p + y);
                    Gizmos.color = pathColor;
#endif
                    lastW = w;
                }

                lastPos = p;
            }
            Gizmos.color = colorOld;
        }
    }
}
