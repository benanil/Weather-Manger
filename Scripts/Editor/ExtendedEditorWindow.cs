using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Serializer
{
    public class ExtendedEditorWindow : EditorWindow
    {
        protected SerializedObject serializedObject;
        protected SerializedProperty currentProperty;

        private string sellectedPropertyPath;
        protected SerializedProperty sellectedProperty;

        protected static void DrawProperties(SerializedProperty prop,bool drawChildren)
        {
            string lastPropPath = string.Empty;

            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) continue;
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }

        protected void DrawSideBar(SerializedProperty prop)
        {
            
            foreach (SerializedProperty p in prop)
            {
                if (GUILayout.Button(p.displayName))
                {
                    sellectedPropertyPath = p.propertyPath;
                }
            }

            if (!string.IsNullOrEmpty(sellectedPropertyPath))
            {
                sellectedProperty = serializedObject.FindProperty(sellectedPropertyPath);
            }

        }

    }
}
