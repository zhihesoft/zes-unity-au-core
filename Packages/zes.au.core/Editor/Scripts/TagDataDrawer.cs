using UnityEditor;
using UnityEngine;

namespace Au
{
    [CustomPropertyDrawer(typeof(Tag))]
    public class TagDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float nameWidth = position.width / 3 * 2;
            // Calculate rects
            var amountRect = new Rect(position.x, position.y, nameWidth, position.height);
            var unitRect = new Rect(position.x + nameWidth + 2, position.y,
                position.width - nameWidth, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("target"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
