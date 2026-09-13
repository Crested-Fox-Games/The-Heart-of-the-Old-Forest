using System;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(AbilitySO))]
public class AbilitySOEditor : Editor
{
    private Type[] abilityTypes;
    private string[] abilityTypeNames;

    private void OnEnable()
    {
        //Find every non abstract class that inherits from ability
        abilityTypes = TypeCache.GetTypesDerivedFrom<Ability>()
            .Where(type => !type.IsAbstract)
            .OrderBy(type => type.Name)
            .ToArray();

        //Create the names displayed in the drop down
        abilityTypeNames = new string [abilityTypes.Length + 1];

        abilityTypeNames[0] = "None";

        for(int i = 0; i < abilityTypes.Length; i++)
        {
            abilityTypeNames[i + 1] = abilityTypes[i].Name;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Draw the normal AbilitySO fields
        DrawPropertiesExcluding(serializedObject, "abilityTypeName");

        SerializedProperty abilityTypeProperty = serializedObject.FindProperty("abilityTypeName");

        //Find which ability is currently selected
        int currentIndex = 0;

        for (int i = 0; i < abilityTypes.Length; i++)
        {
            if (abilityTypes[i].AssemblyQualifiedName == abilityTypeProperty.stringValue)
            {
                currentIndex = i + 1;
                break;
            }
        }

        //Draw the dropdown
        int newIndex = EditorGUILayout.Popup("Ability Type", currentIndex, abilityTypeNames);

        if(newIndex == 0)
        {
            abilityTypeProperty.stringValue = "";
        }
        else
        {
            abilityTypeProperty.stringValue = abilityTypes[newIndex - 1].AssemblyQualifiedName;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
