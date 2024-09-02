using UnityEditor;
using UnityEngine;
using TMPro;

public class ChangeFontTMPEditor : EditorWindow
{
    private TMP_FontAsset newFont;
    private GameObject selectedPrefab;

    [MenuItem("Tools/Change TMP Font")]
    public static void ShowWindow()
    {
        GetWindow<ChangeFontTMPEditor>("Change TMP Font");
    }

    private void OnGUI()
    {
        GUILayout.Label("Change TMP Font", EditorStyles.boldLabel);

        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font", newFont, typeof(TMP_FontAsset), false);
        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Selected Prefab", selectedPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Change Font in Scene"))
        {
            ChangeFontInScene();
        }

        if (GUILayout.Button("Change Font in Prefabs"))
        {
            ChangeFontInPrefabs();
        }

        if (GUILayout.Button("Change Font in Selected Prefab"))
        {
            ChangeFontInSelectedPrefab();
        }
    }

    private void ChangeFontInScene()
    {
        if (newFont == null)
        {
            //Debug.LogError("Please assign a new font.");
            return;
        }

        TMP_Text[] textComponents = FindObjectsOfType<TMP_Text>();

        foreach (TMP_Text text in textComponents)
        {
            Undo.RecordObject(text, "Change TMP Font");
            text.font = newFont;
            EditorUtility.SetDirty(text);
        }

        //Debug.Log("Font change completed in the current scene!");
    }

    private void ChangeFontInPrefabs()
    {
        if (newFont == null)
        {
            //Debug.LogError("Please assign a new font.");
            return;
        }

        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) continue;

            TMP_Text[] textComponents = prefab.GetComponentsInChildren<TMP_Text>(true);
            if (textComponents.Length > 0)
            {
                foreach (TMP_Text text in textComponents)
                {
                    Undo.RecordObject(text, "Change TMP Font");
                    text.font = newFont;
                    EditorUtility.SetDirty(text);
                }
                PrefabUtility.SavePrefabAsset(prefab);
            }
        }

        //Debug.Log("Font change completed in all prefabs!");
    }

    private void ChangeFontInSelectedPrefab()
    {
        if (newFont == null)
        {
            //Debug.LogError("Please assign a new font.");
            return;
        }

        if (selectedPrefab == null)
        {
            //Debug.LogError("Please assign a prefab.");
            return;
        }

        TMP_Text[] textComponents = selectedPrefab.GetComponentsInChildren<TMP_Text>(true);
        if (textComponents.Length > 0)
        {
            foreach (TMP_Text text in textComponents)
            {
                Undo.RecordObject(text, "Change TMP Font");
                text.font = newFont;
                EditorUtility.SetDirty(text);
            }
            PrefabUtility.SavePrefabAsset(selectedPrefab);
        }

        //Debug.Log("Font change completed in the selected prefab!");
    }
}
