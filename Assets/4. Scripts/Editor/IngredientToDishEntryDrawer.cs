using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IngredientToDishEntry))]
public class IngredientToDishEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var ingredientDataRect = new Rect(position.x, position.y, position.width / 2, position.height);
        var foodDataRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);

        EditorGUI.PropertyField(ingredientDataRect, property.FindPropertyRelative("ingredientData"), GUIContent.none);
        EditorGUI.PropertyField(foodDataRect, property.FindPropertyRelative("foodData"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
