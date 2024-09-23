using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ToggleWithValueFloat))]
[CustomPropertyDrawer(typeof(ToggleWithValueInt))]
public class ToggleWithValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get all attributes on the field
        object[] attributes = fieldInfo.GetCustomAttributes(true);

        // Search for the ToggleWithValueLabelAttribute
        ValueLabelAttribute toggleLabelAttribute = null;
        foreach (object attribute in attributes)
        {
            if (attribute is ValueLabelAttribute)
            {
                toggleLabelAttribute = (ValueLabelAttribute)attribute;
                break;
            }
        }

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate the positions for the toggle and value fields
        Rect toggleRect = new Rect(position.x, position.y, position.width * 0.1f, position.height);
        Rect labelRect = new Rect(position.x + position.width * 0.1f, position.y, position.width * 0.3f, position.height);
        Rect valueRect = new Rect(position.x + position.width * 0.35f, position.y, position.width * 0.65f, position.height);

        var toggleProperty = property.FindPropertyRelative("toggle");
        EditorGUI.PropertyField(toggleRect, toggleProperty, GUIContent.none);

        if (toggleProperty.boolValue)
        {
            // Use the custom label text if the attribute is present, otherwise use the default label
            GUIContent toggleLabelContent = toggleLabelAttribute != null ? new GUIContent(toggleLabelAttribute.labelText) : label;
            EditorGUI.LabelField(labelRect, toggleLabelContent);

            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
        }       

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
