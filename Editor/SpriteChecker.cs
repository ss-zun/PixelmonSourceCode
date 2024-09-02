using UnityEngine;
using UnityEditor;
using UnityEngine.U2D; // Sprite Atlas 관련 API를 사용하기 위해 필요합니다.

public class SpriteChecker : EditorWindow
{
    private Sprite spriteToCheck;

    [MenuItem("Tools/Sprite Checker")]
    public static void ShowWindow()
    {
        GetWindow<SpriteChecker>("Sprite Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Check if a Sprite is in any Sprite Atlas", EditorStyles.boldLabel);

        // 드래그 앤 드롭을 위한 필드
        spriteToCheck = (Sprite)EditorGUILayout.ObjectField("Sprite", spriteToCheck, typeof(Sprite), false);

        if (spriteToCheck != null)
        {
            if (GUILayout.Button("Check Sprite"))
            {
                CheckSpriteInAtlases(spriteToCheck);
            }
        }
    }

    private void CheckSpriteInAtlases(Sprite sprite)
    {
        string[] atlasGuids = AssetDatabase.FindAssets("t:SpriteAtlas");
        bool found = false;

        foreach (string guid in atlasGuids)
        {
            string atlasPath = AssetDatabase.GUIDToAssetPath(guid);
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

            if (atlas != null && atlas.CanBindTo(sprite))
            {
                found = true;
                Debug.Log($"Sprite '{sprite.name}' is included in Atlas: {atlasPath}");
            }
        }

        if (!found)
        {
            Debug.LogWarning($"Sprite '{sprite.name}' is not included in any Sprite Atlas.");
        }
    }
}
