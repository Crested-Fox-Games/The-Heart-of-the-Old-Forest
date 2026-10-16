using UnityEngine;
using UnityEditor;

public class TreeReplacer : EditorWindow
{
    private Terrain targetTerrain;

    [MenuItem("Tools/Convert Terrain Trees")]
    public static void ShowWindow()
    {
        GetWindow<TreeReplacer>("Tree Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Convert Terrain Trees to GameObjects", EditorStyles.boldLabel);
        targetTerrain = (Terrain)EditorGUILayout.ObjectField("Target Terrain", targetTerrain, typeof(Terrain), true);

        if (GUILayout.Button("Convert Trees to Objects"))
        {
            if (targetTerrain == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Target Terrain first!", "OK");
                return;
            }
            ConvertTrees();
        }
    }

    private void ConvertTrees()
    {
        TerrainData data = targetTerrain.terrainData;
        GameObject root = new GameObject("Converted_Trees_Root");
        Undo.RegisterCreatedObjectUndo(root, "Convert Terrain Trees");

        for (int i = 0; i < data.treeInstances.Length; i++)
        {
            TreeInstance tree = data.treeInstances[i];

            // Get the prefab associated with this tree type
            GameObject prefab = data.treePrototypes[tree.prototypeIndex].prefab;
            if (prefab == null) continue;

            // Calculate actual world position from terrain relative position
            Vector3 worldPos = Vector3.Scale(tree.position, data.size) + targetTerrain.transform.position;

            // Instantiate the prefab
            GameObject newTree = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            newTree.transform.position = worldPos;
            newTree.transform.parent = root.transform;

            // Apply rotation (Terrain trees only rotate around the Y axis)
            newTree.transform.rotation = Quaternion.AngleAxis(tree.rotation * Mathf.Rad2Deg, Vector3.up);

            // Apply random scale based on terrain settings
            newTree.transform.localScale = new Vector3(tree.widthScale, tree.heightScale, tree.widthScale);
        }

        EditorUtility.DisplayDialog("Success", $"Converted {data.treeInstances.Length} trees into GameObjects!", "OK");
    }
}
