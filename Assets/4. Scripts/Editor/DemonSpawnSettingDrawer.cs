using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DemonSpawnSetting))]
public class DemonSpawnSettingDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        var demonTypeRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth - 2, EditorGUIUtility.singleLineHeight);
        var content = position.width - EditorGUIUtility.labelWidth;
        var spawnTypeRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, content * 0.7f - 2, EditorGUIUtility.singleLineHeight);
        var spawnChanceRect = new Rect(position.x + EditorGUIUtility.labelWidth +  content * 0.7f, position.y, content * 0.3f, EditorGUIUtility.singleLineHeight);
        var spawnXDemonRect = new Rect(position.x + EditorGUIUtility.labelWidth + content * 0.7f, position.y, content * 0.3f, EditorGUIUtility.singleLineHeight);

        var yOffset = position.y;
        yOffset += EditorGUIUtility.singleLineHeight + 2;
        var maxPerQueueRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
        yOffset += EditorGUIUtility.singleLineHeight + 2;
        var maxPerMapRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
        yOffset += EditorGUIUtility.singleLineHeight + 2;
        var maxPerNightRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);

        // Draw fields
        EditorGUI.PropertyField(demonTypeRect, property.FindPropertyRelative("demonType"), GUIContent.none);
        var options = property.FindPropertyRelative("demonSpawnType").enumValueIndex;
        EditorGUI.PropertyField(spawnTypeRect, property.FindPropertyRelative("demonSpawnType"), GUIContent.none);
        if(options == 0)
            EditorGUI.PropertyField(spawnChanceRect, property.FindPropertyRelative("spawnChance"), GUIContent.none);
        else if (options == 1)
            EditorGUI.PropertyField(spawnXDemonRect, property.FindPropertyRelative("spawnEveryXDemon"), GUIContent.none);

        EditorGUI.PropertyField(maxPerQueueRect, property.FindPropertyRelative("maxPerQueue"), new GUIContent("Max Per Queue"));
        EditorGUI.PropertyField(maxPerMapRect, property.FindPropertyRelative("maxPerMap"), new GUIContent("Max Per Map"));
        EditorGUI.PropertyField(maxPerNightRect, property.FindPropertyRelative("maxPerNight"), new GUIContent("Max Per Night"));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4 + 3 * 2 + 4; // 4 lines, 3 spacings of 2px, 4px padding
    }
}