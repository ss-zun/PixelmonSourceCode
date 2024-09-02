using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEditor;

public class PrefabAtlasChecker : EditorWindow
{
    // GameObject 배열을 직접 선언
    public GameObject[] selectedPrefabs;

    [MenuItem("Tools/Prefab Atlas Checker")]
    public static void ShowWindow()
    {
        GetWindow<PrefabAtlasChecker>("Prefab Atlas Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Drag & Drop Prefabs to Check", EditorStyles.boldLabel);

        // 드래그 앤 드롭으로 여러 개의 프리팹을 받을 수 있게 함
        EditorGUI.BeginChangeCheck();
        int newSize = Mathf.Max(0, EditorGUILayout.IntField("Number of Prefabs", selectedPrefabs != null ? selectedPrefabs.Length : 0));
        if (selectedPrefabs == null || newSize != selectedPrefabs.Length)
        {
            GameObject[] newSelectedPrefabs = new GameObject[newSize];
            for (int i = 0; i < Mathf.Min(newSize, selectedPrefabs != null ? selectedPrefabs.Length : 0); i++)
            {
                newSelectedPrefabs[i] = selectedPrefabs[i];
            }
            selectedPrefabs = newSelectedPrefabs;
        }

        for (int i = 0; i < newSize; i++)
        {
            selectedPrefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i + 1}", selectedPrefabs[i], typeof(GameObject), false);
        }

        if (GUILayout.Button("Check Prefabs for Non-Atlas Sprites"))
        {
            if (selectedPrefabs != null && selectedPrefabs.Length > 0)
            {
                CheckSelectedPrefabsForNonAtlasSprites();
            }
            else
            {
                Debug.LogWarning("No prefabs selected!");
            }
        }
    }

    private void CheckSelectedPrefabsForNonAtlasSprites()
    {
        SpriteAtlas[] atlases = Resources.FindObjectsOfTypeAll<SpriteAtlas>();

        Debug.Log("Checking selected prefabs for non-atlas sprites...");

        foreach (var prefab in selectedPrefabs)
        {
            if (prefab != null)
            {
                CheckGameObjectForNonAtlasSprites(prefab, atlases, AssetDatabase.GetAssetPath(prefab));
            }
        }

        Debug.Log("Selected prefabs check complete.");
    }

    private void CheckGameObjectForNonAtlasSprites(GameObject gameObject, SpriteAtlas[] atlases, string assetPath)
    {
        Image[] images = gameObject.GetComponentsInChildren<Image>(true);
        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var image in images)
        {
            if (image.sprite != null && !IsSpriteInAnyAtlas(image.sprite, atlases))
            {
                Debug.Log("Prefab Image not in any atlas: " + gameObject.name + " - " + assetPath + " - " + AssetDatabase.GetAssetPath(image.sprite));
            }
        }

        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.sprite != null && !IsSpriteInAnyAtlas(spriteRenderer.sprite, atlases))
            {
                Debug.Log("Prefab SpriteRenderer not in any atlas: " + gameObject.name + " - " + assetPath + " - " + AssetDatabase.GetAssetPath(spriteRenderer.sprite));
            }
        }
    }

    private bool IsSpriteInAnyAtlas(Sprite sprite, SpriteAtlas[] atlases)
    {
        foreach (var atlas in atlases)
        {
            if (atlas.GetSprite(sprite.name) != null)
            {
                return true;
            }
        }
        return false;
    }
}
